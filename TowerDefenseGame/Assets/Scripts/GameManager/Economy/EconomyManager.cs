using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    public float tickInterval = 10;         //Interval of time betweeen income.

    [Header("Resources")]
    public double resources;                 //Money Basically
    public TextMeshProUGUI totalResourcesText;
    public double defenderPrice = 50;

    [Header("Infrastructure")]
    public List<InfrastructureData> dataLog;
    public Dictionary<InfrastructureData, int> owned = new();                           //key is the Business and the value is the amount owned
    public Dictionary<InfrastructureData, double> currentCosts = new();       //key is the Business and the value is the current cost of that business.

    private Coroutine incomeRoutine;

    [SerializeField] InfrastructureUI ui;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (dataLog == null)
        {
            dataLog = new List<InfrastructureData>();
        }
        incomeRoutine = StartCoroutine(IncomeLoop());

        foreach (var data in dataLog)
        {
            if (data == null) continue;
            if (!owned.ContainsKey(data))
            {
                owned[data] = 0;
            }
            if (!currentCosts.ContainsKey(data))
            {
                currentCosts[data] = data.baseCost;
            }
        }

        UpdateUI();

        if (ui != null)
        {
            ui.UpdateUIShop();
        }

    }

    private IEnumerator IncomeLoop()
    {
        var wait = new WaitForSeconds(tickInterval);
        while (true)
        {
            yield return wait;
            PayPassiveIncome();
        }
    }

    private void PayPassiveIncome()
    {
        double income = 0;
        foreach(var business in owned)          //Goes thru all owned businesses and adds to our resources;
        {
            var data = business.Key;
            var count = business.Value;
            income += data.incomePerTick * count;
        }

        resources += income;
        UpdateUI(); //At the Bottom
    }

    public void TryBuyInfrastructure(InfrastructureData data)
    {
        if (data == null) return;

        if (!currentCosts.ContainsKey(data))
        {
            currentCosts[data] = data.baseCost;
        }
        if (!owned.ContainsKey(data))
        {
            owned[data] = 0;
        }

        double cost = currentCosts[data];

        if(resources>=cost)
        {
            //YAY we can afford
            resources -= cost;
            owned[data] = owned[data] + 1;  //Success we Bought Building

            if(data.costMultiplier > 1)
            {
                //OH NO, It gets more expensive
                currentCosts[data] = System.Math.Round(cost * data.costMultiplier, 2);      //had to make everything Double. DO NOT CHANGE
            }
        }
        else
        {
            Debug.LogWarning($"Not enough Resources to Purschase {data.name}");
        }

        UpdateUI(); //At the Bottom
    }

    public bool BuyTower()
    {
        if(resources >= defenderPrice)
        {
            resources -= defenderPrice;
            UpdateUI();
            return true;
        }

        return false;
    }


    private void UpdateUI() //Almost forgot about this.
    {
        if(totalResourcesText != null)
        {
            totalResourcesText.text = resources.ToString();
        }
        if(ui != null)
        {
            ui.UpdateUIOwned();
            ui.UpdateUIShop();
        }

    }
}


