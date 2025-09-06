using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node cameFrom;
    public List<Node> nodes;

    public float gScore;
    public float hScore;

    public float GetFScore()
    {
        return gScore + hScore;
    }
}
