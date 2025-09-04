using Unity.AI.Navigation;
using UnityEngine;

public class NavigationUpdate : MonoBehaviour
{
    public bool clickme = false;
    public NavMeshSurface surface;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        surface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (clickme)
        {
            clickme = false;
            surface.BuildNavMesh();
        }
    }
}
