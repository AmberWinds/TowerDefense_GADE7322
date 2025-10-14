using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefenderShop : MonoBehaviour
{
    public static DefenderShop Instance { get; private set; }

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
    }// End of Awake

    /*
     * Defender types and Natural Enemies:
     * Goblin and basic Fight each other
     * Plant > BrickDude
     * Dark > Skeleton
     * Skeleton > Plant
     * BrickDude > Dark
     */

    [Header("Defender Types")]
    [SerializeField] GameObject basicDef, plantDef, darkDef;
    [Header("Cost")]
    [SerializeField] int basicCost, plantCost, darkCost;
    Transform defenderPos;

    [Header("UI")]
    [SerializeField] Canvas shopUI;
    [SerializeField] TextMeshProUGUI basicCost_txt, plantCost_txt, darkCost_txt;


    private void Start()
    {
        //Update Cost At Start      --->     Cost For Towers Remains The Same unlike Infrastructure.
        basicCost_txt.text = basicCost.ToString();
        plantCost_txt.text = plantCost.ToString();
        darkCost_txt.text = darkCost.ToString();

        shopUI.gameObject.SetActive(false);
        defenderPos = null; 
    }

    public void OpenDefenderShop(Transform defPos)
    {
        shopUI.gameObject.SetActive(true);

        if ( defPos != null )
        {
            defenderPos = defPos;
        }
        else
        {
            Debug.Log("Passed Position in Defender Shop is Null");
        }
    }

    public void CloseDefenderShop()
    {
        shopUI.gameObject.SetActive(false);
        defenderPos = null;             //set back to null.
    }


    public void BuyBasicTower()
    {
        if( defenderPos == null ) return;           //Make Sure The Position has been assigned.

        EconomyManager.Instance.BuyTower(basicCost);
        DefenderPlacement.Instance.SpawnInDefender(defenderPos.position, basicDef);
        Destroy(defenderPos.gameObject);        //Destroy the Tent GameObject 
        CloseDefenderShop();
    }

    public void BuyPlantTower()
    {
        if (defenderPos == null) return;                    //Make Sure The Position has been assigned.

        EconomyManager.Instance.BuyTower(plantCost);
        DefenderPlacement.Instance.SpawnInDefender(defenderPos.position, plantDef);
        Destroy(defenderPos.gameObject);        //Destroy the Tent GameObject 
        CloseDefenderShop();
    }

    public void BuyDarkTower()
    {
        if (defenderPos == null) return;                    //Make Sure The Position has been assigned.

        EconomyManager.Instance.BuyTower(darkCost);
        DefenderPlacement.Instance.SpawnInDefender(defenderPos.position, darkDef);
        Destroy(defenderPos.gameObject);        //Destroy the Tent GameObject 
        CloseDefenderShop();
    }
}

