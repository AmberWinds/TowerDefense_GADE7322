using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static List<Vector3> GetPath(Vector3 startPos, Vector3 target, GameObject[] obstaclesArray, MeshData meshData)
    {

        VertexPosition startVertex = new VertexPosition(startPos);

        VertexPosition targetVertex = new VertexPosition(target);
        //God Help me

        List<Vector3> path = new List<Vector3>();

        List<VertexPosition> openedList = new List<VertexPosition>();
        HashSet<VertexPosition> closedList = new HashSet<VertexPosition>();

        startVertex.estimatedCost = TowerDistance(startVertex, targetVertex);
        openedList.Add(startVertex);
        
        VertexPosition currentVertex = null;
        
        while( openedList.Count > 0 )
        {
            openedList.Sort();
            currentVertex = openedList[0];

            if( currentVertex.Equals(currentVertex))
            {
                while(currentVertex != startVertex)
                {
                    path.Add(currentVertex.Position);
                    currentVertex = currentVertex.previousVertex;
                }

                path.Reverse();     //Started from the end so now we here
                break;
            }

            var arrayOfNeighbours = FindNeighboursFor(currentVertex, meshData, obstaclesArray);

            foreach( var neighbour in arrayOfNeighbours)
            {
                //Check is neighbout is nulll
                if (neighbour == null || closedList.Contains(neighbour)){ continue; }

                if(neighbour.IsTaken == false)
                {
                    var totalCost = currentVertex.estimatedCost + 1;
                    var neighbourEstimatedCost = TowerDistance(neighbour, targetVertex);
                    neighbour.totalCost = totalCost;
                    neighbour.previousVertex = currentVertex;
                    neighbour.estimatedCost = totalCost + neighbourEstimatedCost;

                    if(openedList.Contains(neighbour) == false)
                    {
                        openedList.Add(neighbour);
                    }
                }
            }

            closedList.Add(currentVertex);
            openedList.Remove(currentVertex);       //Do not want to check it again.

        }


        return path;
    }

    private static VertexPosition[] FindNeighboursFor(VertexPosition currentVertex, MeshData meshData, GameObject[] obstaclesArray)
    {
        VertexPosition[] arrayOfNeighbours = new VertexPosition[4];      //Number of spaces that we can move is 4 because we are not moving diagonally.

        int arrayIndex = 0;
        foreach (var possibleNeighbour in VertexPosition.possibleNeighbours)
        {
            Vector3 position = new Vector3(currentVertex.X + possibleNeighbour.x, 0, currentVertex.Z + possibleNeighbour.y);
            if (meshData.isValidVertex(position.x, position.z))
            {
                int index = meshData.CalculateIndexFromCoords(position.x, position.z);

                bool isTaken = false;
                foreach(var obj in obstaclesArray)
                {
                    if(obj.transform.position.x == position.x && obj.transform.position.z == position.z)
                    {
                        isTaken = true;
                        break;
                    }
                }
                
                arrayOfNeighbours[index] = new VertexPosition(position, isTaken);
                arrayIndex++;
            }
        }

        return arrayOfNeighbours;
    }

    private static float TowerDistance(VertexPosition startPos, VertexPosition targetVertex)
    {
        return Mathf.Abs(startPos.X - targetVertex.X + Mathf.Abs(startPos.Z - targetVertex.Z));
    }
}

