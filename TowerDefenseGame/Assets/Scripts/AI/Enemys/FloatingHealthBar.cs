using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Transform target;
    Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }


    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue/maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
                         mainCam.transform.rotation * Vector3.up);

    }
}
