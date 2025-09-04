using UnityEditor.AssetImporters;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap, Mesh }
    public DrawMode drawMode;       //i am mkaing my own drawmode that can be changed in the Inspector depending on what I want

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0f, 1f)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public TerrainTypes[] regions;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        float[,] noiseMap = NoiseHandler.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapWidth * mapHeight];    //Array that saves the colours for the map

        //This Nested Loop Mess assigns the colours/mat to the height.
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];

                for (int z = 0; z < regions.Length; z++)     //I'm using z because the regions go by height which is like the z.
                {
                    if (currentHeight > regions[z].height)
                    {
                        colourMap[y * mapWidth + x] = regions[z].colour;
                        break;
                    }
                }
            }
        }

        //Turn into Terrain Mesh
        MapDisplay display = FindAnyObjectByType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));

        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenTerrain.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }


    }

    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }

        if (mapHeight < 1)
        {
            mapHeight = 1;
        }

        if (lacunarity < 1) { lacunarity = 1; }

        if (octaves < 1) { octaves = 1; }

    }
}

[System.Serializable]
public struct TerrainTypes
{
    public string name;
    public float height;
    public Color colour;        //Can Change this to Texture/Material
}

