using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    public Slider slider;
    public Transform target;

    public void UpdateHealthBar(float current, float max)
    {
        slider.value = current/max;
        transform.rotation = Camera.main.transform.rotation;

    }
}
