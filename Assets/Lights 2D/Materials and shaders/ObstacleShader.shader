// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "My Shaders/2D Lights/ObstacleShader" 
{

	Properties{
		_MainTex("", 2D) = "white" {}
		_Tex("Base Tex(RGB)", 2D) = "white" {}
		_Translucency("Translucency", Range(0.01, 2)) = 0
		_Opacity("Opacity", Range(0.01, 2)) = 0
		//_Multiplier("Multiplier", Range(0.1, 10)) = 0
		_Color("Color", Color) = (0, 0, 0, 0)
	}

		SubShader{
			ZWrite Off

			//Blend One One
			Blend SrcAlpha OneMinusSrcAlpha
			Tags{"Queue" = "Transparent+2" "IgnoreProjector" = "Flase" "RenderType" = "Transparent"}

			Pass{
			Tags{ "LightMode" = "ForwardBase" }
				CGPROGRAM

					#pragma vertex vert
					#pragma fragment frag
					#include "UnityCG.cginc"
					

					fixed4 _LightColors[10];
					fixed4 _LightPositions[10];
					fixed3 _LightDirections[10];

					half _LightDistances[10];
					half _NoFadeDistances[10];
					half _LightMixtureAdjustmentControllers[10];

					fixed _LightAngles[10];
					int _IsAPixelLight[10];	//the flag to check if the light is a pixel light
					fixed4 _Color;
					fixed _Multiplier;
					sampler2D _MainTex;		// This main Tex is the sprite that is provided in the sprite editor
					sampler2D _Tex;
					fixed _Translucency;
					fixed _Opacity;

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
						fixed4 vertexColor = fixed4(_Multiplier, _Multiplier, _Multiplier, 1);


						for (int i = 0; i < 4; i++) {
							int index = 1;	// The storing starts from 1 to eliminate the anomaly of 0
							if (_LightAngles[i] != 0) {
								//If the light is not a pixel light, do the following calculations
								
								//vertexColor = half4(0, 0, 0, 0);

								fixed vertToLightDist = distance(_LightPositions[i], wp);
 
								fixed reductionAlpha = smoothstep(_LightDistances[i], 0, vertToLightDist) * 1.3f;

								fixed4 vertexToLight = normalize(wp - _LightPositions[i]);
								fixed angleReduction = smoothstep(_LightAngles[i] - 2, 1, dot(_LightDirections[i], vertexToLight));


								vertexColor += _LightColors[i] * reductionAlpha * angleReduction * _Translucency;
							}
						}
						o.color = fixed4(vertexColor.r, vertexColor.g, vertexColor.b, vertexColor.a * _Opacity);
						return o;
					}

					fixed4 frag(vertOutput o) : COLOR{
						fixed4 spriteColor = tex2D(_MainTex, o.texCoord);
						fixed4 texColor = tex2D(_Tex, o.texCoord);
						fixed4 textureColor = spriteColor * texColor;
						
						//float4 finalColor = textureColor;
						//float4 finalColor = half4(0, 0, 0, 1);
						//float4 finalColor = half4(1, 1, 1, 0.3f);
						float4 finalColor = textureColor * o.color;

						/*for (int i = 0; i < 2; i++) {
							int index = i;
							
							if (_LightAngles[index] != 0 && _IsAPixelLight[index] == 1) {
								//Ignoring the first light, if the next light value of pixel light index is 0, then break the loop
								
								fixed4 currentLoopColor = fixed4(0, 0, 0, 0);
								fixed3 pixelRelToLight = o.worldPos - _LightPositions[index];
								fixed dist = distance(_LightPositions[index], o.worldPos);
							
								if (dist < _LightDistances[index]) {
							
									fixed3 pixelRelToLightNormalized = normalize(pixelRelToLight);
									//Shine will have a value from 0 to 1
									fixed shine = smoothstep(_LightAngles[index] - 0.5, 1, dot(_LightDirections[index], pixelRelToLightNormalized));
									if (shine > 0) {
										//float shine2 = smoothstep(0, 0.5, dot(_LightDirections[i], pixelRelToLightNormalized));


										//float translucency = 1 / pow(_Translucency * _LightDistances[i], 2);

										fixed s = exp2(1.5f - length(pixelRelToLight))* _Translucency;
										//float s = exp2(_LightDistances[i] - length(pixelRelToLight)) * _Translucency;
										//float s = log2(0.5 - length(pixelRelToLight * _Translucency));
										//float s2 = 1 / pow(_Translucency, shine);


										//currentLoopColor = (col) * s * shine;
										currentLoopColor = (_LightColors[index] + textureColor * 0.5f) * s * shine;
										//currentLoopColor = (_LightColors[i]);
										finalColor += fixed4(currentLoopColor.r, currentLoopColor.g, currentLoopColor.b, 0);
										finalColor = fixed4(finalColor.x, finalColor.y, finalColor.z, textureColor.a);
										//float4 highlights;
										//
										//float rDiff = abs(textureColor.r - currentLoopColor.r);
										//float gDiff = abs(textureColor.g - currentLoopColor.g);
										//float bDiff = abs(textureColor.b - currentLoopColor.b);
										//
										//highlights = float4(((1 - rDiff) + (1 - gDiff) + (1 - bDiff)), 
										//	((1 - rDiff) + (1 - gDiff) + (1 - bDiff)),
										//	((1 - rDiff) + (1 - gDiff) + (1 - bDiff)), 0);

										//finalColor += (half4(currentLoopColor.r + textureColor.r
										//	, currentLoopColor.g + textureColor.g
										//	, currentLoopColor.b + textureColor.b
										//	, 0) + highlights * 0.4f)* s * shine;
										//finalColor += half4(textureColor.r * 0.2, textureColor.g* 0.2, textureColor.b* 0.2, 0) * s * shine;

										//finalColor += half4(textureColor.r * 0.2, textureColor.g* 0.2, textureColor.b* 0.2, 0);
										//finalColor += highlights;
										//finalColor += highlights * 0.2;
										//
										//finalColor += (half4(textureColor.r
										//		, textureColor.g
										//		, textureColor.b 
										//		, 1)) * s * shine;



										//float rDiff = abs(textureColor.r - currentLoopColor.r);
										//float gDiff = abs(textureColor.g - currentLoopColor.g);
										//float bDiff = abs(textureColor.b - currentLoopColor.b);
										//finalColor += half4(currentLoopColor.r + (1 - rDiff) + (1 - gDiff) + (1 - bDiff) * textureColor.r
										//	, currentLoopColor.g + (1 - rDiff) + (1 - gDiff) + (1 - bDiff) * textureColor.g
										//	, currentLoopColor.b + (1 - rDiff) + (1 - gDiff) + (1 - bDiff) * textureColor.b
										//	, 0)* s / 2 * shine;
									}
								}
							}
						}*/
						return finalColor;
					}

				ENDCG

			}

			Pass{
				Tags { "LightMode" = "ForwardAdd" }
				Blend One One
			
				CGPROGRAM
			
				#pragma vertex vert
				#pragma fragment frag
				#include "Lights.cginc"
			
				ENDCG
			}

		}

}
