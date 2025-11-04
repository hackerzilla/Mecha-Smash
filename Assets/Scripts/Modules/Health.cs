using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100.0f;
    public float currentHealth = 0.0f;
    public bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth; 
        isDead = false;
    }
    
    void Update()
    {
        // testing purposes 
        TakeDamage(Time.deltaTime * 5); 
    }

    void TakeDamage(float damage)
    {
        if (isDead)
        {
            Debug.Log(name + " is already dead, ignoring " + damage + " damage"); 
            return;
        }
        var prevHealth = currentHealth;
        currentHealth -= damage;
        Debug.Log(name + " takes " + damage + " damage" + $" [{prevHealth}->{currentHealth}]"); 
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
        currentHealth += heal;
        Debug.Log(name + " heals for " + heal + $" [{prevHealth}->{currentHealth}]");
    }
    
    void Die()
    {
        isDead = true;
        // do any Dying logic here, or use this component to trigger death logic elsewhere.
        print(name + " died");
        Destroy(gameObject);
    }
}
