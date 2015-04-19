
Shader "LD/ColourKey"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		
		_BgTex("Bg Texture", 2D) = "white" {}
		
		_ForegroundTex("Foreground Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		
		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					float4 worldSpacePosition : TEXCOORD1;
				};

				float4 _MainTex_ST;
				
				sampler2D _MainTex;
				sampler2D _BgTex;
				sampler2D _ForegroundTex;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.worldSpacePosition = mul(_Object2World, v.vertex);
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					float u = frac(i.worldSpacePosition.x / 0.64f * 1.0f);
					float v = frac(i.worldSpacePosition.y / 0.64f * 1.0f);
					
					fixed4 mainCol = tex2D(_MainTex, i.texcoord);
					fixed4 bgCol = tex2D(_BgTex, float2(u, v));
					fixed4 foreCol = tex2D(_ForegroundTex, float2(u, v));
					
					if (mainCol.r >= 0.98f && mainCol.g <= 0.01f && mainCol.b <= 0.01f)
					{
						return bgCol;
					//	return fixed4(i.worldSpacePosition.x, 0.0f, 0.0f, 1.0f);
					}
					else if (mainCol.r >= 0.98f && mainCol.g <= 0.01f && mainCol.b >= 0.98f)
					{
						return foreCol;
					}
					else
					{
						return mainCol;
					}
				}
			ENDCG
		}
	}
}
