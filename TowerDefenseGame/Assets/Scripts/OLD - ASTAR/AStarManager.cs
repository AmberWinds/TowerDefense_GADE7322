using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AStarManager 
{
    public static AStarManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        return null;
    }

}
