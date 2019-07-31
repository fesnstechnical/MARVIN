Shader "Selected Effect --- Outline/Normal Expansion/Diffuse Transparent" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0
		_OutlineWidth ("Outline Width", Float) = 0.1
		_OutlineColor ("Outline Color", Color) = (0, 1, 0, 1)
		_OutlineFactor ("Outline Factor", Range(0, 1)) = 1
		_Overlay ("Overlay", Float) = 0
		_OverlayColor ("Overlay Color", Color) = (1, 1, 0, 1)
		_Transparent ("Transparent", Range(0, 1)) = 0.5
		[MaterialToggle] _OutlineBasedVertexColorR ("Outline Based Vertex Color R", Float) = 0.0
	}
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		
		// 1st pass --- depth only pass
		Pass {
			ZWrite On
			ColorMask Off
		}
		
		// 2nd pass --- draw transparent stuff
		ZWrite Off
		ColorMask RGBA
		ZTest Equal
		CGPROGRAM
		#pragma surface surf Standard alpha:blend
		sampler2D _MainTex;
		float4 _OverlayColor;
		float _Overlay, _Transparent, _Glossiness, _Metallic;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = lerp(c.rgb, _OverlayColor.rgb, _Overlay);
			o.Alpha = c.a * _Transparent;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
		
		// 3rd pass --- draw outline
		Pass {
			ZWrite On
			ColorMask RGB
			ZTest Less
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Outline.cginc"
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
