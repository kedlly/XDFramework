﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/test" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		SubShader{
		Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }
		LOD 100

		Pass{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog

#include "UnityCG.cginc"

		struct appdata_t
	{
		float4 vertex : POSITION;
		float4 texcoord : TEXCOORD0;

	};

	struct v2f
	{
		float4 vertex : POSITION;
		float4 texcoord : TEXCOORD0;

	};

	sampler2D _MainTex;
	float4 _MainTex_ST;

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = float4(UnityObjectToViewPos( v.vertex ),1) ;
		return o;
	}

		fixed4 frag(v2f i) : SV_Target 
		{
			float4x4 af = unity_ObjectToWorld;
			return fixed4(i.vertex.rgb,1);
		}
		ENDCG
	}
	}

}