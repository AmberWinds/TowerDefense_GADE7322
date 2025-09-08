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

    public static DefenderPlacement Instance { get; private set; }

    [SerializeField] GameObject defender;
    [SerializeField] GameObject tempDefender;
    public Vector3 tempDefScale = Vector3.one;

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

    public void SpawnInDefender(Vector3 location)
    {
        Instantiate(defender, location, Quaternion.identity, transform);
    }

    public void SpawnSingleDefenderPlacement(Vector3 location)
    {
       GameObject obj = Instantiate(tempDefender, location, Quaternion.identity, transform);
        obj .transform.localScale = tempDefScale;
    }

}
