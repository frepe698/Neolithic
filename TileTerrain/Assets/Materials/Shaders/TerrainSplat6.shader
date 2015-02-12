Shader "Custom/TerrainSplat6" {
	Properties {
	   _Splat0 ("Splat Map 1", 2D) = "white" {}
	   _Splat1 ("Splat Map 2", 2D) = "white" {}
	   _Texture1 ("Texture 1", 2D) = "white" {}
	   _Texture2 ("Texture 2", 2D) = "white" {}
	   _Texture3 ("Texture 3", 2D) = "white" {}
	   _Texture4 ("Texture 4", 2D) = "white" {}
	   _Texture5 ("Texture 5", 2D) = "white" {}
	   _Texture6 ("Texture 6", 2D) = "white" {}
	}

	SubShader {
	    Tags {"RenderType" = "Opaque"}

	    CGPROGRAM
	    #pragma target 3.0
	    #pragma surface surf Lambert
	    

	    struct Input {
	            float2 uv_Splat0;
	            //float2 uv_Splat1;
	            float2 uv_Texture1;
//	            float2 uv_Texture2;
//	            float2 uv_Texture3;
//	            float2 uv_Texture4;
//	            float2 uv_Texture5;
//	            float2 uv_Texture6;
	           // INTERNAL_DATA
	    };

	    sampler2D _Splat0;
	    sampler2D _Splat1;
	    sampler2D _Texture1;
	    sampler2D _Texture2;
	    sampler2D _Texture3;
	    sampler2D _Texture4;
	    sampler2D _Texture5;
	    sampler2D _Texture6;
	    

	    void surf (Input i, inout SurfaceOutput o) {
       
            half4 splatmapMask1 = tex2D (_Splat0, i.uv_Splat0);
            half4 splatmapMask2 = tex2D (_Splat1, i.uv_Splat0);
          	half4 combinedColor = tex2D (_Texture1, i.uv_Texture1) * splatmapMask1.r;
            combinedColor += tex2D (_Texture2, i.uv_Texture1) * splatmapMask1.g;
            combinedColor += tex2D (_Texture3, i.uv_Texture1) * splatmapMask1.b;
            combinedColor += tex2D (_Texture4, i.uv_Texture1) * splatmapMask2.r;
            combinedColor += tex2D (_Texture5, i.uv_Texture1) * splatmapMask2.b;
            combinedColor += tex2D (_Texture6, i.uv_Texture1) * splatmapMask2.g;

            o.Albedo = combinedColor;
	    }
	    ENDCG
	}
	
	FallBack "Diffuse"
}
