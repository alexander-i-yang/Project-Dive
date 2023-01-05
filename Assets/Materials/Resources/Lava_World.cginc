#ifndef LAVA_WORLD
#define LAVA_WORLD

void AddNoise_float(float input, float noise, float noiseScale, float noiseBlend, out float output)
{
	float wetInput = input + (noise - 0.5) * noiseScale;
	output = lerp(input, wetInput, noiseBlend);
}

void yPlusSinX_float(float x, float y, float frequency, float amplitude, float phase, out float output)
{
	const float tau = 6.28318530718f;
	output = y + amplitude * sin(tau * frequency*x + phase);
}

#endif