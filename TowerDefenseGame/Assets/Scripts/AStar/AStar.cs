using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static List<Vector3> GetPath(Vector3 startPos, Vector3 target, MeshData meshData)
    {

        // Check if start and target are the same
        if (Mathf.Abs(startPos.x - target.x) < 0.1f && Mathf.Abs(startPos.z - target.z) < 0.1f)
        {
            Debug.LogWarning("A* Start and target are the same position - returning empty path");
            return new List<Vector3> { startPos };
        }
        
        VertexPosition startVertex = new VertexPosition(startPos);
        VertexPosition targetVertex = new VertexPosition(target);

        List<Vector3> path = new List<Vector3>();
        List<VertexPosition> openedList = new List<VertexPosition>();
        HashSet<VertexPosition> closedList = new HashSet<VertexPosition>();

        startVertex.estimatedCost = TowerDistance(startVertex, targetVertex);
        openedList.Add(startVertex);
        
        VertexPosition currentVertex = null;
        int iterations = 0;

        while( openedList.Count > 0)
        {
            iterations++;
            openedList.Sort();
            currentVertex = openedList[0];


            if( currentVertex.Equals(targetVertex))
            {
                while(currentVertex != null)
                {
                    path.Add(currentVertex.Position);
                    currentVertex = currentVertex.previousVertex;
                }

                path.Reverse();
                break;
            }

            var arrayOfNeighbours = FindNeighboursFor(currentVertex, meshData);
            int validNeighbours = 0;
            int blockedNeighbours = 0;

            foreach( var neighbour in arrayOfNeighbours)
            {
                if (neighbour == null || closedList.Contains(neighbour)){ continue; }

                if(neighbour.IsTaken == false)
                {
                    var totalCost = currentVertex.totalCost + 1;
                    var neighbourEstimatedCost = TowerDistance(neighbour, targetVertex);
                    neighbour.totalCost = totalCost;
                    neighbour.previousVertex = currentVertex;
                    neighbour.estimatedCost = totalCost + neighbourEstimatedCost;

                    if(openedList.Contains(neighbour) == false)
                    {
                        openedList.Add(neighbour);
                        validNeighbours++;
                    }
                }
                else
                {
                    blockedNeighbours++;
                }
            }

            closedList.Add(currentVertex);
            openedList.Remove(currentVertex);
        }

        if (openedList.Count == 0)
        {
            Debug.LogWarning("A* exhausted all possibilities without finding a path");
        }

        return path;
    }

    private static VertexPosition[] FindNeighboursFor(VertexPosition currentVertex, MeshData meshData)
    {
        VertexPosition[] arrayOfNeighbours = new VertexPosition[4];      //Number of spaces that we can move is 4 because we are not moving diagonally.

        int arrayIndex = 0;
        foreach (var possibleNeighbour in VertexPosition.possibleNeighbours)
        {
            // Use actual mesh coordinate 
            Vector3 position = new Vector3(currentVertex.Position.x + possibleNeighbour.x, 0.2f ,currentVertex.Position.z + possibleNeighbour.y);
            
            if (meshData.isValidVertex(position.x, position.z))
            {
                // No obstacles to check - all positions are walkable
                arrayOfNeighbours[arrayIndex] = new VertexPosition(position, false);
                arrayIndex++;
            }
            else
            {
                // Invalid neighbor 
            }
        }

        return arrayOfNeighbours;
    }

    private static float TowerDistance(VertexPosition startPos, VertexPosition targetVertex)
    {
        return Mathf.Abs(startPos.Position.x - targetVertex.Position.x) + Mathf.Abs(startPos.Position.z - targetVertex.Position.z);
    }
}

