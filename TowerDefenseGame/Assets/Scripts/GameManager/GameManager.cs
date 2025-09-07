using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Variables
    [Header("Main Tower")]
    public GameObject mainTower;
    public Vector3 scale = Vector3.one;
    private MeshData mesh;
    int centerX;
    int centerY;

    [Header("Terrain")]
    public MeshFilter terrainMeshFilter;
    public MeshCollider terrainMeshCollider;

    [Header("Spawner")]
    public GridObjectSpawner spawner;
    public int spawnPosAmount = 3;
    public NavigationUpdate navigation;

    [Header("Defenders")]
    public DefenderPlacement defenderPlacement;
    public int defenderAmount;
    public float sideOffset;
    public bool closed = false;         //Treat Paths as Loops

    private List<Vector3> defenderPositions;
    private List<Vector3> defenderDirections;

    [Header("Flattening")]
    public float flattenRadius = 30.0f;
    public float flattenHeight = 0f;

    //Paths
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
        defenderDirections = new List<Vector3>();
        defenderPositions = new List<Vector3>(); 
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
        FlattenAreaAroundMainPos(centerPoint, enemySpawnPos);

        SpawnMainTower(); //Spawn in The BOSS BOY       -- IT WORKS, we can change the thingy leter

        FindPath();     //IT WORKS, NO TOUCH
        
        // Flatten the mesh around all path positions
        FlattenPaths();

        meshdata.paths = paths;     //This little meshdata class holds ALL POWER

        Pathing.instance.SpawnPaths(meshdata);

        FindDefenderPlacements();
        defenderPlacement.SpawnDefenderPlacements(defenderPositions, defenderDirections);

        UpdateMeshAfterFlattening();
        
        // Spawn objects after pathing is complete
        SpawnObjects();
    }
    
    //Defender
    private void FindDefenderPlacements()
    {
        if (paths == null)      //Null Check
        {
            Debug.LogWarning("Paths is null");
            return;
        }

        foreach (var path in paths)
        {
            FindSinglePathDefenders(path);
        }


    }

    private void FindSinglePathDefenders(List<Vector3> path)
    {
        //Need the length for even distribution
        float totalLen = ComputeTotalLength(path, closed);
        if (totalLen <= 0) return;

        float step = (defenderAmount > 1) ? (totalLen / (defenderAmount - 1)) : 0f;     //How far apart each defender shoudl be from one another

        for (int i = 0; i <defenderAmount; i++)
        {
            float d = i * step;
            d = Mathf.Min(d, totalLen);         // clamp tiny float errors at the end

            // Get the actual world position at this distance along the path
            float sideSign = (i % 2 == 0) ? -1f : 1f; // even = left, odd = right
            Vector3 defenderPosition = GetPositionAtDistance(path, d, sideSign);

            // Store the position 
            defenderPositions.Add(defenderPosition);

            // Get direction/rotation for the defender
            Vector3 direction = GetDirectionAtDistance(path, d);

            //Vector3
            defenderDirections.Add(direction);

            foreach(var def in defenderPositions)
            {
                FlattenAreaAroundPosition(def);
            }
        }
    }

    private Vector3 GetPositionAtDistance(List<Vector3> path, float targetDistance, float sideSign)
    {
        if (path.Count < 2) return path.Count > 0 ? path[0] : Vector3.zero;

        float currentDistance = 0f;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 segmentStart = path[i];
            Vector3 segmentEnd = path[i + 1];
            float segmentLength = Vector3.Distance(segmentStart, segmentEnd);

            // Check to see if target distance falls within this segment
            if (currentDistance + segmentLength >= targetDistance)
            {
                // Calculate how far along this segment we need to be
                float distanceIntoSegment = targetDistance - currentDistance;
                float t = segmentLength > 0 ? (distanceIntoSegment / segmentLength) : 0f;

                // Interpolate between segment start and end
                Vector3 basePosition = Vector3.Lerp(segmentStart, segmentEnd, t);

                // Apply lateral offset from the path center respecting requested side
                float offsetMagnitude = Mathf.Abs(sideOffset);
                if (offsetMagnitude > Mathf.Epsilon)
                {
                    Vector3 segmentDirection = (segmentEnd - segmentStart);
                    if (segmentDirection.sqrMagnitude > 1e-6f)
                    {
                        segmentDirection.Normalize();
                        // Right-hand perpendicular in XZ plane
                        Vector3 perpendicularXZ = new Vector3(-segmentDirection.z, 0f, segmentDirection.x);
                        basePosition += perpendicularXZ * (offsetMagnitude * Mathf.Sign(sideSign));
                    }
                }

                return basePosition;
            }

            currentDistance += segmentLength;
        }

        //return last point
        return path[path.Count - 1];
    }

    private Vector3 GetDirectionAtDistance(List<Vector3> path, float targetDistance)
    {
        if (path.Count < 2) return Vector3.forward;

        float currentDistance = 0f;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 segmentStart = path[i];
            Vector3 segmentEnd = path[i + 1];
            float segmentLength = Vector3.Distance(segmentStart, segmentEnd);

            // Check if target distance falls within this segment
            if (currentDistance + segmentLength >= targetDistance)
            {
                // Return the direction of this segment
                Vector3 direction = (segmentEnd - segmentStart).normalized;
                return direction;
            }

            currentDistance += segmentLength;
        }

        // Return direction of the last segment
        Vector3 lastSegmentStart = path[path.Count - 2];
        Vector3 lastSegmentEnd = path[path.Count - 1];
        return (lastSegmentEnd - lastSegmentStart).normalized;
    }

    private float ComputeTotalLength(List<Vector3> path, bool closed)
    {
        if (path.Count < 2) return 0f;

        float totalLength = 0f;
        int last = path.Count - 1;

        // Calculate length of segments
        for (int i = 0; i < path.Count - 1; i++)
        {
            totalLength += Vector3.Distance(path[i], path[i + 1]);
        }

        // If closed, add distance from last point back to first
        if (closed && path.Count > 2)
        {
            totalLength += Vector3.Distance(path[path.Count - 1], path[0]);
        }

        return totalLength;
    }


    //Pathing
    private void FindPath()
    {
        for (int i = 0; i < enemySpawnPos.Count; i++)
        {
            paths.AddFirst(AStar.GetPath(enemySpawnPos[i], centerPoint, mesh));

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

        GameObject obj = Instantiate(mainTower, worldPos, Quaternion.identity);
        obj.transform.localScale = scale;
    
    }


    private void FlattenAreaAroundMainPos(Vector3 mainTowerPos, List<Vector3> enemySpawnPos)
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
                
                foreach(var spawnpos in enemySpawnPos)
                {
                    float dist2 = Vector2.Distance(new Vector2(x, y), new Vector2(spawnpos.x, spawnpos.z));

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


    }

    private void FlattenPaths()
    {
        if (paths == null || paths.Count == 0)
        {
            Debug.LogWarning("FlattenPaths: No paths available to flatten.");
            return;
        }


        int totalPositions = 0;
        // Iterate through each path in the LinkedList
        foreach (var path in paths)
        {
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("FlattenPaths: Skipping null or empty path.");
                continue;
            }

            totalPositions += path.Count;

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

    private void SpawnObjects()
    {
        
        if (spawner != null)
        {
            // Passes the paths to the spawner so it can avoid spawning on them
            spawner.PlaceObjects(paths);
        }
        else
        {
            Debug.LogWarning("SpawnObjects: GridObjectSpawner is not assigned.");
        }
    }




    private void FlattenAreaAroundPosition(Vector3 position)
    {
        
        int verticesModified = 0;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 v = mesh.vertices[i];
            
            // Calculate distance from current vertex to the path position (2D distance, ignoring height)       //We can ignore height because we are going to flatten  it.
            float dist = Vector2.Distance(new Vector2(v.x, v.z), new Vector2(position.x, position.z));

            if (dist < flattenRadius)
            {
                v.y = flattenHeight;
                mesh.vertices[i] = v;
                verticesModified++;
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
