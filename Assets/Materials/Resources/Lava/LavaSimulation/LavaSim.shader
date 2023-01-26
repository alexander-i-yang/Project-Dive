Shader "CustomRenderTexture/LavaSim"
{
    Properties
    {
        _K("K", Float) = 0.1
        _HalfNeighborhood("Half Neighborhood", Integer) = 2
        // _Impulse("Impulse", Vector) = (0, 0, 0, 0)
        _ImpulseWidth("Impulse Width", Float) = 0.04
    }

        SubShader
    {
       Blend One Zero

       Pass
       {
           Name "LavaSim"

           CGPROGRAM
           #include "UnityCustomRenderTexture.cginc"
           #pragma vertex CustomRenderTextureVertexShader
           #pragma fragment frag
           #pragma target 3.0

            float _K;
            int _HalfNeighborhood;
            float3 _Impulse;
            float _ImpulseWidth;

            //L: Shader inspired by Sean Flanagan's WaterSimulation shader from TOBI.
            //The main difference is that this is a 2D game, so displacement is a function of x instead of x and z. 
            //The texture is 256x1

            float4 getTargetDisplacement(sampler2D tex, float2 uv)
            {
                //Calculates a weighted average of a neighborhood of pixels, where the weight is equal to the distance between the neighbor and the center pixel.
                float sum = 0;
                float totalWeight = 0;

                for (int i = -_HalfNeighborhood; i <= _HalfNeighborhood; i++)
                {
                    float weight = 1 / abs(i);
                    //Only care about pixels on the left and right since it ripples "horizontally"
                    sum += tex2D(tex, float2(uv.x + i / _CustomRenderTextureWidth, uv.y)).r * weight;
                    totalWeight += weight;
                }
                
                //I'm guessing this is so there's no divide by 0? I actually don't know.
                totalWeight += 0.5; 

                return sum / totalWeight;
            }

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                ////Read from double buffered texture.

                float2 uv = IN.localTexcoord.xy;
                float2 c = tex2D(_SelfTexture2D, uv).rg;

                float displacement = c.r;
                float velocity = c.g;

                //Find target displacement based on neighbors

                float target = getTargetDisplacement(_SelfTexture2D, uv);

                ////Apply hooke's law to velocity

                float accel = (target - displacement) * _K;
                velocity += accel;
                velocity *= 0.975;  //Energy Loss
                displacement += velocity;

                //Apply Impulse

                float2 impulseUV = _Impulse.xy;
                float impulseStrength = _Impulse.z;
                float dist = 1;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        dist = min(dist, distance(uv + float2(i, j), frac(impulseUV)));
                    }
                }
                float shouldDisplace = step(dist, _ImpulseWidth);  //dist <= 0.01, displace the pixel, otherwise don't do anything
                velocity += impulseStrength * shouldDisplace;
                //displacement += _Impulse.z;
                
                //Write back to texture

                float r = displacement;
                float g = velocity;
                return float4(r, g, 0, 1);
            }
            ENDCG
        }
    }
}
