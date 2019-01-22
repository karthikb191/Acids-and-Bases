// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "My Shaders/2D Lights/VertexLightsShaderModified" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Light Color", Color) = (1, 1, 1, 1)
		
		_LightPosition("Light Position", Vector) = (1, 1, 1, 1)	//This will be set in the c# script
		_LightDirection("Light Direction", Vector) = (0 , 0, 0, 0) //This will also be set inside the script
		
		_PerpendicularRightVector("Perpendicular Right Vector", Vector) = (0, 0, 0, 0)
		_PerpendicularLeftVector("Perpendicular Left Vector", Vector) = (0, 0, 0, 0)
		
		_PerpendiculatRightFogVector("", Vector) = (0, 0, 0, 0)
		_PerpendiculatLeftFogVector("", Vector) = (0, 0, 0, 0)
		
		_ActualRightVector("Right Vector", Vector) = (0, 0, 0, 0)
		_ActualLeftVector("Left Vector", Vector) = (0, 0, 0, 0)
		
		_ActualRightFogVector("", Vector) = (0, 0, 0, 0)
		_ActualLeftFogVector("", Vector) = (0, 0, 0, 0)
		
		_Distance("Distance of light", Range(0, 30)) = 0.5
		_NoFadeDistance("", Range(0, 30)) = 0.5
		
		
		_LightMixtureAdjustmentController("", Range(0, 2)) = 0.4
	}

	SubShader{
		ZWrite Off

		Tags{"Queue" = "Transparent+3" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

		//LOD 200
		ZWrite off
		
		//TODO: Try different blending modes
		Blend One One
		//Blend SrcAlpha OneMinusSrcAlpha

		//GrabPass{}
		
		Pass{
			
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				//sampler2D _GrabTexture;
				
				
				fixed4 _Color;
				fixed3 _LightPosition;
				fixed3 _LightDirection;
				
				fixed3 _PerpendicularLeftVector;
				fixed3 _PerpendicularRightVector;
				
				fixed3 _PerpendiculatLeftFogVector;
				fixed3 _PerpendiculatRightFogVector;
				
				fixed3 _ActualLeftVector;
				fixed3 _ActualRightVector;
				fixed3 _ActualLeftFogVector;
				fixed3 _ActualRightFogVector;

				half _Distance;
				half _NoFadeDistance;

				half _LightMixtureAdjustmentController;

				struct vertexInput{
					fixed4 vertex : POSITION;
					fixed4 worldPos : TEXCOORD2;
				};

				struct vertexOutput{
					fixed4 vertex : POSITION;
					//float3 uvGrab : TEXCOORD1;
					fixed4 worldPos : TEXCOORD2;
					fixed4 color : COLOR;
				};

				vertexOutput vert(vertexInput i){
					vertexOutput o;
					o.vertex = UnityObjectToClipPos(i.vertex);
					//o.uvGrab = ComputeGrabScreenPos(i.vertex);

					fixed4 wp =  mul(unity_ObjectToWorld, i.vertex);
					o.worldPos = wp;

					fixed3 worldPos = fixed3(wp.x, wp.y, _LightPosition.z);
					half currentPixelDistFromLight = distance(_LightPosition, worldPos);

					o.color = fixed4(0, 0, 0, 0);
					if (_Distance > 0.01f && currentPixelDistFromLight < _Distance) {


						fixed4 currentLoopColor = fixed4(1, 1, 1, 1);

						fixed reductionAlpha = smoothstep(_Distance, _NoFadeDistance, currentPixelDistFromLight);


						//Get the direction of the pixel WRT the light position
						fixed3 pixelRelToLight = worldPos - _LightPosition;
						fixed3 pixelRelToLightNormalized = normalize(pixelRelToLight);

						fixed d1 = sign(dot(_PerpendicularRightVector, pixelRelToLight));
						fixed d2 = sign(dot(_PerpendicularLeftVector, pixelRelToLight));

						fixed d3 = dot(_PerpendiculatRightFogVector, pixelRelToLight);
						fixed d4 = dot(_PerpendiculatLeftFogVector, pixelRelToLight);


						if (d1 > 0 && d2 > 0) {
							currentLoopColor = ((/*c +*/ _Color)+_LightMixtureAdjustmentController) * reductionAlpha;

						}
						else {
							currentLoopColor = fixed4(0, 0, 0, 0);
						}

						if (d1 < 0 && d3 > 0) {

							fixed sm = smoothstep(abs(dot(_ActualRightVector, _ActualRightFogVector)), 1,
								abs(dot(pixelRelToLightNormalized, _ActualRightFogVector)));

							//currentLoopColor = float4((/*c +*/ (_Color)+_LightMixtureAdjustmentController) * reductionAlpha * (1 - sm));
							currentLoopColor = (/*c +*/ (_Color)+_LightMixtureAdjustmentController) * reductionAlpha * (1 - sm);

						}

						if (d4 > 0 && d2 < 0) {


							//This method creates a uniform gradient effect
							fixed sm = smoothstep(abs(dot(_ActualLeftVector, _ActualLeftFogVector)), 1,
								abs(dot(pixelRelToLightNormalized, _ActualLeftFogVector)));

							currentLoopColor = (/*c +*/ (_Color)+_LightMixtureAdjustmentController) * reductionAlpha * (1 - sm);

						}
						o.color = currentLoopColor;
					}



					return o;
				}

				fixed4 frag(vertexOutput o) : COLOR{
					//fixed4 c = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(o.uvGrab));

					fixed4 finalColor = fixed4(1, 1, 1, 1) * o.color;

					//fixed3 worldPos = fixed3(o.worldPos.x, o.worldPos.y, _LightPosition.z);
					//half currentPixelDistFromLight = distance(_LightPosition, worldPos);
					//
					//if (_Distance > 0.01f && currentPixelDistFromLight < _Distance) {
					//	
					//
					//	fixed4 currentLoopColor = fixed4(0, 0, 0, 0);
					//	
					//	fixed reductionAlpha = smoothstep(_Distance, _NoFadeDistance, currentPixelDistFromLight);
					//
					//
					//	//Get the direction of the pixel WRT the light position
					//	fixed3 pixelRelToLight = worldPos - _LightPosition;
					//	fixed3 pixelRelToLightNormalized = normalize(pixelRelToLight);
					//
					//	fixed d1 = sign(dot(_PerpendicularRightVector, pixelRelToLight));
					//	fixed d2 = sign(dot(_PerpendicularLeftVector, pixelRelToLight));
					//
					//	fixed d3 = dot(_PerpendiculatRightFogVector, pixelRelToLight);
					//	fixed d4 = dot(_PerpendiculatLeftFogVector, pixelRelToLight);
					//
					//
					//	if (d1 > 0 && d2 > 0) {
					//		currentLoopColor = ((/*c +*/ _Color) + _LightMixtureAdjustmentController) * reductionAlpha;
					//	
					//	}
					//	else {
					//		currentLoopColor = fixed4(0, 0, 0, 0);
					//	}
					//	
					//	if (d1 < 0 && d3 > 0) {
					//
					//		fixed sm = smoothstep(abs(dot(_ActualRightVector, _ActualRightFogVector)), 1,
					//			abs(dot(pixelRelToLightNormalized, _ActualRightFogVector)));
					//
					//		//currentLoopColor = float4((/*c +*/ (_Color)+_LightMixtureAdjustmentController) * reductionAlpha * (1 - sm));
					//		currentLoopColor = (/*c +*/ (_Color)+_LightMixtureAdjustmentController) * reductionAlpha * (1 - sm);
					//		
					//	}
					//
					//	if (d4 > 0 && d2 < 0) {
					//
					//
					//		//This method creates a uniform gradient effect
					//		fixed sm = smoothstep(abs(dot(_ActualLeftVector, _ActualLeftFogVector)), 1,
					//			abs(dot(pixelRelToLightNormalized, _ActualLeftFogVector)));
					//
					//		currentLoopColor = (/*c +*/ (_Color)+_LightMixtureAdjustmentController) * reductionAlpha * (1 - sm);
					//
					//	}
					//	finalColor += currentLoopColor;
					//}
					
					return finalColor;
				}


			ENDCG
		}
		

	}
	
}
