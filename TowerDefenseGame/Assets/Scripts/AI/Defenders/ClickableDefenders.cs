using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableDefenders : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler        //I have never done this before so I apologise in advance.
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color hoverColor = Color.yellow;
    private Color originalColour;

    void Awake()
    {
        if (!targetRenderer) targetRenderer.sharedMaterial = GetComponentInChildren<Renderer>().sharedMaterial;
        if (targetRenderer) originalColour = targetRenderer.sharedMaterial.color;     //Keep it the Og Colour
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("World object clicked!");
        //RUN CODE HERE
        ClickedDefender();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(targetRenderer) targetRenderer.sharedMaterial.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetRenderer) targetRenderer.sharedMaterial.color = originalColour;
    }

    private void ClickedDefender()
    {
        bool canSpawn = EconomyManager.Instance.BuyTower();
        if (canSpawn)
        {
            DefenderPlacement.Instance.SpawnInDefender(transform.position);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Not enoughh Resources");
        }

    }

}
