Shader "Hidden/Light"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[PerRendererData]_Multiplier ("Multiplier", Float) = 2
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Tags{"Queue" = "Transparent" "RenderType" = "Transparent"}
		Blend One One
		//Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _Color;
			float _Multiplier;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				//col.rgba = col.rgba * i.color;
				//col.rgba = col.rgba * i.color;
				//col.rgba = col.rgba * _Color;
				col.rgb = col.rgb * i.color.rgb * _Multiplier;
				return col;
			}
			ENDCG
		}
	}
}
