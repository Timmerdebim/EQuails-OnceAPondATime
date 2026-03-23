Shader "Hidden/JFA_Step"
{
    Properties
    {
        _SeedTex("Seed Texture", 2D) = "black" {}
        _StepSize("Step Size", Float) = 1.0
    }
    SubShader
    {
        ZWrite Off ZTest Always Cull Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            TEXTURE2D(_SeedTex);
            SAMPLER(sampler_SeedTex);

            float4 _ScreenParams;
            float _StepSize;
            
            struct Varyings { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };
            
            
            bool is_colored(float2 uv)
            {
            	// Uncolored pixels have UV (-1, -1), so we can check if the UV is valid by checking if it's greater than or equal to 0
                return uv.x >= 0 && uv.y >= 0; 
            }
            
            float2 JFA(float2 P_UV)
            {
                float2 resolution = _ScreenParams.xy;
				float2 P_pos = P_UV * resolution;

				float2 seed_UV = LOAD_TEXTURE2D(_SeedTex, P_pos).rg;
            	
            	float2 output_UV = float2(-1, -1);
            	
                for (int x = -1; x <= 1; x++)
				{
					for (int y = -1; y <= 1; y++)
					{
						if (x == 0 && y == 0) continue; // Skip the current pixel
						
						float2 pixel_offset = float2(x, y) * _StepSize / resolution; 
                        
						float2 Q_UV = LOAD_TEXTURE2D(_SeedTex, P_pos + pixel_offset).rg;
						float2 Q_pos = Q_UV * resolution;

						if (is_colored(Q_UV)) 
						{
							if(is_colored(P_UV))
                            {
								float2 seed_pos = seed_UV * resolution; 

								float P_dist = distance(P_pos, seed_pos);
								float Q_dist = distance(Q_pos, seed_pos);

								if (Q_dist < P_dist)
								{
									output_UV = Q_UV; 
								}
							}
							else
							{
								output_UV = Q_UV;
                            }
						}
					}
                }
            	
            	return output_UV;
            }
            
            Varyings vert(uint id : SV_VertexID)
            {
                Varyings o;
                o.pos = GetFullScreenTriangleVertexPosition(id);
                o.uv  = GetFullScreenTriangleTexCoord(id);
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                // The JFA step is to sample the seed texture at the current pixel and return the UV of the seed if it exists, otherwise return (-1, -1)
                // We store the UV of the seed in the red and green channels of the seed texture, so we can sample it directly
                // Call the current point "P" and the neighbour point "Q".

                // float2 P_UV = SAMPLE_TEXTURE2D(_SeedTex, sampler_SeedTex, i.uv).rg; 
                float2 P_UV = JFA(i.uv);
            	return float4(P_UV, 0, 0);
            }


            ENDHLSL
        }
    }
}
