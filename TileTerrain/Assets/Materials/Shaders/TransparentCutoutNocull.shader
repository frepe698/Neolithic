Shader "Legacy Shaders/Transparent/Cutout/DiffuseNoCull" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_Highlight("Highlight", float) = 0
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 200
	Cull Back
	
	CGPROGRAM
	#pragma surface surf Lambert alphatest:_Cutoff

	sampler2D _MainTex;
	fixed4 _Color;
	float _Highlight;

	struct Input {
		float2 uv_MainTex;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		if(_Highlight > 0)
		{
            o.Emission = half3(0.05f, 0.05f, 0);
		}
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
		
	ENDCG
	}

	Fallback "Legacy Shaders/Transparent/Cutout/Diffuse"
}
