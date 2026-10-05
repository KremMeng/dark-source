Shader "URP/SwordSlash"
//particle alpha-blend shader---ASE
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white"{}
        _DissolveTex("溶解纹理",2D) = "white"{}
        
        _MainColor ("Main Color",Color) = (1,1,1,1)
        _ParticleColor("ParticleColor",Color)=(1,1,1,1)
        
        _Opacity("刀光透明度",Float) = 20 
        //_DissolveFactor("溶解系数",Range(0,1)) = 1
        //_DissolveThreshold("溶解阈值",Range(0,1))=0.3
    }
    
    SubShader
    {
        Tags {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off //关闭深度写入，防止物体相互覆盖遮挡，无法有透明效果
        Cull Off
        
        LOD 100

        Pass
        {
            Name "Forward"
            Tags{"LightMode"="UniversalForward"}
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST; //Tiling、Offset属性
                float4 _DissolveTex_ST;
                float4 _MainColor;
                float4 _ParticleColor;
                float _Opacity;
                //float _DissolveFactor;
                //float _DissolveThreshold;
            CBUFFER_END

            TEXTURE2D(_MainTex);  SAMPLER(sampler_MainTex);
            TEXTURE2D(_DissolveTex);  SAMPLER(sampler_DissolveTex);

            struct Attributes //顶点输入属性
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;//改成四个分量，粒子系统Custom Vertex Streams的自定义数据存在uv.zw里
            };

            struct Varyings //varying variables插值变量，光栅化阶段数据在插值后传给片元；v2f：Verterx2FragmentShader
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 custom1 : TEXCOORD1; //粒子系统传入
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.vertex);
                OUT.uv = TRANSFORM_TEX(IN.uv.xy, _MainTex);//= v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                OUT.custom1 = IN.uv.zw;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                //w、t参数直接在Custom Data里加个Vector2来调整，w-随时间变化的曲线，t-固定值y=1
                //一开始放在TEXCOORD0的zw分量，拆开后现在是放在float2的xy
                float w = IN.custom1.x;
                float t = IN.custom1.y;
                //float t = _Time.y;//单位时间/秒，经验证这个不对
                
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,IN.uv);
                color.rgb = color.rgb * _MainColor.rgb;
                color.a = color.a * _Opacity; //main tex乘以opacity
                color.a = clamp(color.a,0,1); //把透明度clamp到0，1之间,ASE是直接clamp整体

                //溶解：噪声贴图，根据阈值输出0或1的黑白遮罩,step或smoothstep
                half4 dissolve = SAMPLE_TEXTURE2D(_DissolveTex,sampler_MainTex,IN.uv);
                //dissolve *= _DissolveFactor;
                //dissolve = step(_DissolveThreshold,dissolve);//if in > threshold,output 1
                dissolve *= t;
                dissolve = step(w,dissolve); //曲线：阈值w随着曲线变化，刀光从0->1变深

                color = color * _ParticleColor * dissolve;
                
                return color;
            }
            ENDHLSL
        }
    }
}
