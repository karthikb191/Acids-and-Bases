Shader "Unlit/LiquidMakser"
{
	Properties
	{
		[PerRendererData]_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_BodyTex("Body Texture", 2D) = "white"{}
		_LiquidColor("Liquid Color", Color) = (1, 1, 1, 1)
		_XOffset("Offset X", Range(-1, 1)) = 0
		_YOffset("Offset Y", Range(-1, 1)) = 0
		_XSize("Scale X", Range(-2, 2)) = 0
		_YSize("Scale Y", Range(-2, 2)) = 0
	}
	SubShader
	{
		Blend SrcAlpha OneMinusSrcAlpha

		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
		LOD 100
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct Input
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			struct Output
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			float4 _MainTex_ST;
			float4 _BodyTex_ST;
			float4 _Color;
			float4 _LiquidColor;
			sampler2D _MainTex;
			sampler2D _BodyTex;
			float _XOffset;
			float _YOffset;
			float _XSize;
			float _YSize;
			
			Output vert (Input i)
			{
				Output o;
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.uv = i.uv;
				o.uv2 = fixed2((i.uv2.x + _XOffset) * _XSize, (i.uv2.y + _YOffset) * _YSize);
				//o.uv = TRANSFORM_TEX(i.uv, _MainTex);
				//o.uv2 = TRANSFORM_TEX(i.uv2, _BodyTex);
				
				return o;
			}
			
			fixed4 frag (Output i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_BodyTex, i.uv2) * _LiquidColor;
				
				//fixed4 res = (1 - (col2.a + col.a)*0.85) * col + col2;
				fixed4 res = (1 - (col2.a + col.a)*0.85) * col + col2;
				return res;
			}
			ENDCG
		}
	}
}
