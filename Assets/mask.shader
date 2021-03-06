﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UnlitStencilMaskVF" {
	SubShader{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry-1" }



		CGINCLUDE
		struct appdata
	{
		float4 vertex : POSITION;
	};
	struct v2f
	{
		float4 pos : SV_POSITION;
	};
	v2f vert(appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}
	half4 frag(v2f i) : SV_Target{
		return half4(1,1,0,1);
	}
		ENDCG

		Pass
	{

		ColorMask RGBA
			//ZWrite Off

			Stencil
		{
			Ref 2
			Comp Always
			Pass Replace
		}
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
			ENDCG
	}


	}
}