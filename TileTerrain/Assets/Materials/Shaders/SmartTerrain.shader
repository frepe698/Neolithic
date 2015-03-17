Shader "Custom/SmartTerrain" {
	Properties {
		_Splat0 ("Splat Map 1", 2D) = "white" {}
	   	_Splat1 ("Splat Map 2", 2D) = "white" {}
		_ColorTexture ("Color", 2D) = "white" {}
		_BumpTexture ("Bump", 2D) = "white" {}
		_GridTexture ("Grid Texture", 2D) = "white" {}
	   [MaterialToggle] _DrawGrid ("Draw Grid", Float) = 0
	   _SnowHeightMap ("Snow Height Map", 2D) = "white" {}
	   _SnowAmount ("Snow Amount", Float) = 0.5
	}
	SubShader {
		Tags {"Queue"="Geometry-2" "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert fullforwardshadows
		#pragma glsl
		

		sampler2D _Splat0;
		sampler2D _Splat1;
		sampler2D _ColorTexture;
		sampler2D _BumpTexture;
		sampler2D _GridTexture;
	    float _DrawGrid;
	    sampler2D _SnowHeightMap;
	    float _SnowAmount;

		struct Input {
			float2 uv_Splat0;
	        float2 uv_ColorTexture;
	        float2 uv_GridTexture;
		};
		#if 0
		void vert (inout appdata_full v) {
			#if !defined(SHADER_API_OPENGL)
				float4 tex = tex2Dlod (_SnowHeightMap, float4(v.texcoord.xy,0,0)/4);
				v.vertex.y += max(tex.r * _SnowAmount*0.5, 0);
			#endif
          //v.vertex.xyz += v.normal * max(_SnowAmount*0.2, 0);
     	}
     	#endif

		void surf (Input i, inout SurfaceOutput o) {
			half4 splatmapMask1 = tex2D (_Splat0, i.uv_Splat0);
            half4 splatmapMask2 = tex2D (_Splat1, i.uv_Splat0);
            
            float2 uv = (frac(i.uv_ColorTexture * float2(4, 4))) * float2(0.25, 0.25);
            float2 uvc = (i.uv_ColorTexture * float2(4, 4)) * float2(0.25, 0.25);
            
            half snowAmount = min(_SnowAmount*(tex2D(_SnowHeightMap, i.uv_ColorTexture).r), 1);
			half amount1 = max(splatmapMask1.r - snowAmount*splatmapMask1.r, 0);
			half amount2 = max(splatmapMask1.g - snowAmount*splatmapMask1.g, 0);
			half amount3 = max(splatmapMask1.b - snowAmount*splatmapMask1.b, 0);
			half amount4 = max(splatmapMask1.a - snowAmount*splatmapMask1.a, 0);
			half amount5 = max(splatmapMask2.r - snowAmount*splatmapMask2.r, 0);
			half amount6 = max(splatmapMask2.g - snowAmount*splatmapMask2.g, 0);
			half amount7 = max(splatmapMask2.b - snowAmount*splatmapMask2.b, 0);
			half amount8 = max(splatmapMask2.a - snowAmount*splatmapMask2.a, 0);
			
			float border = 0.0078125;
			
          	half4 combinedColor = tex2D (_ColorTexture, uv + float2(border, 0.75-border), ddx(uvc + float2(border, 0.75-border)), ddy(uvc + float2(border, 0.75-border))) * amount1;
 
          	combinedColor += tex2D (_ColorTexture, uv + float2(0.375, 0.75-border), ddx(uvc + float2(0.375, 0.75-border)), ddy(uvc + float2(0.375, 0.75-border))) * amount2;
          	 
          	combinedColor += tex2D (_ColorTexture, uv + float2(0.75-border, 0.75-border), ddx(uvc + float2(0.75-border, 0.75-border)), ddy(uvc + float2(0.75-border, 0.75-border))) * amount3;
          	
          	 
          	combinedColor += tex2D (_ColorTexture, uv + float2(border, 0.375), ddx(uvc + float2(border, 0.375)), ddy(uvc + float2(border, 0.375))) * amount4;      	 
          	 
          	 combinedColor += tex2D (_ColorTexture, uv + float2(0.375, 0.375), ddx(uvc + float2(0.375, 0.375)), ddy(uvc + float2(0.375, 0.375))) * amount5;
          	 
          	combinedColor += tex2D (_ColorTexture, uv + float2(0.75-border, 0.375), ddx(uvc + float2(0.75-border, 0.375)), ddy(uvc + float2(0.75-border, 0.375))) * amount6;
          	
          	 
          	combinedColor += tex2D (_ColorTexture, uv + float2(border, border), ddx(uvc + float2(border, border)), ddy(uvc + float2(border, border))) * amount7;
          	 
          	combinedColor += tex2D (_ColorTexture, uv + float2(0.375, border), ddx(uvc + float2(0.375, border)), ddy(uvc + float2(0.375, border))) * amount8;
          	
          	combinedColor += tex2D (_ColorTexture, uv + float2(0.375, 0.75-border), ddx(uvc + float2(0.375, 0.75-border)), ddy(uvc + float2(0.375, 0.75-border))) * snowAmount;
          	

            
            half4 grid = tex2D (_GridTexture, i.uv_GridTexture);
            half dgrid = grid.a * _DrawGrid;
            combinedColor = combinedColor * (1-dgrid);  

            o.Albedo = combinedColor + grid*dgrid;
            half3 combinedNormal;
            combinedNormal = tex2D (_BumpTexture, uv + float2(border, 0.75-border), ddx(uvc + float2(border, 0.75-border)), ddy(uvc + float2(border, 0.75-border))).rgb * amount1;
				
          	combinedNormal += tex2D (_BumpTexture, uv + float2(0.375, 0.75-border), ddx(uvc + float2(0.375, 0.75-border)), ddy(uvc + float2(0.375, 0.75-border))).rgb * amount2;
          	 
          	combinedNormal += tex2D (_BumpTexture, uv + float2(0.75-border, 0.75-border), ddx(uvc + float2(0.75-border, 0.75-border)), ddy(uvc + float2(0.75-border, 0.75-border))).rgb * amount3;
          	
          	 
          	combinedNormal += tex2D (_BumpTexture, uv + float2(border, 0.375), ddx(uvc + float2(border, 0.375)), ddy(uvc + float2(border, 0.375))).rgb * amount4;
          	 
          	combinedNormal += tex2D (_BumpTexture, uv + float2(0.375, 0.375), ddx(uvc + float2(0.375, 0.375)), ddy(uvc + float2(0.375, 0.375))).rgb * amount5;
          	 
          	combinedNormal += tex2D (_BumpTexture, uv + float2(0.75-border, 0.375), ddx(uvc + float2(0.75-border, 0.375)), ddy(uvc + float2(0.75-border, 0.375))).rgb * amount6;
          	
          	 
          	combinedNormal += tex2D (_BumpTexture, uv + float2(border, border), ddx(uvc + float2(border, border)), ddy(uvc + float2(border, border))).rgb * amount7;
          	 
          	combinedNormal += tex2D (_BumpTexture, uv + float2(0.375, border), ddx(uvc + float2(0.375, border)), ddy(uvc + float2(0.375, border))).rgb * amount8;
          	
          	combinedNormal += tex2D (_BumpTexture, uv + float2(0.375, 0.75-border), ddx(uvc + float2(0.375, 0.75-border)), ddy(uvc + float2(0.375, 0.75-border))).rgb * snowAmount;
//			if(snowAmount < 1)
//			{
//				combinedNormal = tex2D (_BumpTexture, uv + float2(border, 0.75-border)).rgb * splatmapMask1.r;
//				
//	          	combinedNormal += tex2D (_BumpTexture, uv + float2(0.375, 0.75-border)).rgb * splatmapMask1.g;
//	          	 
//	          	combinedNormal += tex2D (_BumpTexture, uv + float2(0.75-border, 0.75-border)).rgb * splatmapMask1.b;
//	          	
//	          	 
//	          	combinedNormal += tex2D (_BumpTexture, uv + float2(border, 0.375)).rgb * splatmapMask1.a;
//	          	 
//	          	combinedNormal += tex2D (_BumpTexture, uv + float2(0.375, 0.375)).rgb * splatmapMask2.r;
//	          	 
//	          	combinedNormal += tex2D (_BumpTexture, uv + float2(0.75-border, 0.375)).rgb * splatmapMask2.g;
//	          	
//	          	 
//	          	combinedNormal += tex2D (_BumpTexture, uv + float2(border, border)).rgb * splatmapMask2.b;
//	          	 
//	          	combinedNormal += tex2D (_BumpTexture, uv + float2(0.375, border)).rgb * splatmapMask2.a;
//			}
//			else
//			{
//				combinedNormal = UnpackNormal(tex2D (_BumpTexture, uv + float2(0.375, 0.75-border)));//.rgb * 1;
//			}
          	
          	
          	 
          	o.Normal = combinedNormal;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
