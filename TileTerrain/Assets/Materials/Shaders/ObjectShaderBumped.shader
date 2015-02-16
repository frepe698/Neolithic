Shader "Custom/ObjectShaderBumped" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Highlight("Highlight", float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _BumpMap;
		float _Highlight;
		
		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			if(_Highlight > 0)
			{
				half4 c = tex2D (_MainTex, IN.uv_MainTex);
 				o.Emission = half3(0.05f, 0.05f, 0);
	            o.Albedo = c.rgb;

			}
			else
			{
				half4 c = tex2D (_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb;		
			}
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
