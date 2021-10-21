// base version is taken from https://github.com/nubick/unity-utils/blob/master/sources/Assets/Scripts/Shaders/Sprites-Default.shader
Shader "Unlit/HealthBarSprite_unlit"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Color2("Color2", Color) = (1,1,1,1)
		_SplitPos("Split pos", float) = 0.5
		[MaterialToggle] PixelSnap("Pixel snap", float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile _ PIXELSNAP_ON
				#include "UnityCG.cginc"

				struct appdata_t
				{
					half4 vertex   : POSITION;
					half4 color    : COLOR;
					half2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					half4 vertex   : SV_POSITION;
					half4 color : COLOR;
					half2 texcoord  : TEXCOORD0;
				};

				half4 _Color;
				half4 _Color2;
				half _SplitPos;

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color * _Color;
					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif

					return OUT;
				}

				sampler2D _MainTex;
				sampler2D _AlphaTex;
				half _AlphaSplitEnabled;

				half4 SampleSpriteTexture(half2 uv)
				{
					half4 color = tex2D(_MainTex, uv);
					return color;
				}

				half4 frag(v2f IN) : SV_Target
				{
					half4 c = tex2D(_MainTex, IN.texcoord);
					if (IN.texcoord.x > _SplitPos)
						c *= _Color;
					else
						c *= _Color2;

					c.a = 1;
					//c.rgb *= c.a;
					return c;
				}
			ENDHLSL
			}
		}
}
