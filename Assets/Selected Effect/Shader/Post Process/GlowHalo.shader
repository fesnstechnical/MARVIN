Shader "Selected Effect --- Outline/Post Process/Halo" {
	Properties {
		_MainTex ("Main", 2D) = "black" {}
		_GlowObjectTex ("Glow Object", 2D) = "black" {}
		_GlowColor ("Glow Color", Color) = (0, 1, 1, 1)
		_GlowIntensity ("Glow Intensity", Float) = 3
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _MainTex, _GlowObjectTex;
	float4 _GlowColor;
	float _GlowIntensity;
	float4 frag (v2f_img i) : SV_Target 
	{
		float r = tex2D(_GlowObjectTex, i.uv).r;
		clip(-r);
		return tex2D(_MainTex, i.uv).r * _GlowColor * _GlowIntensity;
	}
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	ENDCG
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			ENDCG
		}
    }
}