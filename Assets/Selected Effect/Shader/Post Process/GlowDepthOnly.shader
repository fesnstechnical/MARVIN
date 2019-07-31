Shader "Selected Effect --- Outline/Post Process/Depth Only" {
	Properties {
	}
	SubShader {
		Zwrite On
		ColorMask 0
		Pass {}
	}
	FallBack Off
}