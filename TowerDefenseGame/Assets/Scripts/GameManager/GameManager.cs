using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    public GameManager Instance;

    //Variables
    public GameObject mainTower;
    public MeshFilter terrainMeshFilter;
    public MeshCollider terrainMeshCollider;
    public float flattenRadius = 10.0f;
    public float flattenHeight = 1.5f;

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

        SpawnMainTower(); //Spawn in The BOSS
    }

    private void SpawnMainTower()
    {

        if (mainTower == null)
        {
            Debug.LogWarning("SpawnMainTowerAtCenter: mainTower prefab is not assigned.");
            return;
        }

        Vector3 worldPos = terrainMeshFilter.transform.TransformPoint(
            new Vector3(centerPoint.x, flattenHeight, centerPoint.z)
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


        Mesh m = terrainMeshFilter.sharedMesh;
        m.RecalculateNormals();
        m.RecalculateBounds();

        terrainMeshCollider.sharedMesh = null;
        terrainMeshCollider.sharedMesh = m;
        Physics.SyncTransforms();



    }
}
