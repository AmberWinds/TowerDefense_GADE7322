using TMPro;
using UnityEngine;

public class InfrastructureUI : MonoBehaviour
{
    [Header("Owned")]
    public TextMeshProUGUI AgriOwned;
    public TextMeshProUGUI MineOwned;
    public TextMeshProUGUI EntOwned;
    public TextMeshProUGUI ResOwned;

    [Header("Cost")]
    public TextMeshProUGUI AgriCost;
    public TextMeshProUGUI MineCost;
    public TextMeshProUGUI EntCost;
    public TextMeshProUGUI ResCost;

    [Header("Economy Data")]
    public EconomyManager EconomyManager;
    private InfrastructureData agri;
    private InfrastructureData Mine;
    private InfrastructureData Ent;
    private InfrastructureData Res;

    [Header("UI")]
    public GameObject Shop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Shop != null)
        {
            Shop.SetActive(false);
        }

        if (EconomyManager == null)
        {
            EconomyManager = EconomyManager.Instance;
        }
        if (EconomyManager == null) return;

        foreach(var data in EconomyManager.dataLog)
        {
            if(data == null) continue;
            if(data.name == "Agriculture")
            {
                agri = data;
            }
            else if(data.name == "Entertainment")
            {
                Ent = data;
            }
            else if(data.name == "Mine")
            {
                Mine = data;
            }
            else if(data.name == "Research")
            {
                Res = data;
            }
        }
        // Initial UI refresh in case EconomyManager updated before us
        UpdateUIShop();
        UpdateUIOwned();
    }

    public void OpenShop()
    {
        if (Shop != null)
        {
            Shop.SetActive(true);
        }
        UpdateUIShop();
    }

    public void CloseShop()
    {
        if (Shop != null)
        {
            Shop.SetActive(false);
        }
    }


    public void UpdateUIShop()
    {
        if (EconomyManager == null) return;
        if (AgriCost != null && agri != null && EconomyManager.currentCosts.ContainsKey(agri))
            AgriCost.text = EconomyManager.currentCosts[agri].ToString();
        if (MineCost != null && Mine != null && EconomyManager.currentCosts.ContainsKey(Mine))
            MineCost.text = EconomyManager.currentCosts[Mine].ToString();
        if (EntCost != null && Ent != null && EconomyManager.currentCosts.ContainsKey(Ent))
            EntCost.text = EconomyManager.currentCosts[Ent].ToString();
        if (ResCost != null && Res != null && EconomyManager.currentCosts.ContainsKey(Res))
            ResCost.text = EconomyManager.currentCosts[Res].ToString();
    }

    public void UpdateUIOwned()
    {
        if (EconomyManager == null) return;
        if (AgriOwned != null && agri != null && EconomyManager.owned.ContainsKey(agri))
            AgriOwned.text = EconomyManager.owned[agri].ToString();
        if (MineOwned != null && Mine != null && EconomyManager.owned.ContainsKey(Mine))
            MineOwned.text = EconomyManager.owned[Mine].ToString();
        if (EntOwned != null && Ent != null && EconomyManager.owned.ContainsKey(Ent))
            EntOwned.text = EconomyManager.owned[Ent].ToString();
        if (ResOwned != null && Res != null && EconomyManager.owned.ContainsKey(Res))
            ResOwned.text = EconomyManager.owned[Res].ToString();
    }
}


