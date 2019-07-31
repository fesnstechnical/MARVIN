#ifndef OUTLINE_INCLUDED
#define OUTLINE_INCLUDED

#include "UnityCG.cginc"

float3 _OutlineColor;
float _OutlineWidth, _OutlineFactor, _OutlineBasedVertexColorR;

struct v2f
{
	float4 pos : SV_POSITION;
};
v2f vert (appdata_full v)
{
	float3 dir1 = normalize(v.vertex.xyz);
	float3 dir2 = v.normal;
	float3 dir = lerp(dir1, dir2, _OutlineFactor);
	dir = mul((float3x3)UNITY_MATRIX_IT_MV, dir);
	float2 offset = TransformViewToProjection(dir.xy);
	offset = normalize(offset);
	float dist = distance(mul(unity_ObjectToWorld, v.vertex), _WorldSpaceCameraPos);

	// is outline based on R channel of vertex color ?
	float ow = _OutlineWidth * v.color.r * _OutlineBasedVertexColorR + (1.0 - _OutlineBasedVertexColorR);
	
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
#if UNITY_VERSION > 540
	o.pos.xy += offset * o.pos.z * ow * dist;
#else
	o.pos.xy += offset * o.pos.z * ow / dist;
#endif
	return o;
}
float4 frag (v2f i) : SV_TARGET
{
	return float4(_OutlineColor, 1);
}

#endif