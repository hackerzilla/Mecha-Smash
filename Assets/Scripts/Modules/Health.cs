using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100.0f;
    [SerializeField] private float currentHealth = 0.0f;
    [SerializeField] private bool isDead = false;
    
    [System.Serializable]
    public class HealthChangedEvent : UnityEvent<float, float> { } // new health, max health
    public HealthChangedEvent onHealthChanged = new HealthChangedEvent();
    public UnityEvent onDeath;
    
    void Start()
    {
        currentHealth = maxHealth; 
        isDead = false;
        
        onHealthChanged?.Invoke(currentHealth, maxHealth); 
    }
    
    void Update()
    {
        // testing purposes 
        TakeDamage(Time.deltaTime); 
    }

    void TakeDamage(float damage)
    {
        if (isDead)
        {
            Debug.Log(name + " is already dead, ignoring " + damage + " damage"); 
            return;
        }
        var prevHealth = currentHealth;
        ChangeHealth(-damage);
        // Debug.Log(name + " takes " + damage + " damage" + $" [{prevHealth}->{currentHealth}]"); 
        if (currentHealth <= 0)
        {
            Die(); 
        }
    }
    
    void Heal(float heal)
    {
        if (isDead)
        {
            print(name + " is dead, ignoring " + heal + " healing");
            return;
        }
        var prevHealth = currentHealth;
        ChangeHealth(+heal);
        Debug.Log(name + " heals for " + heal + $" [{prevHealth}->{currentHealth}]");
    }
    
    private void ChangeHealth(float delta)
    {
        currentHealth += delta;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void Die()
    {
        isDead = true;
        // do any Dying logic here, or use this component to trigger death logic elsewhere.
        onDeath?.Invoke(); 
        Debug.Log(name + " died");
        Destroy(gameObject);
    }
}
