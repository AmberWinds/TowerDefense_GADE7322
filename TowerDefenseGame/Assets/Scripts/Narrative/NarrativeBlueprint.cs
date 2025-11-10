using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NarrativeSet", menuName = "Narrative Data")]
public class NarrativeBlueprint : ScriptableObject
{
    public string setName;

    public List<NarrativeSet> narrativeSet;

    [System.Serializable]
    public class NarrativeSet
    {  
        public string paragraph;

        public bool addEcon, addInfrastructure, addTowerHealth;

        public int econ;
        public InfrastructureData infrastructure;
        public int towerHealth;
    }

}


