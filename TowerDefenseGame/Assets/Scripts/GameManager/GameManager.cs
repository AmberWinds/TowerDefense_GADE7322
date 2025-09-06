using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Mesh;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Variables
    public GameObject mainTower;
    public MeshFilter terrainMeshFilter;
    public MeshCollider terrainMeshCollider;
    public GridObjectSpawner spawner;
    public NavigationUpdate navigation;

    public float flattenRadius = 30.0f;
    public float flattenHeight = 0f;
    public int spawnPosAmount = 3;

    private MeshData mesh;
    int centerX;
    int centerY;
    Vector3 centerPoint;
    List<Vector3> enemySpawnPos;
    private LinkedList<List<Vector3>> paths;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
        }

        enemySpawnPos = new List<Vector3>();
        paths = new LinkedList<List<Vector3>>();
    }


    public void BeginGeneratingGame(MeshData meshdata)
    {
        if (meshdata == null || meshdata.vertices == null || meshdata.vertices.Length == 0)
        {
            Debug.LogError("BeginGeneratingGame: meshdata is null or empty.");
            return;
        }

        this.mesh = meshdata;

        //Begin by finding the middle of the mesh.
        centerX = meshdata.width / 2;
        centerY = meshdata.height / 2;
        int centerIndex = centerY * meshdata.width + centerX;

        centerPoint = meshdata.vertices[centerIndex];
        
        enemySpawnPos = SpawnEnemyStartPos();

        //Flatten the Middle of The Map
        FlattenAreaAroundAreas(centerPoint, enemySpawnPos);

        SpawnMainTower(); //Spawn in The BOSS BOY       -- IT WORKS, we can change the thingy leter

        FindPath();     //IT WORKS, NO TOUCH
        
        // Flatten the mesh around all path positions
        FlattenPaths();

        meshdata.paths = paths;     //This little meshdata class holds ALL POWER
    }

    private void FindPath()
    {
        for (int i = 0; i < enemySpawnPos.Count; i++)
        {
            paths.AddFirst(AStar.GetPath(enemySpawnPos[i], centerPoint, GridObjectSpawner.Instance.objects, mesh));

        }


    }

    private List<Vector3> SpawnEnemyStartPos()
    {
        int width = mesh.width;
        int height = mesh.height;

        List<Vector3> edgeVertices = new List<Vector3>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    int i = y * width + x;
                    edgeVertices.Add(mesh.vertices[i]);
                }
            }
        }

        List<Vector3> results = new List<Vector3>();
        for (int k = 0; k < spawnPosAmount && edgeVertices.Count > 0; k++)
        {
            int idx = UnityEngine.Random.Range(0, edgeVertices.Count);
            results.Add(edgeVertices[idx]);
            edgeVertices.RemoveAt(idx); // ensures no duplicates
        }

        return results;

    }

    private void SpawnMainTower()
    {

        if (mainTower == null)
        {
            Debug.LogWarning("SpawnMainTowerAtCenter: mainTower prefab is not assigned.");
            return;
        }

        Vector3 worldPos = terrainMeshFilter.transform.TransformPoint(
            new Vector3(centerPoint.x, flattenHeight - 1, centerPoint.z)
        );

        Instantiate(mainTower, worldPos, Quaternion.identity);
    
    }


    private void FlattenAreaAroundAreas(Vector3 mainTowerPos, List<Vector3> enemySpawnPos)
    {
        for (int y = 0; y < mesh.height; y++)
        {
            for (int x = 0; x < mesh.width; x++)
            {
                int i = y * mesh.width + x;
                Vector3 v = mesh.vertices[i];

                // Distance in 2D (ignore height)
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));

                if (dist < flattenRadius)
                {
                    v.y = flattenHeight;
                    mesh.vertices[i] = v;
                }
                
                foreach(var spawnpos  in enemySpawnPos)
                {
                    float dist2 = Vector2.Distance(new Vector2(x, y), new Vector2(spawnpos.x, spawnpos.y));

                    if (dist2 < flattenRadius)
                    {
                        v.y = flattenHeight;
                        mesh.vertices[i] = v;
                    }
                }

            
            }
        }

        foreach (var obj in spawner.objects)
        {
            float objDist = Vector2.Distance(new Vector2(obj.transform.position.x, obj.transform.position.y), new Vector2(centerX, centerY));

            if (objDist < flattenRadius)
            {
                Destroy(obj);
            }
        }

        terrainMeshFilter.sharedMesh.vertices = mesh.vertices;
        terrainMeshFilter.sharedMesh.RecalculateBounds();
        terrainMeshFilter.sharedMesh.RecalculateNormals();


        terrainMeshCollider.sharedMesh = null;
        terrainMeshCollider.sharedMesh = terrainMeshFilter.sharedMesh;
        Physics.SyncTransforms();


        navigation.surface.BuildNavMesh();

    }

    private void FlattenPaths()
    {
        if (paths == null || paths.Count == 0)
        {
            Debug.LogWarning("FlattenPaths: No paths available to flatten.");
            return;
        }

        Debug.Log($"FlattenPaths: Starting to flatten {paths.Count} paths.");

        // Iterate through each path in the LinkedList
        foreach (var path in paths)
        {
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("FlattenPaths: Skipping null or empty path.");
                continue;
            }

            // Iterate through each position in the current path
            foreach (var position in path)
            {
                FlattenAreaAroundPosition(position);
            }
        }

        // Update the mesh after all flattening is complete
        UpdateMeshAfterFlattening();
        
        Debug.Log("FlattenPaths: Completed flattening all paths.");
    }

    private void FlattenAreaAroundPosition(Vector3 position)
    {
        // Convert world position to grid coordinates
        int gridX = Mathf.RoundToInt(position.x);
        int gridZ = Mathf.RoundToInt(position.z);

        // Iterate through the mesh vertices around the position
        for (int y = 0; y < mesh.height; y++)
        {
            for (int x = 0; x < mesh.width; x++)
            {
                int i = y * mesh.width + x;
                Vector3 v = mesh.vertices[i];

                // Calculate distance from current vertex to the path position (2D distance)
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(gridX, gridZ));

                // If within flatten radius, set height to flattenHeight
                if (distance < flattenRadius)
                {
                    v.y = flattenHeight;
                    mesh.vertices[i] = v;
                }
            }
        }
    }

    private void UpdateMeshAfterFlattening()
    {
        // Update the mesh filter
        terrainMeshFilter.sharedMesh.vertices = mesh.vertices;
        terrainMeshFilter.sharedMesh.RecalculateBounds();
        terrainMeshFilter.sharedMesh.RecalculateNormals();

        // Update the mesh collider
        terrainMeshCollider.sharedMesh = null;
        terrainMeshCollider.sharedMesh = terrainMeshFilter.sharedMesh;
        Physics.SyncTransforms();

        // Rebuild navigation mesh
        navigation.surface.BuildNavMesh();
    }
}
