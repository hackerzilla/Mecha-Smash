using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health health;
    public Image  fillBar;
    private float initialBarSize;
    
    void Start()
    {
        initialBarSize = fillBar.rectTransform.sizeDelta.x; 
    }

    void Update()
    {
        
    }
    
    void OnHealthChanged()
    {
        float percent = health.currentHealth / health.maxHealth;
        fillBar.rectTransform.sizeDelta = new Vector2(initialBarSize * percent,  fillBar.rectTransform.sizeDelta.y); 
    }
}
