using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenTerrain
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve curve)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        //make the mesh the centre of unity map thing
        float topLeftX = (width - 1) / -2;
        float topLeftZ = (height - 1) / 2;

        MeshData meshData = new MeshData(width, height);
        int vertixIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //Generate Vertices
                meshData.vertices[vertixIndex] = new Vector3(topLeftX + x, curve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);    //(x, y, z)

                //Where each vertex is in relation to the rest of the map as a percentage. in this case. It is a percentage between 0 and 1
                meshData.uvs[vertixIndex] = new Vector2(x / (float)width, y / (float)height);

                //This Generates ONE Quad
                if (x < width - 1 && y < height - 1) //Ignore Right and Bottom Edge vertices
                {
                    //Generate triangles
                    meshData.AddTriangle(vertixIndex, vertixIndex + width + 1, vertixIndex + width);
                    meshData.AddTriangle(vertixIndex + width + 1, vertixIndex, vertixIndex + 1);
                }


                vertixIndex++;
            }
        }




        return meshData;    //Contains All Data for the Mesh
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;       //UV map so that we can add Textures
    public int width;
    public int height;

    int triangleIndex;

    //Pathing
    public LinkedList<List<Vector3>> paths;

    public MeshData(int meshWidth, int meshHeight)
    {
        width = meshWidth;
        height = meshHeight;
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public int centerIndex { get; internal set; }

    //Generate triangles
    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public bool isValidVertex(float x, float z)
    {
        // Convert world coordinates to grid coordinates
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;
        
        int gridX = Mathf.RoundToInt(x - topLeftX);
        int gridZ = Mathf.RoundToInt(topLeftZ - z);
        
        return gridX >= 0 && gridX < width && gridZ >= 0 && gridZ < height;
    }



    //generate Mesh
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals(); //For Lighting
        return mesh;
    }

    internal int CalculateIndexFromCoords(float x, float z)
    {
        // Convert world coordinates to grid coordinates
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;
        
        int gridX = Mathf.RoundToInt(x - topLeftX);
        int gridZ = Mathf.RoundToInt(topLeftZ - z);
        
        if (gridX >= 0 && gridX < width && gridZ >= 0 && gridZ < height)
        {
            return gridZ * width + gridX;
        }
        
        return -1;
    }
}
