using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VertexPosition : IEquatable<VertexPosition>, IComparable<VertexPosition>
{
    public static List<Vector2Int> possibleNeighbours = new List<Vector2Int> {

        new Vector2Int(0, -1),  //Down                  //potantial neighbours because we move from Neighbour to Neighbour
        new Vector2Int(0, 1),   //Up
        new Vector2Int(1, 0),   //Right
        new Vector2Int(-1, 0),  //Left
    };

    public float totalCost, estimatedCost;
    public VertexPosition previousVertex = null;        //posiiton we have come from.
    private Vector3 position;
    private bool isTaken;

    public int X { get => (int)position.x; }
    public int Z { get => (int)position.z; }

    public Vector3 Position { get => position; }
    public bool IsTaken { get => isTaken; }

    public VertexPosition(Vector3 position, bool isTaken = false)
    {
        this.position = position;
        this.isTaken = isTaken;
        this.estimatedCost = 0;
        this.totalCost = 1;
    }

    public override int GetHashCode()
    {
        return ((int)position.x * 1000 + (int)position.z).GetHashCode();
    }

    public int CompareTo(VertexPosition other)
    {
        if (this.estimatedCost < other.estimatedCost) return -1;         //Comes before other
        if (this.estimatedCost > other.estimatedCost) { return 1; }      //Vertex pos comes after
        else { return 0; }                                              //Both Vertices Are Equal.
    }

    public bool Equals(VertexPosition other)
    {
        if (other == null) return false;
        return Mathf.Abs(Position.x - other.Position.x) < 0.1f && 
               Mathf.Abs(Position.z - other.Position.z) < 0.1f;
    }
}