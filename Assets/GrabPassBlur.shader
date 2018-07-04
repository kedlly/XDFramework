﻿Shader "Custom/GrabPassBlur"
{

	Properties
	{
		//_BumpMap 和 _BumpAmt控制扭曲
		//_TintColor 和 _TintAmt控制叠加颜色
		//_BlurAmt 控制模糊程度
		_BumpAmt("Distortion", range(0, 2)) = 1
		_TintAmt("Tint Amount", Range(0,1)) = 0.1
		_TintColor("Tint Color", Color) = (1, 1, 1, 1)
		_MainTex("Tint Texture (RGB)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_BlurAmt("Blur", Range(0, 10)) = 1
	}

		SubShader
		{
			//Queue is Transparent so other objects will be rendered first
			Tags{ "RenderType" = "Opaque" "Queue" = "Transparent" }
			LOD 100

			GrabPass{}

			Pass
			{
				CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

				struct appData
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					float4 uvgrab : TEXCOORD1;
				};

				float _BumpAmt;
				float _TintAmt;
				float _BlurAmt;
				float4 _TintColor;
				sampler2D _MainTex;
				sampler2D _BumpMap;
				sampler2D _GrabTexture;
				float4 _GrabTexture_TexelSize;

				//https://en.wikipedia.org/wiki/Gaussian_blur
				float blurWeight[49];

				half4 blur(half4 col, sampler2D tex, float4 uvrgab)
				{
					float2 offset = 1.0 / _ScreenParams.xy;
					for (int i = -3; i <= 3; ++i)
					{
						for (int j = -3; j <= 3; ++j)
						{
							//col += tex2Dproj(tex, uvrgab + float4(_GrabTexture_TexelSize.x * i * _BlurAmt, _GrabTexture_TexelSize.y *j * _BlurAmt, 0.0f, 0.0f)) * blurWeight[j * 7 + i + 24];
							col += tex2Dproj(tex, uvrgab + float4(offset.x * i * _BlurAmt, offset.y * j * _BlurAmt, 0.0f, 0.0f)) * blurWeight[j * 7 + i + 24];
						}
					}
					return col;
				}

				v2f vert(appData v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = v.texcoord;
					o.uvgrab = ComputeGrabScreenPos(o.vertex);
					return o;
				}

				half4 frag(v2f i) : COLOR
				{
					half4 mainColor = tex2D(_MainTex, i.texcoord);
					half2 distortion = UnpackNormal(tex2D(_BumpMap, i.texcoord)).rg * _BumpAmt;
					half4 col = half4(0, 0, 0, 0);
					float4 uvgrab = float4(i.uvgrab.x + distortion.x, i.uvgrab.y + distortion.y, i.uvgrab.z, i.uvgrab.w);
					col = blur(col, _GrabTexture, uvgrab);
					return lerp(col, col * mainColor, _TintAmt) * _TintColor;
				}

			ENDCG
		}
	}
}