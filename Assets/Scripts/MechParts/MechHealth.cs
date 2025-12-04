using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MechHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;

    [System.Serializable]
    public class HealthChangedEvent : UnityEvent<float, float> { } // currentHealth, maxHealth

    public HealthChangedEvent onHealthChanged = new HealthChangedEvent();
    public UnityEvent onDeath;

    private float damageMultiplier = 1.0f; // 1.0 = 100% damage
    private bool isInvincible = false;  // for Quantum Core
    private Animator skeletonAnimator;
    
    [SerializeField] protected AudioSource hitmarkerSound1;
    [SerializeField] protected AudioSource hitmarkerSound2;

    [Header("Damage Flash VFX")]
    [SerializeField] private float flashDuration = 0.15f;
    private Coroutine flashCoroutine;

    void Awake()
    {
        currentHealth = maxHealth;
        onDeath = new UnityEvent();
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        // Get skeleton animator for flinch animation
        MechController mechController = GetComponent<MechController>();
        if (mechController != null && mechController.skeletonRig != null)
        {
            skeletonAnimator = mechController.skeletonRig.GetComponent<Animator>();
        }
    }
    
    // ⭐️ Different scripts(bullet, ability stuff) call this method and give damage.
    public void TakeDamage(float amount)
    {
        float rand = Random.Range(0.0f, 1.0f);
        if (rand > 0.5f)
        {
            hitmarkerSound1.Play(); 
        }
        else
        {
            hitmarkerSound2.Play();
        }
        if (isInvincible || amount <= 0 || isDead)
        {
            return;
        }

        float finalDamage = amount * damageMultiplier; // damage reduction
        ChangeHealth(-finalDamage);

        // Trigger damage flash VFX
        TriggerDamageFlash();

        // Trigger flinch animation
        if (skeletonAnimator != null)
        {
            skeletonAnimator.SetTrigger("flinch");
        }

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
        isDead = true;
        onDeath.Invoke();
        Destroy(gameObject);
        // Todo : die animation, game over logic, etc..
    }

    public void Heal(float amount)
    {
        if (isDead)
        {
            Debug.Log($"{name} is dead, ignoring {amount} healing");
            return;
        }

        ChangeHealth(amount);
        Debug.Log($"{name} heals for {amount} (Current: {currentHealth}/{maxHealth})");
    }

    private void ChangeHealth(float delta)
    {
        currentHealth += delta;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void TriggerDamageFlash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(DamageFlashVFX());
    }

    private IEnumerator DamageFlashVFX()
    {
        MechController mechController = GetComponent<MechController>();
        if (mechController?.damageFlashMaterial == null || mechController.skeletonRig == null)
            yield break;

        SpriteRenderer[] renderers = mechController.skeletonRig.GetComponentsInChildren<SpriteRenderer>(true);
        Material matInstance = new Material(mechController.damageFlashMaterial);

        float t = 0f;
        while (t < flashDuration)
        {
            // Start at full white (1), fade to normal (0)
            float flashAmount = Mathf.Lerp(1f, 0f, t / flashDuration);
            matInstance.SetFloat("_FlashAmount", flashAmount);

            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.material = matInstance;
            }

            t += Time.deltaTime;
            yield return null;
        }

        // Reset to default material
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer != null)
                renderer.material = mechController.defaultSpriteMaterial;
        }

        Destroy(matInstance);
        flashCoroutine = null;
    }
}