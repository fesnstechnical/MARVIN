Shader "Selected Effect --- Outline/Normal Expansion/Outline Only" {
	Properties {
		_OutlineWidth ("Outline Width", Float) = 0.1
		_OutlineColor ("Outline Color", Color) = (0, 1, 0, 0)
		_OutlineFactor ("Outline Factor", Range(0, 1)) = 1
		[MaterialToggle] _OutlineWriteZ ("Outline Z Write", Float) = 1.0
		_DepthOffset ("Depth Offset", Float) = -8
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Pass {
			Cull Back
			Blend Zero One
			Offset [_DepthOffset], [_DepthOffset]
		}
		Pass {
			Cull Front
			Blend One OneMinusDstColor
			ZWrite [_OutlineWriteZ]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Outline.cginc"
			ENDCG
		}
	} 
	FallBack "VertexLit"
}
