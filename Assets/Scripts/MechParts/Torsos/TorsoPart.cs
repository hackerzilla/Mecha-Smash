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
    [SerializeField] protected List<GameObject> torsoSprites = new List<GameObject>();

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

    /// <summary>
    /// Cleanup sprites when this part is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        foreach (GameObject sprite in torsoSprites)
        {
            if (sprite != null)
            {
                Destroy(sprite);
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

        mechController.ApplyOutlineMaterial();
        vfxRunning = null;
    }
}
