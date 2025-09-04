using System;
using System.Data;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    public GameManager Instance;

    //Variables
    public GameObject mainTower;
    public MeshFilter terrainMeshFilter;
    public MeshCollider terrainMeshCollider;
    public GridObjectSpawner spawner;
    public NavigationUpdate navigation;


    public float flattenRadius = 30.0f;
    public float flattenHeight = 0f;

    private MeshData mesh;
    int centerX;
    int centerY;
    Vector3 centerPoint;

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

        //Flatten the Middle of The Map
        FlattenAreaAroundTower(centerPoint);

        SpawnMainTower(); //Spawn in The BOSS BOY       -- IT WORKS, we can change the thingy leter
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



    private void FlattenAreaAroundTower(Vector3 mainTowerPos)
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
}
