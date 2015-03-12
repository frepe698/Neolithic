Shader "Legacy Shaders/Outlined/Outlined Bumped" {
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_OutlineColor ("Outline Color", Color) = (1,0,0,1)
		_Highlight ("Highlight", Float) = 0
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_BumpMap ("Bumpmap", 2D) = "bump" {}
	}
 
	CGINCLUDE
	#include "UnityCG.cginc"
 
	struct appdata 
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
 
	struct v2f 
	{
		float4 pos : POSITION;
		float4 color : COLOR;
	};

	struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};
 
	uniform float _Outline;
	uniform float _Highlight;
	uniform float4 _OutlineColor;
 
	v2f vert(appdata v) 
	{
		// just make a copy of incoming vertex data but scaled according to normal direction
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
 
		float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		float2 offset = TransformViewToProjection(norm.xy);
 
		o.pos.xy += offset * o.pos.z * 0.005 * _Highlight;
		o.color = _OutlineColor;
		o.color.a = 0.3;
		return o;
	}
	ENDCG
 
	SubShader 
	{
		Tags { "Queue" = "Transparent" }
		//Fog {Fog Block}
		// note that a vertex shader is specified here but its using the one above
		Pass 
		{
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Off
			ZWrite Off
			ZTest LEqual

 
			// you can choose what kind of blending mode you want for the outline
			//Blend SrcAlpha OneMinusSrcAlpha // Normal
			//Blend One One // Additive
			//Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 
			half4 frag(v2f i) : COLOR 
			{
				return i.color;
			}
			ENDCG
		}
 

		Tags {"LigthMode" = "ForwardBase" "RenderType"="Opaque" }
		Cull Back
		ZWrite On
		

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _BumpMap;
		uniform float4 _Color;

		void surf(Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
 
	}
 
	SubShader 
	{
		Tags { "Queue" = "Transparent" }
 
		Pass 
		{
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Front
			ZWrite Off
			ZTest Always
			Offset 15,15
 
			// you can choose what kind of blending mode you want for the outline
			//Blend SrcAlpha OneMinusSrcAlpha // Normal
			//Blend One One // Additive
			//Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative
 
			CGPROGRAM
			#pragma vertex vert
			#pragma exclude_renderers gles xbox360 ps3
			ENDCG
			SetTexture [_MainTex] { combine primary }
		}
 

		Tags {"Queue" = "Geometry" "LigthMode" = "ForwardBase" "RenderType"="Opaque" }
		Cull Back
		ZWrite On
		//Fog{Fog Block}

		CGPROGRAM
		#pragma surface surf Lambert

			

		sampler2D _MainTex;
		sampler2D _BumpMap;
		uniform float4 _Color;

		void surf(Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	}
 
	Fallback "Legacy Shaders/Bumped Diffuse"
}