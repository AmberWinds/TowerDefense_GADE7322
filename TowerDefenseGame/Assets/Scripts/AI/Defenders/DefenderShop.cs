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
    private enum defenderType { basic, plant, dark }

    [SerializeField] GameObject basicDef;
    [SerializeField] GameObject plantDef;
    [SerializeField] GameObject darkDef;

    public void BuyBasicTower()
    {

    }

    public void BuyPlantTower()
    {

    }

    public void BuyDarkTower()
    {

    }
}
