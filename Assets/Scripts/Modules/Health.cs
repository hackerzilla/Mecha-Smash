using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100.0f;
    public float health = 0.0f;
    public bool isDead = false;
    
    void Start()
    {
        health = maxHealth; 
        isDead = false;
    }
    
    void Update()
    {
        // testing purposes 
        // TakeDamage(Time.deltaTime * 20); 
    }

    void TakeDamage(float damage)
    {
        if (isDead)
        {
            Debug.Log(name + " is already dead, ignoring " + damage + " damage"); 
            return;
        }
        var prevHealth = health;
        health -= damage;
        Debug.Log(name + " takes " + damage + " damage" + $" [{prevHealth}->{health}]"); 
        if (health <= 0)
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
        var prevHealth = health;
        health += heal;
        Debug.Log(name + " heals for " + heal + $" [{prevHealth}->{health}]");
    }
    
    void Die()
    {
        isDead = true;
        // do any Dying logic here, or use this component to trigger death logic elsewhere.
        print(name + " died");
        Destroy(gameObject);
    }
}
