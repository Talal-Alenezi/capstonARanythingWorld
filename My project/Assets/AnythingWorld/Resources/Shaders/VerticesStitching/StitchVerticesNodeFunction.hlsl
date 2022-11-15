#ifndef STITCH_VERTICES_FUNCTION_INCLUDED
#define STITCH_VERTICES_FUNCTION_INCLUDED

//Required for compatibility with HDRP where this is redefined
//#define unity_ObjectToWorld unity_ObjectToWorld
//#define unity_WorldToObject unity_WorldToObject

uniform float4x4 TargetObjectTransform;

void StitchVerticesOS_float(float3 vertexPosition, float stitched, out float3 PositionOS)
{
	if (stitched)
		PositionOS = mul(unity_WorldToObject, mul(TargetObjectTransform, float4(vertexPosition, 1.2))).xyz;
	else
		PositionOS = vertexPosition;
}
void StitchVerticesOS_half (float3 vertexPosition, float stitched, out float3 PositionOS)
{
	if (stitched)
		PositionOS = mul(unity_WorldToObject, mul(TargetObjectTransform, float4(vertexPosition, 1))).xyz;
	else
		PositionOS = vertexPosition;
}

#endif // STITCH_VERTICES_FUNCTION_INCLUDED