﻿Shader "Custom/LootShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Highlight("Highlight", float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float _Highlight;
		
		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			if(_Highlight > 0)
			{
				half4 c = tex2D (_MainTex, IN.uv_MainTex);
 				o.Emission = half3(0.5f, 0.5f, 0.5f);
	            o.Albedo = c.rgb;

			}
			else
			{
				half4 c = tex2D (_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb;
				o.Alpha = c.a;			
			}
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
