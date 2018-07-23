// create by JiepengTan 
// https://github.com/JiepengTan/FishManShaderTutorial
// date: 2018-03-27  
// email: jiepengtan@gmail.com
Shader "FishManShaderTutorial/BaseMath"{
	Properties{
		_MainTex("MainTex", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			//Cull off

			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#define USING_PERLIN_NOISE 1
#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 old : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.old = v.vertex;
				return o;
			}
			float3 ProcessFrag(float2 uv);
			float4 frag(v2f i) : SV_Target{
				return float4(ProcessFrag(i.uv),1.0);
			}

#define DrawInGrid(uv,DRAW_FUNC)\
				{\
					float2 pFloor = floor(uv);\
					if(length(pFloor-float2(j,i))<0.1){\
						col = DRAW_FUNC(frac(uv)-0.5);\
					}\
					num = num + 1.000;\
					i=floor(num / gridSize); j=fmod(num,gridSize);\
				}\

			//绘制格子线
			float3 DrawGridLine(float2 uv) {
				float2 _uv = uv - floor(uv);
				float val = 0.;
				const float eps = 0.01;
				if (_uv.x<eps || _uv.y<eps) {
					val = 1.;
				}
				return float3(val,val,val);
			}

			float3 DrawSmoothstep(float2 uv) {
				uv += 0.5;
				float val = smoothstep(0.0, 1.0, uv.x);
				val = step(abs(val - uv.y), 0.01);
				return float3(val, val, val);
			}

			float3 DrawCircle(float2 uv) {
				float val = clamp((1.0 - length(uv) * 2), 0., 1.);
				//val = step(length(uv), 0.5);
				return float3(val, val, val);
			}
			float3 DrawWeakCircle(float2 uv) {
				float val = clamp((1.0 - length(uv) * 2), 0., 1.);
				val = pow(val, 2);
				return float3(val, val, val);
			}
			float3 DrawFlower(float2 uv) {
				float deg = atan2(uv.y, uv.x);// +_Time.y * -0.1;
				float len = length(uv)*3;
				float offs = abs(sin(deg*3.));// *0.35;
				return float3(len, len, len);
				return smoothstep(1. + offs, 1. + offs - 0.05, len);
				return smoothstep(1. + offs - 0.01, 1. + offs, len);
			}

			float3 ProcessFrag(float2 uv) {
				float3 col = float3(0.0,0.0,0.0);
				float num = 0.;
				float gridSize = 3.;
				float i = 0.,j = 0.;
				uv *= gridSize;

				DrawInGrid(uv, DrawSmoothstep);
				DrawInGrid(uv, DrawCircle);
				DrawInGrid(uv, DrawWeakCircle);
				DrawInGrid(uv, DrawFlower);

				col += DrawGridLine(uv);
				return col;
			}
		ENDCG
		}//end pass
	}//end SubShader
}//end Shader

