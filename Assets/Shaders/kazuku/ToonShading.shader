Shader "URP/ToonShading"
{
    Properties
    {
        _MainTex      ("MainTex", 2D)        = "white" {}
        _RampTex      ("RampTex", 2D)        = "white" {}
        
        _MainColor    ("Main Color", Color)  = (1,1,1,1)
        _ShadowColor  ("Shadow Color", Color)= (0.7,0.7,0.8,1)
        _ShadowRange  ("Shadow Range", Range(0,1)) = 0.5
        _ShadowSmooth ("Shadow Smooth", Range(0,1))= 0.2
        
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularRange ("Specular Range", Range(0,1)) = 0.9
        _SpecularMulti ("Specular Multi", Range(0,1)) = 0.4
        _SpecularGloss ("Specular Gloss", Range(0.001,8)) = 4
        
        _RimColor      ("Rim Color", Color) = (0,0,0,1)
        _RimPower      ("Rim Power", Range(0.0001,5)) = 0.0001
        
        _OutlineWidth  ("Outline Width", Range(0,1)) = 0.24
        _OutlineColor  ("Outline Color", Color) = (0.5,0.5,0.5,1)
    }
    
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }
        
        // ---------- 主光照 Pass ----------
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            TEXTURE2D(_MainTex);   SAMPLER(sampler_MainTex);
            TEXTURE2D(_RampTex);   SAMPLER(sampler_RampTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _RampTex_ST;
                half4  _MainColor;
                half4  _ShadowColor;
                half   _ShadowRange;
                half   _ShadowSmooth;
                half4  _SpecularColor;
                half   _SpecularRange;
                half   _SpecularMulti;
                half   _SpecularGloss;
                half4  _RimColor;
                half   _RimPower;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 worldPos    : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };
            
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos    = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }
            
            half4 frag (Varyings IN) : SV_Target
            {
                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
                half4 ramp   = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, IN.uv);
                
                Light mainLight = GetMainLight();
                half3 worldNormal = normalize(IN.worldNormal);
                half3 lightDir    = normalize(mainLight.direction);
                half3 viewDir     = normalize(GetWorldSpaceViewDir(IN.worldPos));
                
                // 半兰伯特
                half halfLambert = saturate(dot(worldNormal, lightDir) * 0.5 + 0.5);
                half rampU = saturate(_ShadowRange - (halfLambert + ramp.g) * 0.5);
                half rampFactor = smoothstep(0, _ShadowSmooth, rampU);
                half3 diffuse = lerp(_MainColor.rgb, _ShadowColor.rgb, rampFactor) * albedo;
                
                // Blinn-Phong 高光
                half3 halfDir = normalize(lightDir + viewDir);
                half nh = saturate(dot(worldNormal, halfDir));
                half specMask = ramp.b;
                half specRange = pow(nh, _SpecularGloss);
                half3 specular = 0;
                if (specRange >= 1 - specMask * _ShadowRange)
                    specular = _SpecularMulti * ramp.r * _SpecularColor.rgb;
                
                // 边缘光
                half rim = 1 - saturate(dot(viewDir, worldNormal));
                half3 rimColor = _RimColor.rgb * pow(rim, 1 / _RimPower);
                
                half3 final = diffuse + specular + rimColor;
                return half4(final * mainLight.color, 1);
            }
            ENDHLSL
        }
        
        // ---------- 描边 Pass ----------
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }   // 不参与光照，只画轮廓
            Cull Front
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            half _OutlineWidth;
            half4 _OutlineColor;
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };
            
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float4 pos = TransformObjectToHClip(IN.positionOS.xyz);
                float3 viewNormal = TransformWorldToViewDir(TransformObjectToWorldNormal(IN.normalOS));
                float2 offset = normalize(viewNormal.xy) * _OutlineWidth * 0.01 * pos.w;
                pos.xy += offset;
                OUT.positionHCS = pos;
                return OUT;
            }
            
            half4 frag (Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }

        // ---------- 阴影 Pass ----------
        Pass
        {
            Name "ShadowCaster"
            Tags {"LightMode"="ShadowCaster"}
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attribute
            {
                float4 positionOS : POSITION;                                                  
            };

            struct Varyings    
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attribute IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }
            half4 frag(Varyings IN):SV_Target
            {
                return 0;
            }
            ENDHLSL
            
        }
    }
    FallBack Off
}