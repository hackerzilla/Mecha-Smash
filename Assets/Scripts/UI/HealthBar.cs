using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public MechHealth mechHealth;
    public Image  fillBar;
    private float initialBarSize;

    void OnEnable()
    {
        initialBarSize = fillBar.rectTransform.sizeDelta.x;
        mechHealth.onHealthChanged.AddListener(OnHealthChanged);
    }

    void OnDisable()
    {
        mechHealth.onHealthChanged.RemoveListener(OnHealthChanged);
    }

    void OnHealthChanged(float newHealth, float maxHealth)
    {
        float healthPercent = newHealth / maxHealth;
        fillBar.rectTransform.sizeDelta = new Vector2(initialBarSize * healthPercent,  fillBar.rectTransform.sizeDelta.y);
    }
}
