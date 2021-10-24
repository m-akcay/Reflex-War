const float MAX_DIST_TO_CENTER = 1.73f / 2;

void calculateColor_float(in float distToCenter, in float3 c0, in float3 c1, out float3 color, out float smoothness)
{
	float t;
	if (distToCenter > 0.65f)
	{
		t = 1 - smoothstep(0.65f, 0.7f, distToCenter);
		color = lerp(c0, c1, t);
		smoothness = lerp(0.5f, 0.98f, t);
	}
	else
	{
		color = c1;
		smoothness = 0.98f;
	}
}