Shader "Unlit/pHShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Speed ("Speed of scan", Range(0.1, 3)) = 2
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct Input {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Output {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float _Speed;

			Output vert(Input i) {
				Output o;
				o.uv = i.uv;
				o.position = UnityObjectToClipPos(i.position);
				return o;
			}

			fixed4 frag(Output o) : COLOR {
				fixed4 color = tex2D(_MainTex, o.uv);
				return color;
			}
			
			ENDCG
		}
	}
}
