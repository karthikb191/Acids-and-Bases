Shader "Custom/TexOverlayShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_OverlayTex("Overlay Texture(RGB)", 2D) = "white"{}
		[PerRendererData]_MaxOverlayMultiplier("Max Overlay Multiplier", Float) = 1
		[PerRendererData]_OverlayMultiplier("Overlay Multiplier", Float) = 1
	}
	SubShader {
		Cull Off
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		//Blend One One
		//Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _Color;
			sampler2D _MainTex;
			sampler2D _OverlayTex;
			fixed _OverlayMultiplier;
			half _MaxOverlayMultiplier;

			struct Input {
				fixed2 uv : TEXCOORD0;
				fixed2 uv2_OverlayTex : TEXCOORD1;
				fixed4 vertex : POSITION;
				fixed4 color : COLOR;
			};
			struct Output {
				fixed2 uv : TEXCOORD0;
				fixed2 uv2_OverlayTex : TEXCOORD1;
				fixed4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			Output vert(Input i) {
				Output o;

				o.uv = i.uv;
				o.uv2_OverlayTex = i.uv2_OverlayTex;
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.color = i.color;

				return o;
			}

			fixed4 frag(Output i) : COLOR {
				fixed4 finalCol = fixed4(1, 1, 1, 1);
				
				fixed4 defCol = tex2D(_MainTex, i.uv);
				fixed4 texCol = tex2D(_OverlayTex, i.uv2_OverlayTex);

				//finalCol = defCol + (texCol - 1)* _OverlayMultiplier;
				//finalCol = defCol + ((texCol * _OverlayMultiplier * 2 - 1) * _OverlayMultiplier * abs((i.uv.y * 2) - 1));
				//finalCol = defCol + ((texCol) * _OverlayMultiplier + abs((i.uv.y * 2) - 1) * _OverlayMultiplier) + 0.1 * _OverlayMultiplier;
				float multiplier = clamp(_OverlayMultiplier, 0, _MaxOverlayMultiplier);
				float uvMult = abs((i.uv.y * 2) - 1) * multiplier + 0.5 * multiplier;
				finalCol = defCol * _Color + (texCol - 1) * multiplier + clamp(uvMult, 0, 1.3);// ;
				return finalCol;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
