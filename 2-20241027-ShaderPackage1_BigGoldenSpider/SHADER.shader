Shader "A/SHADER"
{
    Properties
    {
        //5-增加主贴图UV的Alpha
        _MainTex("1_TextureAlpha", 2D) = "black" {} //5-增加主贴图UV的Alpha
        _MainTexColorStrength ("1_MainTexColorStrength", Float) = 0.0 //默认没有color强度  //5-
        _MainTexAlphaStrength  ("1_MainTexAlphaStrength", Range(0.0, 1.0)) = 1.0 //默认有alpha强度  //5-
        _MainTexPower("1_MainTexPower", Float) = 1.0 //默认对比度  //5-
        _MainTexColor("1_MainTexColor", Color) = (1,1,1,1) //5-
        //3-控制菲涅尔光
        _InRim ("2_InRim", Float) = 0.0 //控制菲涅尔光的内部 //3-
        _OutRim ("2_OutRim", Float) = 1.0 //控制菲涅尔光的外部 //3-
        //4-添加颜色
        _InColor("3_InColor", Color) = (0,0,0,0) //内光默认无颜色 //4-
        _InColorStrength ("3_InColorStrength", Float) = 1.0  //4-
        _OutColor("3_OutColor", Color) = (1,1,1,1)//外光默认实心 //4-
        _OutColorStrength ("3_OutColorStrength", Float) = 1.0 //4-
        //6-增加副贴图
        _AddTex("4_AddTex", 2D) = "black" {}//6-
        _AddTexTilling("4_AddTexTilling", Vector) = (1,1,0,0) //6-副贴图的tilling值
        _AddTexSpeed("4_AddTexSpeed", Vector) = (0,0,0,0) //6-副贴图的移动速度 后面两个值是没有用的，因为uv是一个float2值
        _AddTexColorStrength ("4_AddTexColorStrength", Float) = 0.0  //需要一个默认的颜色为1
        _AddTexColorAlphaStrength ("4_AddTexColorAlphaStrength", Range(0.0, 1.0)) = 0.0
        _AddTexPower("4_AddTexPower", Float) = 1.0 //默认对比度  //6
        _AddTexColor("4_AddTexColor", Color) = (1,1,1,1) //6
        //7-最终调整alpha值
        _FinalAlphaAdd ("5_FinalAlphaAdd", Float) = 0.0
    }
    SubShader
    {
        //1-修改Blend模式
        Tags { "Queue"="Transparent" } //1
        LOD 100

        Pass
        {
            //1-修改Blend模式
            Zwrite Off //1
            Blend SrcAlpha One //1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;//5-
                // 2- 菲涅尔光 NdotV 需要的东西
                float3 normal: NORMAL;// 2-
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;//5-
                float4 vertex : SV_POSITION;
                // 2- 菲涅尔光 NdotV 需要的东西
                float3 normal_world:TEXCOORD1;// 2-
                float3 vertex_world:TEXCOORD2;// 2-
                //6-副贴图坐标uv
                float4 pivot_world:TEXCOORD3;//6-
            };

            //5-增加主贴图UV的Alpha
            sampler2D _MainTex;//5-
            float4 _MainTex_ST;//5- 
            float _MainTexColorStrength; //默认没有color强度  //5-
            float _MainTexAlphaStrength; //默认有alpha强度  //5-
            float _MainTexPower;//默认对比度  //5-
            float4 _MainTexColor;
            //3-控制菲涅尔光
            float _InRim;//控制菲涅尔光的内部 //3-
            float _OutRim; //控制菲涅尔光的外部 //3-
            //4-添加颜色
            float4 _InColor; //内光默认无颜色 //4-
            float _InColorStrength; //4-
            float4 _OutColor;//外光默认实心 //4-
            float _OutColorStrength; //4-
            //6-增加副贴图
            sampler2D _AddTex; //6-
            float4 _AddTexTilling;//6-副贴图的tilling值
            float4 _AddTexSpeed;//6-副贴图的移动速度
            float _AddTexColorStrength; //需要一个默认的颜色为1
            float _AddTexColorAlphaStrength;
            float _AddTexPower;
            float4 _AddTexColor;//6
            //7-最终调整alpha值
            float _FinalAlphaAdd;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //5-增加主贴图UV的Alpha
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);//5-
                // 2- 菲涅尔光 NdotV 需要的东西
                o.normal_world = normalize( mul( float4(v.normal , 0.0) , unity_WorldToObject).xyz );// 2-
                o.vertex_world = mul(unity_ObjectToWorld , v.vertex).xyz;// 2-
                //6-副贴图坐标uv
                o.pivot_world = mul(unity_ObjectToWorld , float4(0,0,0,1));
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // 2- 菲涅尔光 NdotV 需要的东西
                half3 N = normalize(i.normal_world); // 2-
                half3 V = normalize(_WorldSpaceCameraPos.xyz - i.vertex_world); // 2-
                half NdotV = saturate( dot(N, V) ); //因为要放到alpha里面，所以需要它的值在0-1之间 // 2-
                half NdotV1Minus = 1-NdotV; // 2
                //3-控制菲涅尔光
                half NdotV1MinusSS = smoothstep(_InRim,_OutRim,NdotV1Minus);//3-
                //4-添加颜色，
                half3 Rimcolor = lerp (_InColor.xyz * _InColorStrength , _OutColor.xyz*_OutColorStrength,NdotV1MinusSS);
                //5-增加主贴图UV的Alpha
                half4 maintex = tex2D(_MainTex, i.uv) * _MainTexColor;//采样之后直接改颜色
                half3 maintexcolor = maintex.xyz * _MainTexColorStrength;//5-
                half maintexalpha = pow ((maintex.r * _MainTexAlphaStrength) , _MainTexPower);//5-
                //6-增加副贴图
                half2 addtexuv = (i.vertex_world.xz-i.pivot_world.xz) * _AddTexTilling.xy +_AddTexTilling.zw + (_AddTexSpeed.xy * _Time.y);//副贴图运动的一部操作
                half4 addtex = tex2D(_AddTex, addtexuv) * _AddTexColor;//采样之后直接改颜色
                half3 addtexcolor = addtex.xyz * _AddTexColorStrength;
                half addtexalpha = pow (addtex.r * _AddTexColorAlphaStrength , _AddTexPower);
                //最终输出颜色：
                half3 finalcolor = Rimcolor + maintexcolor +addtexcolor;
                half finalalpha = saturate (NdotV1MinusSS + maintexalpha + addtexalpha + _FinalAlphaAdd);
                //最终输出颜色
                return half4(finalcolor,finalalpha);
            }
            ENDCG
        }
    }
}
