using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NarrativeManager : MonoBehaviour
{
    //public static NarrativeManager Instance { get; private set; }

    //private void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject); // Destroy duplicate instances
    //    }
    //    else
    //    {
    //        Instance = this;
    //    }
    //}

    [SerializeField] GameObject canvas;
    [SerializeField] TextMeshProUGUI story;
    [SerializeField] List<NarrativeBlueprint> blueprints;
    [SerializeField] GameObject tower;
    [SerializeField] float minTime;
    [SerializeField] float maxTime;

    private int maxEventNum;
    private int blueprintIndex;
    private int currentEvent;

    private void Start()
    {
        tower = GameObject.FindGameObjectWithTag("Tower");

        var setAmount = blueprints.Count;
        blueprintIndex = Random.Range(0, setAmount);

        canvas.gameObject.SetActive(true);
        story.text = blueprints[blueprintIndex].narrativeSet[0].paragraph;

        maxEventNum = blueprints[blueprintIndex].narrativeSet.Count;

        StartCoroutine(RunEvents());
    }

    public IEnumerator RunEvents()
    {
        while (true)                //Just Keep on Going
        {
            //Random Time
            float t = Random.Range(minTime, maxTime);
            Debug.Log("t in Narr Man: " + t);
            yield return new WaitForSeconds(t);
            BeginRandomSet();
        }
    }

    public void BeginRandomSet()
    {
        Debug.Log("Beginnging a Random ass set");

        //Pick Random Event
        currentEvent = Random.Range(1, maxEventNum);        //0 is always intro
        story.text = blueprints[blueprintIndex].narrativeSet[currentEvent].paragraph;

        canvas.gameObject.SetActive(true);
        Time.timeScale = 0;

        if(blueprints[blueprintIndex].narrativeSet[currentEvent].addEcon)
        {
            AddEconomy();
        }
        else if(blueprints[blueprintIndex].narrativeSet[currentEvent].addInfrastructure)
        {
            AddBuilding();
        }
        else if (blueprints[blueprintIndex].narrativeSet[currentEvent].addTowerHealth)
        {
            AddTowerHealth();
        }


    }

    private void AddTowerHealth()
    {
        tower = GameObject.FindGameObjectWithTag("Tower");
        if (tower == null) { Debug.LogWarning("tower is null"); return; }

        DefenderBehaviour def = tower.GetComponentInChildren<DefenderBehaviour>();
        Debug.Log("Add Tower Health is Successfully called");

        def.AddHealth(blueprints[blueprintIndex].narrativeSet[currentEvent].towerHealth);
    }

    private void AddBuilding()
    {
        EconomyManager.Instance.AddEventInfrastructure(blueprints[blueprintIndex].narrativeSet[currentEvent].infrastructure);
    }

    private void AddEconomy()
    {
        EconomyManager.Instance.AddEventIncome(blueprints[blueprintIndex].narrativeSet[currentEvent].econ);
    }
}
