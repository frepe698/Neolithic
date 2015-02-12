Shader "Custom/TerrainSplat8" {
	Properties {
	   _Splat0 ("Splat Map 1", 2D) = "white" {}
	   _Splat1 ("Splat Map 2", 2D) = "white" {}
	   _Texture1 ("Texture 1", 2D) = "white" {}
	   _Texture2 ("Texture 2", 2D) = "white" {}
	   _Texture3 ("Texture 3", 2D) = "white" {}
	   _Texture4 ("Texture 4", 2D) = "white" {}
	   _Texture5 ("Texture 5", 2D) = "white" {}
	   _Texture6 ("Texture 6", 2D) = "white" {}
	   _Texture7 ("Texture 7", 2D) = "white" {}
	   _Texture8 ("Texture 8", 2D) = "white" {}
	   _Bump1 ("Bump 1", 2D) = "white" {}
	   _Bump2 ("Bump 2", 2D) = "white" {}
	   _Bump3 ("Bump 3", 2D) = "white" {}
	   _Bump4 ("Bump 4", 2D) = "white" {}
	   _GridTexture ("Grid Texture", 2D) = "white" {}
	   [MaterialToggle] _DrawGrid ("Draw Grid", Float) = 0
	}

	SubShader {
	    Tags {"RenderType" = "Opaque"}

 		CGPROGRAM
	    #pragma target 3.0
	    #pragma glsl
	    #pragma surface surf Lambert

	    struct Input {
	            float2 uv_Splat0;
	            float2 uv_Texture1;
	            float2 uv_GridTexture;

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
	    sampler2D _Texture7;
	    sampler2D _Texture8;
	   	sampler2D _Bump1;
	    sampler2D _Bump2;
	    sampler2D _Bump3;
	    sampler2D _Bump4;   
	    sampler2D _GridTexture;
	    float _DrawGrid;
	    

	    void surf (Input i, inout SurfaceOutput o) {
       
            half4 splatmapMask1 = tex2D (_Splat0, i.uv_Splat0);
            half4 splatmapMask2 = tex2D (_Splat1, i.uv_Splat0);
          	half4 combinedColor = tex2D (_Texture1, i.uv_Texture1) * splatmapMask1.r;
            combinedColor += tex2D (_Texture2, i.uv_Texture1) * splatmapMask1.g;
            combinedColor += tex2D (_Texture3, i.uv_Texture1) * splatmapMask1.b;
            combinedColor += tex2D (_Texture4, i.uv_Texture1) * splatmapMask1.a;
            combinedColor += tex2D (_Texture5, i.uv_Texture1) * splatmapMask2.r;
            combinedColor += tex2D (_Texture6, i.uv_Texture1) * splatmapMask2.g;
            combinedColor += tex2D (_Texture7, i.uv_Texture1) * splatmapMask2.b;
            combinedColor += tex2D (_Texture8, i.uv_Texture1) * splatmapMask2.a;
            
            half4 grid = tex2D (_GridTexture, i.uv_GridTexture);
            half dgrid = grid.a * _DrawGrid;
            combinedColor = combinedColor * (1-dgrid);  

            o.Albedo = combinedColor + grid*dgrid;
            
           	half3 combinedNormal = tex2D(_Bump1, i.uv_Texture1).rgb * splatmapMask1.r;
			combinedNormal += tex2D(_Bump3, i.uv_Texture1).rgb * splatmapMask1.g;
			combinedNormal += tex2D(_Bump3, i.uv_Texture1).rgb * splatmapMask1.b;
			combinedNormal += tex2D(_Bump4, i.uv_Texture1).rgb * splatmapMask1.a;
			combinedNormal += tex2D(_Bump2, i.uv_Texture1).rgb * splatmapMask2.r;
			combinedNormal += tex2D(_Bump3, i.uv_Texture1).rgb * splatmapMask2.g;
			combinedNormal += tex2D(_Bump3, i.uv_Texture1).rgb * splatmapMask2.b;
			combinedNormal += tex2D(_Bump3, i.uv_Texture1).rgb * splatmapMask2.a;
            o.Normal = combinedNormal;
            
	    }
	    ENDCG
//	    CGPROGRAM
//	    #pragma target 3.0
//	    #pragma glsl
//	    #pragma surface surf Lambert
//
//	    struct Input {
//	            float2 uv_Splat0;
//	            float2 uv_Texture1;
//	            float2 uv_GridTexture;
//
//	           // INTERNAL_DATA
//	    };
//
//	    sampler2D _Splat0;
//	    sampler2D _Splat1;
//	    sampler2D _Texture1;
//	    sampler2D _Texture2;
//	    sampler2D _Texture3;
//	    sampler2D _Texture4;
//	    sampler2D _Texture5;
//	    sampler2D _Texture6;
//	    sampler2D _Texture7;
//	    sampler2D _Texture8;
//	    sampler2D _GridTexture;
//	    float _DrawGrid;
//	    
//
//	    void surf (Input i, inout SurfaceOutput o) {
//       
//            half4 splatmapMask1 = tex2D (_Splat0, i.uv_Splat0);
//            half4 splatmapMask2 = tex2D (_Splat1, i.uv_Splat0);
//          	half4 combinedColor = tex2D (_Texture1, i.uv_Texture1) * splatmapMask1.r;
//            combinedColor += tex2D (_Texture2, i.uv_Texture1) * splatmapMask1.g;
//            combinedColor += tex2D (_Texture3, i.uv_Texture1) * splatmapMask1.b;
//            combinedColor += tex2D (_Texture4, i.uv_Texture1) * splatmapMask1.a;
//            combinedColor += tex2D (_Texture5, i.uv_Texture1) * splatmapMask2.r;
//            combinedColor += tex2D (_Texture6, i.uv_Texture1) * splatmapMask2.g;
//            combinedColor += tex2D (_Texture7, i.uv_Texture1) * splatmapMask2.b;
//            combinedColor += tex2D (_Texture8, i.uv_Texture1) * splatmapMask2.a;
//            
//            half4 grid = tex2D (_GridTexture, i.uv_GridTexture);
//            half dgrid = grid.a * _DrawGrid;
//            combinedColor = combinedColor * (1-dgrid);  
//
//            o.Albedo = combinedColor + grid*dgrid;
//            
//	    }
//	    ENDCG
//	    
//	    CGPROGRAM
//	    #pragma target 3.0
//	    #pragma glsl
//	    #pragma surface surf Lambert
//
//	    struct Input {
//	            float2 uv_Splat0;
//	            float2 uv_Texture1;
//	            float2 uv_GridTexture;
//
//	           // INTERNAL_DATA
//	    };
//
//	    sampler2D _Splat0;
//	    sampler2D _Splat1;
//	    sampler2D _Bump1;
//	    sampler2D _Bump2;
//	    sampler2D _Bump3;
//	    sampler2D _Bump4;    
//
//	    void surf (Input i, inout SurfaceOutput o) {
//       		o.Albedo = tex2D(_Splat0, i.uv_Splat0);
//            half4 splatmapMask1 = tex2D (_Splat0, i.uv_Splat0);
//            half4 splatmapMask2 = tex2D (_Splat1, i.uv_Splat0);
//			half3 combinedNormal = tex2D(_Bump1, i.uv_Texture1).rgb * splatmapMask1.r;
//			combinedNormal += tex2D(_Bump3, i.uv_Texture1).rgb * splatmapMask1.g;
//			combinedNormal += tex2D(_Bump3, i.uv_Texture1).rgb * splatmapMask1.b;
//			combinedNormal += tex2D(_Bump4, i.uv_Texture1).rgb * splatmapMask1.a;
//			combinedNormal += tex2D(_Bump2, i.uv_Texture1).rgb * splatmapMask2.r;
//			combinedNormal += tex2D(_Bump3, i.uv_Texture1).rgb * splatmapMask2.g;
//			combinedNormal += tex2D(_Bump3, i.uv_Texture1).rgb * splatmapMask2.b;
//			combinedNormal += tex2D(_Bump3, i.uv_Texture1).rgb * splatmapMask2.a;
//            o.Normal = combinedNormal;
//            
//	    }
//	    ENDCG
	}
	
	FallBack "Diffuse"
}
