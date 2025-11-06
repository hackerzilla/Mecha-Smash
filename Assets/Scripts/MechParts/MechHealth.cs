using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MechHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    private bool isDead = false;
    public UnityEvent onDeath;

    private float damageMultiplier = 1.0f; // 1.0 = 100% damage
    private bool isInvincible = false;  // for Quantom Core

    void Awake()
    {
        currentHealth = maxHealth;
        onDeath = new UnityEvent();
    }
    
    void Update()
    {
        TakeDamage(25f * Time.deltaTime);
    }

    // ⭐️ Different scripts(bullet, ability stuff) call this method and give damage.
    public void TakeDamage(float amount)
    {
        if (isInvincible || amount <= 0 || isDead)
        {
            return;
        }

        float finalDamage = amount * damageMultiplier; // damage reduction
        currentHealth -= finalDamage;
        
        // Debug.Log(gameObject.name + "got" + finalDamage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Reactive Plating
    public void ApplyDamageReduction(float multiplier, float duration)
    {
        StartCoroutine(DamageReductionCoroutine(multiplier, duration));
    }

    private IEnumerator DamageReductionCoroutine(float multiplier, float duration)
    {
        damageMultiplier = multiplier; // start damage reduction
        Debug.Log("Invincible On : " + (1f - multiplier) * 100 + "%");
        
        yield return new WaitForSeconds(duration);
        
        damageMultiplier = 1.0f; // finish damage reduction
        Debug.Log("Invincible Off");
    }

    // Quantum Core
    public void SetInvincible(float duration)
    {
        StartCoroutine(InvincibleCoroutine(duration));
    }

    private IEnumerator InvincibleCoroutine(float duration)
    {
        isInvincible = true;
        Debug.Log("Incinvible On");
        
        yield return new WaitForSeconds(duration);
        
        isInvincible = false;
        Debug.Log("Invincible Off");
    }

    public void Die()
    {
        Debug.Log($"[{gameObject.name}] MechHealth.Die() called - Time: {Time.time}");
        Debug.Log($"[{gameObject.name}] onDeath listener count: {onDeath.GetPersistentEventCount()}");
        isDead = true;
        Debug.Log($"[{gameObject.name}] Invoking onDeath event...");
        onDeath.Invoke();
        Debug.Log($"[{gameObject.name}] onDeath event invoked");
        // Todo : die animation, game over logic, etc..
    }
}