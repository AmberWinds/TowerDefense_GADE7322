using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectSpawnRule
{
    public GameObject prefab;
    public int amount = 50;
    public float maxHeight = 0.6f;      //At which an object can be spawned
    public float minHeight = 0;         //0 Should be Fine
    public float maxSlopeAngle = 30f;         //Needs to spawn so that it sits on Slope
    public Vector3 scale = Vector3.one; //adding this because my objects are HUGE
}
//Knee deep in Caffeine and Feeling Great

[RequireComponent(typeof(MeshCollider))]
public class GridObjectSpawner : MonoBehaviour
{
    public static GridObjectSpawner Instance { get; private set; }                         //SINGLETON

    MeshCollider meshCollider;
    MeshFilter filter;

    public List<GameObject> objects;
    public List<ObjectSpawnRule> spawnRules;


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

    private void Start()
    {
        objects = new List<GameObject>();

        meshCollider = GetComponent<MeshCollider>();
        filter = GetComponent<MeshFilter>();
        Mesh m = filter.sharedMesh;
        m.RecalculateNormals();
        m.RecalculateBounds();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = m;
        Physics.SyncTransforms();

        Debug.Log("Shared Mesh: " + meshCollider.sharedMesh != null);
        PlaceObjects();

    }


    public void PlaceObjects()
    {

        Bounds bounds = meshCollider.bounds;
        Debug.Log("Place Objects is Called");

        foreach (ObjectSpawnRule rule in spawnRules)
        {
            int spawned = 0;
            int attempted = 0;

            while (spawned < rule.amount && attempted < rule.amount * 10)
            {
                attempted++;

                //Need to find random points within the bounds:
                float x = Random.Range(bounds.min.x, bounds.max.x);
                float z = Random.Range(bounds.min.z, bounds.max.z);
                Vector3 rayOrigin = new Vector3(x, bounds.max.y, z);        //x and z are like edges of the mesh. and y makes sure it spawns on top.

                //Debug.Log("ray Origin Found: " + rayOrigin + "Bounds x " + bounds.max.x + " and z " + bounds.max.z);

                Ray ray = new Ray(rayOrigin, Vector3.down);                 //Create a downward pointing ray

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.collider != meshCollider) continue;             //Ignore hits on other colliders (trees, props, terrain, etc.). Only accept a hit if it’s this mesh’s collider.

                    float worldY = hit.point.y;                             //Extract the surface Y position and the surface normal at the hit point.
                    Vector3 normal = hit.normal;

                    //normalize height between bounds (get decimal 0-1)
                    float heightNorm = Mathf.InverseLerp(bounds.min.y, bounds.max.y, worldY);

                    //slope angle check
                    float slope = Vector3.Angle(normal, Vector3.up);    //Compute the slope angle in degrees. 0° means perfectly flat (facing straight up); larger angles = steeper surfaces.

                    if (heightNorm < rule.minHeight || heightNorm > rule.maxHeight) continue;   //Reject this point if its normalized height is outside the rule’s allowed vertical band.
                    if (slope > rule.maxSlopeAngle) continue;                                   //Reject if the slope is too steep for this rule.

                    Quaternion rotate = Quaternion.FromToRotation(Vector3.up, normal);      //Surface Up and Object are the same so that it sits flush. 
                    //now it can spawn

                    Vector3 spawnPos = new Vector3(x, worldY, z);
                    GameObject obj = Instantiate(rule.prefab, spawnPos, rotate, transform);      //transform makes it a child of the mesh. 
                    obj.transform.localScale = rule.scale;
                    objects.Add(obj);

                    spawned++;
                }
            }// End of While Loop



        }// end of foreach
    }//end of placeObjects

}
