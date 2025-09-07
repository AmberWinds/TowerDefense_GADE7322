using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DefenderPlacement : MonoBehaviour
{
    /* Okay,, so this one will handle the Spawning of the enemis as well as managin the placement depending on pathing.
     *  Spawning can be based off of the GridPlacement thingy class.
     *  
     *  Spawn in different types of defenders, each with their own cost.
     *  - Click to Spawn.
     */

    [SerializeField] GameObject defender;
    [SerializeField] GameObject tempDefender;
    public Vector3 tempDefScale = Vector3.one;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SpawnDefenderPlacements(List<Vector3> defenderPositions, List<Vector3> defenderDirections)
    {
        if (tempDefender == null) return;

        for (int i = 0; i < defenderPositions.Count; i++)
        {
            Vector3 pos = defenderPositions[i];
            Vector3 dir = defenderDirections[i];
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

            GameObject defender = Instantiate(tempDefender, pos, rot, transform);
            defender.transform.localScale = tempDefScale;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
