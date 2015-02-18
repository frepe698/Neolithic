Shader "Custom/LootShaderSpecular" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_Highlight("Highlight", float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		
		CGPROGRAM
		#pragma surface surf BlinnPhong

		sampler2D _MainTex;
		half _Shininess;
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
				o.Gloss = c.a;
				o.Specular = _Shininess;

			}
			else
			{
				half4 c = tex2D (_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb;		
				o.Gloss = c.a;
				o.Specular = _Shininess;
			}
			
		}
		ENDCG
	} 
	FallBack "Custom/LootShader"
}
