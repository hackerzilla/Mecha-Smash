using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;

public class HealthBar : MonoBehaviour
{
    public Health health;
    public Image  fillBar;
    private float initialBarSize;
    
    void OnEnable()
    {
        initialBarSize = fillBar.rectTransform.sizeDelta.x; 
        Assert.NotNull(health);
        health.onHealthChanged.AddListener(OnHealthChanged);
    }
    
    void OnDisable()
    {
        health.onHealthChanged.RemoveListener(OnHealthChanged);
    }
    
    void OnHealthChanged(float newHealth, float maxHealth)
    {
        Assert.NotNull(fillBar);
        float healthPercent = newHealth / maxHealth;
        fillBar.rectTransform.sizeDelta = new Vector2(initialBarSize * healthPercent,  fillBar.rectTransform.sizeDelta.y); 
    }
}
