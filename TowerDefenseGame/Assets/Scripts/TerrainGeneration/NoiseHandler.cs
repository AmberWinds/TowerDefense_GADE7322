using UnityEngine;

public class NoiseHandler : MonoBehaviour
{
    public static float[,] GenerateNoiseMap(int mapwidth, int mapheight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noisemap = new float[mapwidth, mapheight];


        //Creates a new seed so that each generation is random using offsets
        System.Random newSeed = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offSetX = newSeed.Next(-100000, 100000) + offset.x;
            float offSetY = newSeed.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offSetX, offSetY);

        }


        if (scale <= 0)
        {
            scale = 0.001f;
        }
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;



        for (int y = 0; y < mapheight; y++)
        {
            for (int x = 0; x < mapwidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseheight = 25;

                for (int i = 0; i < octaves; i++) // This is for the Octaves
                {
                    float sampleX = x / scale * frequency + octaveOffsets[i].x;
                    float sampleY = y / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseheight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;

                }

                if (noiseheight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseheight;
                }
                else if (noiseheight < minNoiseHeight)
                {
                    minNoiseHeight = noiseheight;
                }

                noisemap[x, y] = noiseheight;
            }
        }
        //Normalize noisemap

        for (int y = 0; y < mapheight; y++)
        {
            for (int x = 0; x < mapwidth; x++)
            {
                noisemap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noisemap[x, y]);
            }

        }
        return noisemap;
    }
}
