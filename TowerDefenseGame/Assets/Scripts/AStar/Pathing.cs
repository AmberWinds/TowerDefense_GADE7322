using UnityEngine;

public class Pathing : MonoBehaviour 
{
    public static Pathing instance;

    private void Awake()
    {
        instance = this;
    }

    public bool visualizePath = false;
    [SerializeField] GameObject pathObj;
    [SerializeField] Vector3 scale = Vector3.one;

    public void SpawnPaths(MeshData data)
    {
        if(visualizePath)
        {
            foreach (var path in data.paths)
            {
                foreach (var pos in path)
                {
                    GameObject obj = Instantiate(pathObj, pos, Quaternion.identity);
                    obj.transform.localScale = scale;
                }
            }
        }
        else
        {
            Debug.Log("Cannot Visuliaze Path as it is set to false");
        }


    }
}
