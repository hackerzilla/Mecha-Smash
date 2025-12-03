using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

abstract public class TorsoPart : MechPart
{
    [SerializeField] protected Color vfxColor;
    [SerializeField] protected float cooldown = 1.0f;
    protected string AbilityName;
    protected float lastUseTime = -1.0f;
    protected Coroutine vfxRunning;
    [SerializeField] protected float vfxDuration = 1.5f;

    // Sprite management for skeleton rig system
    [Header("Sprite Configuration")]
    [SerializeField] protected GameObject chestSprite;
    [SerializeField] protected GameObject coreSprite;
    [SerializeField] protected GameObject leftShoulderSprite;
    [SerializeField] protected GameObject rightShoulderSprite;
    [SerializeField] protected List<GameObject> torsoSprites = new List<GameObject>();

    [Header("Sorting Order")]
    [SerializeField] protected int chestSortingOrder = 14;
    [SerializeField] protected int coreSortingOrder = 9;
    [SerializeField] protected int leftShoulderSortingOrder = 12;
    [SerializeField] protected int rightShoulderSortingOrder = 17;

    abstract public void DefensiveAbility(PlayerController player, InputAction.CallbackContext context);

    public virtual bool CanUseAbility()
    {
        return Time.time >= lastUseTime + cooldown;
    }

    protected void StartCooldown()
    {
        lastUseTime = Time.time;
    }
    abstract public void DefensiveAbility();

    /// <summary>
    /// Attaches the torso sprites to the skeleton rig at the specified attachment points.
    /// Called by MechController during part assembly.
    /// </summary>
    public virtual void AttachSprites(Transform chestAttachment, Transform coreAttachment,
                                       Transform leftShoulderAttachment, Transform rightShoulderAttachment)
    {
        if (chestSprite != null && chestAttachment != null)
        {
            chestSprite.transform.SetParent(chestAttachment);
            chestSprite.transform.localPosition = Vector2.zero;
            chestSprite.transform.localScale = Vector3.one;
            var sr = chestSprite.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = chestSortingOrder;
        }

        if (coreSprite != null && coreAttachment != null)
        {
            coreSprite.transform.SetParent(coreAttachment);
            coreSprite.transform.localPosition = Vector2.zero;
            coreSprite.transform.localScale = Vector3.one;
            var sr = coreSprite.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = coreSortingOrder;
        }

        if (leftShoulderSprite != null && leftShoulderAttachment != null)
        {
            leftShoulderSprite.transform.SetParent(leftShoulderAttachment);
            leftShoulderSprite.transform.localPosition = Vector2.zero;
            leftShoulderSprite.transform.localScale = Vector3.one;
            var sr = leftShoulderSprite.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = leftShoulderSortingOrder;
        }

        if (rightShoulderSprite != null && rightShoulderAttachment != null)
        {
            rightShoulderSprite.transform.SetParent(rightShoulderAttachment);
            rightShoulderSprite.transform.localPosition = Vector2.zero;
            rightShoulderSprite.transform.localScale = Vector3.one;
            var sr = rightShoulderSprite.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = rightShoulderSortingOrder;
        }
    }

    /// <summary>
    /// Cleanup sprites when this part is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (chestSprite != null) Destroy(chestSprite);
        if (coreSprite != null) Destroy(coreSprite);
        if (leftShoulderSprite != null) Destroy(leftShoulderSprite);
        if (rightShoulderSprite != null) Destroy(rightShoulderSprite);
    }

    /// <summary>
    /// Attaches the torso sprites to the skeleton rig at the specified attachment point.
    /// Called by MechController during part assembly.
    /// </summary>
    public virtual void AttachSprites(Transform torsoAttachment)
    {
        // Reparent all torso sprites to attachment point
        if (torsoAttachment != null)
        {
            foreach (GameObject sprite in torsoSprites)
            {
                if (sprite != null)
                {
                    sprite.transform.SetParent(torsoAttachment);
                    sprite.transform.localPosition = Vector2.zero;
                }
            }
        }
    }

    // NEW TORSO VFX CODE
    protected void TorsoDissolveVFX()
    {
        // Run VFX until time runs out
        if (vfxRunning != null)
            StopCoroutine(vfxRunning);

        StartCoroutine(TorsoDissolve());
    }

    protected IEnumerator TorsoDissolve()
    {
        MechController mechController = transform.root.GetComponent<PlayerController>()?.mechInstance;
        
        // Change Parameters here
        float t = 0f;
        float flashSpeed = 10f;
        float dissolveMin = 0.1f;
        float dissolveMax = 0.6f;
        float outlineMin = 0.02f;
        float outlineMax = 0.08f;

        if (mechController.torsoMaterialTemplate == null || mechController.skeletonRig == null)
            yield break;

        SpriteRenderer[] renderers = mechController.skeletonRig.GetComponentsInChildren<SpriteRenderer>(true);
        Material matInstance = new Material(mechController.torsoMaterialTemplate);
        while (t < vfxDuration)
        {
            float pulse = Mathf.Sin(Time.time * flashSpeed) * 0.5f + 0.5f;
            float dissolve = Mathf.Lerp(dissolveMin, dissolveMax, pulse);
            float outline = Mathf.Lerp(outlineMin, outlineMax, pulse);

            foreach (SpriteRenderer renderer in renderers)
            {
                
                matInstance.SetFloat("_DissolveAmount", dissolve);
                matInstance.SetFloat("_OutlineThickness", outline);
                matInstance.SetColor("_OutlineColor", vfxColor * 3);
                renderer.material = matInstance;
            }

            t += Time.deltaTime;
            yield return null;
        }

        // mechController.ApplyOutlineMaterial();
        vfxRunning = null;
    }
}
