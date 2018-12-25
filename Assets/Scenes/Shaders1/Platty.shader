
//
// Shader: "FX/Force Field"
// Version: v1.0
// Written by: Thomas Phillips
//
// Anyone is free to use this shader for non-commercial or commercial projects.
//
// Description:
// Generic force field effect.
// Play with color, opacity, and rate for different effects.
//

Shader "FX/Force Field" {

	Properties{
		_Color("Color Tint", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_Contact("Contact", Range(0, 10)) = 1
	}

	SubShader{

		ZWrite Off
		Tags{ "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		struct input {
			float2 uv : TEXCOORD0;
			float4 pos : POSITION;
		};
		struct output {
			float2 uv : TEXCOORD0;
			float4 color : COLOR;
			float4 pos : SV_POSITION;
		};

		sampler2D _MainTex;
		fixed4 _Color;
		float _Contact;
		float _Contact1;
		float _Contact2;
		output vert(input i){
			output o;

			o.pos = UnityObjectToClipPos(i.pos);
			o.uv = i.uv;
			_Contact += i.pos.x * _Contact;
			_Contact1 = i.pos.y;
			_Contact2 = i.pos.z;
			
			//o.color = _Color * half4(_SinTime.z, _SinTime.z, _SinTime.z, _SinTime.y);
			o.color = half4(i.pos.x, i.pos.y, i.pos.z, _Contact+_Contact1+_Contact1);
			//o.pos += half4(_SinTime.z, 0, 0, _SinTime.y);

			return o;
		}

		half4 frag(output o) : SV_TARGET{
			half4 result;// = tex2D(_MainTex, o.uv);
			result = o.color;
			return result;
		}
		
		ENDCG
		}
	}
	Fallback "Transparent/Diffuse"
}