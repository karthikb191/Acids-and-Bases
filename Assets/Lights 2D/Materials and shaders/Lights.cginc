#if !defined(MY_2D_Lights)
#define MY_2D_Lights

#include "UnityCG.cginc"

#endif

fixed4 _LightColors[4];
fixed4 _LightPositions[4];
fixed3 _LightDirections[4];

half _LightDistances[4];
half _NoFadeDistances[4];
half _LightMixtureAdjustmentControllers[4];

fixed _LightAngles[4];
int _IsAPixelLight[4];	//the flag to check if the light is a pixel light
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
						
    float4 finalColor = textureColor * o.color;

    return finalColor;
}
