Shader "URP/ToonHair"
{
    Properties
    {
        _MainTex      ("MainTex", 2D)        = "white" {}
        _RampTex      ("RampTex", 2D)        = "white" {}
        
        _MainColor    ("Main Color", Color)  = (1,1,1,1)
        _ShadowColor  ("Shadow Color", Color)= (0.7,0.7,0.7,1)
        _ShadowRange  ("Shadow Range", Range(0,1)) = 0.7
        _ShadowSmooth ("Shadow Smooth", Range(0,0.03))= 0.002
        
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularRange ("Specular Range", Range(0,1)) = 0.35
        _SpecularMulti ("Specular Multi", Range(0,1)) = 0.4
        _SpecularGloss ("Specular Gloss", Range(0.001,0.01)) = 0.003
        
        _AnisotropicRange ("Anisotropic Range", Range(0,200)) = 50
        
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
                half   _ShadowSmooth;//明暗分界线的一小块区域
            
                half4  _SpecularColor;
                half   _SpecularRange;
                half   _SpecularMulti;
                half   _SpecularGloss;

                half _AnisotropicRange;
            
                half4  _RimColor;
                half   _RimPower;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 worldPos    : TEXCOORD1; 
                float3 worldNormal : TEXCOORD2;
                float3 worldTangent : TEXCOORD3;
                float3 worldBitangent : TEXCOORD4;
                float3 lightDirTS : TEXCOORD5;
                float3 viewDirTS : TEXCOORD6;
            };
            
            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.worldPos    = TransformObjectToWorld(IN.positionOS.xyz);
                
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.worldTangent = TransformObjectToWorldDir(IN.tangentOS.xyz);
                OUT.worldBitangent = cross(OUT.worldNormal,OUT.worldTangent) * IN.tangentOS.w * unity_WorldTransformParams.w; // 叉乘,注意符号和缩放
                float3x3 TBN = float3x3(OUT.worldTangent, OUT.worldBitangent, OUT.worldNormal);
                //OUT.lightDirTS = mul(TBN, mainLight.direction);
                return OUT;
            }
            
            half4 frag (Varyings IN) : SV_Target
            {
                // 主光源的光照衰减
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(IN.worldPos)); 
                half atten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                
                half3 worldNormal = normalize(IN.worldNormal);
                half3 lightDir    = normalize(mainLight.direction);
                half3 viewDir     = normalize(GetWorldSpaceViewDir(IN.worldPos));

                // Ambient
                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
                half ao = SampleAmbientOcclusion(IN.uv);
                half3 ambient = ao * albedo; //环境光叠加材质颜色
                
                // 漫反射
                half halfLambert = saturate(dot(worldNormal, lightDir) * 0.5 + 0.5);
                half3 diffuse = _MainColor.rgb * albedo * halfLambert;
                
                // 阴影-明暗交界
                half4 ramp = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, IN.uv);
                half rampDelta = saturate(_ShadowRange - (halfLambert + ramp.g) * 0.5); //计算当前像素和明暗分界线的距离，约定取r或g通道存阴影Δ偏移值，归一化到0-1间
                ramp = smoothstep(0, _ShadowSmooth, rampDelta); //平滑过渡带
                diffuse = lerp(diffuse, _ShadowColor.rgb, ramp);  
                
                // Kajiya-kay 高光
                half3 H = normalize(lightDir + viewDir); //半程
                half dotTH = saturate(dot(IN.worldBitangent,H));
                half3 sinTH =  sqrt(1 - dotTH * dotTH);
                half dirAtten = smoothstep(-1,0, dotTH); //随视角变化的衰减项  
                half3 anisotropic = _SpecularColor.rgb * _SpecularMulti * dirAtten * pow(sinTH, _AnisotropicRange);
                
                // 边缘光
                half rim = 1 - saturate(dot(viewDir, worldNormal));  
                half3 rimColor = _RimColor.rgb * pow(rim, 1 / _RimPower);
                
                half3 final = ambient + diffuse + anisotropic;
                return half4(final, 1.0);
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
                float3 posVS = TransformWorldToView(TransformObjectToWorld(IN.positionOS.xyz));
                float3 viewNormal = TransformWorldToViewDir(TransformObjectToWorldNormal(IN.normalOS));
                
                viewNormal.z = -0.5; // 指定z轴，避免穿帮
                
                posVS = posVS + viewNormal * _OutlineWidth * 0.01;
                OUT.positionHCS = TransformWViewToHClip(posVS);
                return OUT;
            }
            
            half4 frag (Varyings IN) : SV_Target
            {
                return half4(_OutlineColor.rgb,1.0);
            }
            ENDHLSL
        }

        // ---------- 投影 Pass ----------
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