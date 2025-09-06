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

    public void SpawnPaths(MeshData data)
    {
        if(visualizePath)
        {
            foreach (var path in data.paths)
            {
                foreach (var pos in path)
                {
                    Instantiate(pathObj, pos, Quaternion.identity);
                }
            }
        }
        else
        {
            Debug.Log("Cannot Visuliaze Path as it is set to false");
        }


    }
}
