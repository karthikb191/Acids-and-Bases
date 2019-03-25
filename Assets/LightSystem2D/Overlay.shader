Shader "Hidden/Overlay"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BrightnessMultiplier("Brightness Multiplier", Float) = 1
		_Emission("Emission Multiplier", Float) = 1
		_SampleRate("Ambience Sample Rate", Float) = 0.01
		//_Interations("Iterations", Int) = 4;
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		//Blend One One
		Blend SrcAlpha OneMinusSrcAlpha
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
				
			};
			
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 lightsUv : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _LightsTexture;
			float4 _LightsTexture_TexelSize;
			float _BrightnessMultiplier;
			float _Emission;
			float _SampleRate;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.lightsUv = v.uv;
				return o;
			}
			

			fixed4 frag (v2f i) : SV_Target
			{
				//fixed4 finalCol;
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 lightsCol = tex2D(_LightsTexture, i.lightsUv);

				fixed4 ambienceCol;
				//fixed4 oldCol = tex2D(_MainTex, i.lightsUv);

				half4 maxLight = tex2D(_LightsTexture, i.uv + half2(_SampleRate, 0));
				maxLight = max(maxLight, tex2D(_LightsTexture, i.uv + half2(-_SampleRate, 0)));
				maxLight = max(maxLight, tex2D(_LightsTexture, i.uv + half2(0, _SampleRate)));
				maxLight = max(maxLight, tex2D(_LightsTexture, i.uv + half2(0, -_SampleRate)));
				
				half dist45 = _SampleRate * 0.707;
				maxLight = max(maxLight, tex2D(_LightsTexture, i.uv + half2(dist45, dist45)));
				maxLight = max(maxLight, tex2D(_LightsTexture, i.uv + half2(dist45, -dist45)));
				maxLight = max(maxLight, tex2D(_LightsTexture, i.uv + half2(-dist45, dist45)));
				maxLight = max(maxLight, tex2D(_LightsTexture, i.uv + half2(-dist45, -dist45)));

				

				//maxLight = max(maxLight, tex2D(_LightsTexture, i.uv + half2(0, dist45)));
				//maxLight = max(maxLight, tex2D(_LightsTexture, i.uv + half2(0, -dist45)));
				//maxLight = max(maxLight, tex2D(_LightsTexture, i.uv + half2(-dist45, 0)));
				//maxLight = max(maxLight, tex2D(_LightsTexture, i.uv + half2(-dist45, -0)));
				
				
				//lightsCol = lerp(lightsCol, maxLight, 0.1);
				ambienceCol = lerp(lightsCol, maxLight, 0.8);
				//ambienceCol = lerp(lightsCol, c, 0.8);
				
				col.rgb = col.rgb * _BrightnessMultiplier * (lightsCol.rgb) * ambienceCol.rgb;// *ambienceCol.rgb; // *(0, 0, 0, lightsCol.a);
				

				return col;
			}
			ENDCG
		}
	}
}
