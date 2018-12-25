// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "My Shaders/2D Lights/ItemShader" 
{

	Properties{
		_MainTex("", 2D) = "white" {}
		_Tex("Base Tex(RGB)", 2D) = "white" {}
		_OutlineMultiplier("Outline Multiplier", Range(0.1, 3.0)) = 0.2
		_OutlineWidth("Outline Width", Range(0.1, 40)) = 5
		_MaxDistance("Maximum Distance", Range(0.1, 10)) = 1.5
		_MinDistance("Minimum Distance", Range(0.1, 10)) = 1.5
		_Center("Center", Vector) = (0, 0, 0, 0)
		//_Multiplier("Multiplier", Range(0.1, 10)) = 0
		_InputColor("Color", Color) = (0, 0, 0, 0)
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 0)
	}

		SubShader{
			ZWrite Off

			//Blend One One
			Blend SrcAlpha OneMinusSrcAlpha
			Tags{"Queue" = "Transparent+2" "IgnoreProjector" = "Flase" "RenderType" = "Transparent"}

			Pass{
				CGPROGRAM

					#pragma vertex vert
					#pragma fragment frag
					#include "UnityCG.cginc"

					float4 _Center;

					
					fixed4 _InputColor;
					fixed4 _OutlineColor;
					fixed _Multiplier;
					sampler2D _MainTex;		// This main Tex is the sprite that is provided in the sprite editor
					sampler2D _Tex;
					fixed _OutlineMultiplier;
					float _MaxDistance;
					float _MinDistance;
					float _OutlineWidth;

					float4 _MainTex_TexelSize;	//gets the single pixel gap of the texture used in the shader

					struct vertInput {
						fixed4 vertex : POSITION;
						fixed2 texCoord : TEXCOORD0;
						fixed4 worldPos : TEXCOORD2;
					};

					struct vertOutput {
						fixed4 vertex : POSITION;
						fixed2 texCoord : TEXCOORD0;
						fixed4 worldPos : TEXCOORD2;
						fixed4 color : COLOR;
						//int pixelLightIndices[11] : TEXCOORD3;
					};

					vertOutput vert(vertInput IN) {
						vertOutput o;
						o.vertex = UnityObjectToClipPos(IN.vertex);
						o.texCoord = IN.texCoord;
						fixed4 wp = mul(unity_ObjectToWorld, IN.vertex);	//Getting the world position of the vertex
						wp = fixed4(wp.r, wp.g, 0, wp.a);
						o.worldPos = wp;

						//Calculating the vertex color for the vertex lights
						fixed4 vertexColor = fixed4(1, 1, 1, 1);

						float dist = smoothstep(_MinDistance, _MaxDistance, distance(_Center, wp));
						float c = smoothstep(0.6, 1, dist);
						//float c = clamp(dist, -1, 1);
						//float mult = log(dist) / _SpriteVisibilityMultiplier;

						o.color = (vertexColor * _InputColor * dist);
						o.color = fixed4(o.color.r * c, o.color.g * c, o.color.b * c, o.color.a);
						return o;
					}

					fixed4 frag(vertOutput o) : COLOR{
						fixed4 pixelColor = tex2D(_MainTex, o.texCoord);
						fixed4 texColor = tex2D(_Tex, o.texCoord);
						fixed4 textureColor = pixelColor * texColor;
						
						//float4 finalColor = textureColor;
						//float4 finalColor = half4(0, 0, 0, 1);
						//float4 finalColor = half4(1, 1, 1, 0.3f);

						//Get the colors of the surrounding pixels
						
						fixed4 up = tex2D(_MainTex, fixed2(o.texCoord.x, o.texCoord.y + _MainTex_TexelSize.y * _OutlineWidth));
						
						fixed4 down = tex2D(_MainTex, fixed2(o.texCoord.x, o.texCoord.y - _MainTex_TexelSize.y * _OutlineWidth));
						
						fixed4 left = tex2D(_MainTex, fixed2(o.texCoord.x - _MainTex_TexelSize.x * _OutlineWidth, o.texCoord.y));
						
						fixed4 right = tex2D(_MainTex, fixed2(o.texCoord.x + _MainTex_TexelSize.x * _OutlineWidth, o.texCoord.y));

						if (pixelColor.a != 0)
							if (up.a * down.a * left.a * right.a == 0)
								return _OutlineColor * _OutlineMultiplier * half4( 1, 1, 1, o.color.a);
						//if (up.a * up2.a * up3.a * down.a * down2.a * down3.a * left.a * left2.a * left3.a * right.a * right2.a * right3.a == 0)
						//	return _OutlineColor * _OutlineMultiplier * o.color;

						return pixelColor * o.color;


						
					}

				ENDCG

			}

		}

}
