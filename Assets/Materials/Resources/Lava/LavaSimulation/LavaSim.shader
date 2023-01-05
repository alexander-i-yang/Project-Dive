Shader "CustomRenderTexture/LavaSim"
{
    Properties
    {
        _K("K", Float) = 0.1
        _HalfNeighborhood("Half Neighborhood", Integer) = 2
        _Impulse ("Impulse", Vector) = (0, 0, 0, 0)
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

            float2 _Impulse;
            float _K;
            int _HalfNeighborhood;

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
                    sum += tex2D(tex, float2(uv.x + i / 256, 0)).r * weight;
                    totalWeight += weight;
                }
                
                //I'm guessing this is so there's no didvide by 0? I actually don't know.
                totalWeight += 0.5; 

                return sum / totalWeight;
            }

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                //Read from double buffered texture.

                float2 uv = IN.localTexcoord.xy;
                float2 c = tex2D(_SelfTexture2D, uv).rg;

                float displacement = c.r;
                float velocity = c.g;

                //Find target displacement based on neighbors

                float target = getTargetDisplacement(_SelfTexture2D, uv);

                //Apply hooke's law to velocity

                float accel = (target - displacement) * _K;
                velocity += accel;
                velocity *= 0.975;  //Energy Loss
                displacement += velocity;

                //Apply Impulse

                float dist = 1;
                for (int i = -1; i <= 1; i++)
                {
                    dist = min(dist, abs(uv.x + i - frac(_Impulse.x)));
                }
                float shouldDisplace = step(dist, 0.01);  //dist <= 0.01, displace the pixel, otherwise don't do anything
                displacement += _Impulse.y * shouldDisplace;

                //Write back to texture

                float r = displacement;
                float g = velocity;
                return float4(r, g, 0, 0);
            }
            ENDCG
        }
    }
}
