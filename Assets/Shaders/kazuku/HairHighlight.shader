Shader "Unlit/HairHighlight"
{
    //第一个shader pass实现光照，第二个实现描边
    Properties
    {
        //texures
       _MainTex("MainTex", 2D) = "white" {}
       _RampTex("RampTex",2D) = "white" {}
        //_SDFTex("SDFMap",2D) = "white"{}
        [Space(10)]
        
        //highlight shading
        _SpecularColor("Specular Color",Color) = (1,1,1)
        _SpecularRange("Specular Range",Range(0,1)) = 0.9
        _SpecularMulti("Specular Multi",Range(0,1)) = 0.4
        _SpecularGloss("Specular Gloss",Range(0.001,8)) = 4
        [Space(10)]
        
        //cel shading
       _MainColor("Main Color",Color)=(1,1,1)                               
       _ShadowColor("Shadow Color",Color)=(0.7,0.7,0.8)
       _ShadowRange("Shadow Range",Range(0,1))=0.5
       _ShadowSmooth("Shadow Smooth",Range(0,1))=0.2 //阴影边缘
       [Space(10)] //在属性面板加间隔                                                                                                                                                                                                                                  
        
        //rim&bloom
        _RimColor("Rim Color",Color) = (0,0,0,1)
        _RimPower("Rim Power",Range(0.0001,5.0))=0.0001
        [Space(10)]
        
        //outline
       _OutlineWidth("OutlineWidth",Range(0,1)) = 0.24
       _OutlineColor ("OutlineColor",Color) = (0.5,0.5,0.5,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        //double color gradient
        Pass
        {
            Tags {"LightMode" = "ForwardBase"} 
            
            Cull Back//相机背面剔除                                                                                                                                                                     
            
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _RampTex;
            float4 _RampTex_ST;
            sampler2D _SDFTex;
            float4 _SDFTex_ST;
            
            half3 _MainColor;
            half3 _ShadowColor;
            half _ShadowSmooth;
            half _ShadowRange;

            half3 _SpecularColor;
            half _SpecularRange;
            half _SpecularMulti;
            half _SpecularGloss;

            half4 _RimColor;
            float _RimPower;

            struct a2v //从应用阶段传过来数据
            {
                float4 vertex :POSITION; //用模型空间顶点坐标填充自定义的vertex变量
                float3 normal : NORMAL;
                float2 uv:TEXCOORD0; //buildin管线4套，通用8+套
                float3 tangent : TANGENT;
            };

            struct v2f //定义顶点shader输出的结构体
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos :TEXCOORD2;
                //模型在世界空间下的前、上、右向量,计算sdf
                //距离场贴图实际记录了单方向光照，所以计算另一方向的光照时需要翻转采样
                float3 upDir :TEXCOORD3;
                float3 rightDir : TEXCOORD4;
                float3 forwardDir : TEXCOORD5;
                UNITY_FOG_COORDS(6)
                float3 worldTangent :TEXCOORD7;
            };

            v2f vert(a2v v) //几何阶段,顶点shader，处理光照
            {
                v2f o;
                
                UNITY_INITIALIZE_OUTPUT(v2f,o);//宏，报错了用
                o.uv = TRANSFORM_TEX(v.uv,_MainTex); //宏，对纹理缩放位移
                
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldTangent = UnityObjectToWorldNormal(v.tangent);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                
                //规定的loacl坐标系up方向：float3(0, 1, 0)
                o.upDir = UnityObjectToWorldDir(float3(0,1,0));
                o.rightDir = UnityObjectToWorldDir(float3(1,0,0));
                o.forwardDir = UnityObjectToWorldDir(float3(0,0,1));
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            half4 frag(v2f i):SV_TARGET{   //光栅化阶段，逐像素计算颜色
                half4 color = 0;
                half4 mainTex = tex2D(_MainTex,i.uv);
                half4 rampTex = tex2D(_RampTex,i.uv);
                
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz-i.worldPos.xyz);
                half3 worldNormal = normalize(i.worldNormal);
                half3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

                //计算光照
                //漫反射
                half3 diffuse = 0;
                half halfLambert = dot(worldNormal,worldLightDir)*0.5 + 0.5;
                half Conditional = (halfLambert + rampTex.g)*0.5;
                
                //ramp贴图,比较半兰伯特光照和阴影范围大小实现双阶色调
                half ramp = saturate(_ShadowRange-Conditional);
                ramp =  smoothstep(0,_ShadowSmooth,ramp);
                diffuse = lerp(_MainColor,_ShadowColor,ramp);
                diffuse *= mainTex.rgb;
                
                //BlinnPhong高光
                half3 specular = 0;
                half3 halfDir = normalize(worldLightDir + viewDir); //半程向量
                half NdotH = max(0,dot(worldNormal,halfDir));
                half SpecularRange = pow(NdotH,_SpecularGloss);
                half specularMask = rampTex.b;//遮罩纹理
                //动态调整高光区域：mask越大，range越大越容易有高光
                if (SpecularRange >= 1- specularMask*_ShadowRange)
                {
                    specular = _SpecularMulti* rampTex.r * _ShadowColor;
                }
                //模拟各向异性高光
                half3 anicoDir = normalize(i.worldTangent + viewDir);
                half3 H = normalize(worldLightDir + viewDir);
                half aniso = 0;
                aniso = max(0,dot(aniso,H));
                half anisoSpecular = pow(aniso,_SpecularColor*8);
                specular += _SpecularColor *anisoSpecular;
                //边缘光
                float rimCond = 1 - max(0,dot(viewDir,worldNormal));
                half3 rimColor = _RimColor * pow(rimCond,1/_RimPower);
                color.a = rimColor * NdotH;
                color.rgb = _LightColor0.rgb * (diffuse + specular+ rimColor); //buildin管线的_LightColor0
                return color;
            }
            ENDCG
        }
     
        
//第二个pass顶点沿法线位移一段距离描边
        Pass
        {
            
            Tags{"LightMode" = "ForwardBase"}
            
            Cull Front //提出模型正面
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment  frag
            #include "UnityCG.cginc"

            half _OutlineWidth;
            half4 _OutlineColor;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 vertColor : COLOR;
                float4 tangent : TANGENT;
            };

                struct v2f
                {
                    float4 pos:SV_POSITION;
                };

                v2f vert(a2v v)
                {
                    v2f o;
                    UNITY_INITIALIZE_OUTPUT(v2f,o);
                    float4 pos = UnityObjectToClipPos(v.vertex);
                    //改善近粗远细：法线外扩toNDC空间的距离外扩
                    float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV,v.tangent.xyz);
                    float3 ndcNormal = normalize(TransformViewToProjection(viewNormal.xyz)) * pos.w;//vN 2 ndcN
                    pos.xy += 0.01 * _OutlineWidth * ndcNormal.xy;
                    o.pos = pos;
                    return o;
                }

                half4 frag(v2f i): SV_Target
                {
                    return _OutlineColor;
                }
                ENDCG
                
              }  
    }
}
