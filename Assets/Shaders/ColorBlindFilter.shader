Shader "Unlit/ColorBlindFilter"
{
    Properties
    {
        _CollisionSpeed("Collision Speed", float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "Universal"
        }

        LOD 100

        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            //#include "UnityCG.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct appdata
            {
                half3 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half2 posUV : TEXCOORD1;
                half4 vertex : SV_POSITION;
            };

            half4 _CameraOpaqueTexture_TexelSize;
            half _CollisionSpeed;

            half3 GetBlurredScreenColor(in const half2 uv, half strength);

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                o.posUV = (ComputeScreenPos(o.vertex) / o.vertex.w).xy;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                //half lum = c.r * .3 + c.g * .59 + c.b * .11;
                half4 col = half4(SampleSceneColor(i.posUV), 1);// *half4(0.3, 0.59, 0.11, 1);
                return col;
            }

            half3 GetBlurredScreenColor(in const half2 uv, half strength)
            {
                half4 offset = _CameraOpaqueTexture_TexelSize.xyxy * half4(-1.0, -1.0, 1.0, 1.0) * strength;

                half3 sum = SampleSceneColor(uv + offset.xy);
                sum += SampleSceneColor(uv + offset.zy);
                sum += SampleSceneColor(uv + offset.xw);
                sum += SampleSceneColor(uv + offset.zw);

                return sum * 0.25f;
            }

            ENDHLSL
        }
    }
}
