using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToUpgradeDef : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //variables
    [Tooltip("The Canvas attached to the GameObject that will display when the players mouse hovers over the Tower.")]
    [SerializeField] private GameObject upgradeNotifier;
    [SerializeField] GameObject upgradedDef;
    [SerializeField] TextMeshProUGUI notifierTxt;
    Camera mainCam;
    private int cost;

    private DefenderBehaviour defenderBehaviour;

    private void Start()
    {
        defenderBehaviour = GetComponent<DefenderBehaviour>();
        mainCam = Camera.main;

        cost = defenderBehaviour.GetDefenderCost();
        notifierTxt.text = cost.ToString();
    }

    private void Update()
    {
        if(upgradeNotifier != null)
        {
            upgradeNotifier.transform.LookAt(upgradeNotifier.transform.position + mainCam.transform.rotation * Vector3.forward,     //Always face Camera
                         mainCam.transform.rotation * Vector3.up);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpgradeDefender();
    }

    public void OnPointerEnter(PointerEventData eventData)      //Hovering
    {
        upgradeNotifier.SetActive(true);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        upgradeNotifier.SetActive(false);
    }

    void UpgradeDefender()
    {

        if(EconomyManager.Instance.UpgradeTower(cost) == true)
        {
            DefenderPlacement.Instance.SpawnInDefender(transform.position, upgradedDef);
            Destroy(gameObject);
        }

    }
}
