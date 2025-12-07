Shader "URP/SDFface"
{
    Properties
    {
        _MainTex     ("MainTex", 2D)   = "white" {}
        _RampTex     ("RampTex", 2D)   = "white" {}
        _SDFTex      ("SDFMap", 2D)    = "white" {}

        _MainColor   ("Main Color", Color)       = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color)     = (0.7,0.7,0.8,1)
        _ShadowRange ("Shadow Range", Range(0,1))= 0.5
        _ShadowSmooth("Shadow Smooth",Range(0,1))= 0.2

        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _SpecularRange ("Specular Range", Range(0,1)) = 0.9
        _SpecularMulti ("Specular Multi", Range(0,1)) = 0.4
        _SpecularGloss ("Specular Gloss", Range(0.001,8)) = 4

        _RimColor ("Rim Color", Color) = (0,0,0,1)
        _RimPower ("Rim Power", Range(0.0001,5)) = 0.0001

        _OutlineWidth ("Outline Width", Range(0,1)) = 0.24
        _OutlineColor ("Outline Color", Color) = (0.5,0.5,0.5,1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }

        // ---------- SDF Toon Lighting ----------
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);   SAMPLER(sampler_MainTex);
            TEXTURE2D(_RampTex);   SAMPLER(sampler_RampTex);
            TEXTURE2D(_SDFTex);    SAMPLER(sampler_SDFTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _RampTex_ST;
                float4 _SDFTex_ST;
                half4  _MainColor, _ShadowColor;
                half   _ShadowRange, _ShadowSmooth;
                half4  _SpecularColor;
                half   _SpecularRange, _SpecularMulti, _SpecularGloss;
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
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 worldPos    : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 upDir       : TEXCOORD3;
                float3 rightDir    : TEXCOORD4;
                float3 forwardDir  : TEXCOORD5;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos    = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);

                // 本地→世界 方向向量
                OUT.upDir      = TransformObjectToWorldDir(float3(0,1,0));
                OUT.rightDir   = TransformObjectToWorldDir(float3(1,0,0));
                OUT.forwardDir = TransformObjectToWorldDir(float3(0,0,1));
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half3 albedo   = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
                half4 rampTex  = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, IN.uv);
                half4 sdfTex   = SAMPLE_TEXTURE2D(_SDFTex, sampler_SDFTex, IN.uv);

                half3 worldNormal = normalize(IN.worldNormal);
                half3 worldLightDir = normalize(GetMainLight().direction);
                half3 viewDir = normalize(GetWorldSpaceViewDir(IN.worldPos));

                // 左右脸 SDF 逻辑
                half3 upDir      = normalize(IN.upDir);
                half3 rightDir   = normalize(IN.rightDir);
                half3 forwardDir = normalize(IN.forwardDir);

                half3 lightProjUp   = dot(worldLightDir, upDir) * upDir;
                half3 lightHorizon  = worldLightDir - lightProjUp;
                //const half PI = 3.141592653589793;
                half theta = acos(dot(normalize(lightHorizon), normalize(rightDir))) / PI;
                bool isRight = theta < 0.5;
                half shadowThresholdL = pow(theta * 2 - 1, 3);
                half shadowThresholdR = pow(1 - theta * 2, 3);
                half shadowThreshold  = lerp(shadowThresholdL, shadowThresholdR, isRight);

                half sdfShadowL = sdfTex.r;
                half sdfShadowR = SAMPLE_TEXTURE2D(_SDFTex, sampler_SDFTex, half2(1 - IN.uv.x, IN.uv.y)).r;
                half sdfShadow  = lerp(sdfShadowL, sdfShadowR, isRight);

                half sdfFactor = lerp(0, step(shadowThreshold, sdfShadow),
                                      step(0, dot(normalize(lightHorizon), forwardDir)));

                half3 diffuse = lerp(_MainColor.rgb, _ShadowColor.rgb, sdfFactor) * albedo;

                // Blinn-Phong 高光
                half3 halfDir = normalize(worldLightDir + viewDir);
                half nh = saturate(dot(worldNormal, halfDir));
                half specRange = pow(nh, _SpecularGloss);
                half specMask = rampTex.b;
                half3 specular = 0;
                if (specRange >= 1 - specMask * _ShadowRange)
                    specular = _SpecularMulti * rampTex.r * _SpecularColor.rgb;

                // 边缘光
                half rim = 1 - saturate(dot(viewDir, worldNormal));
                half3 rimColor = _RimColor.rgb * pow(rim, 1 / _RimPower);

                Light mainLight = GetMainLight();
                half3 final = (diffuse + specular + rimColor) * mainLight.color;
                return half4(final, 1);
            }
            ENDHLSL
        }

        // ---------- Outline ----------
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }
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
    }
    FallBack Off
}