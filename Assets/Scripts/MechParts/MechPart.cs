using NUnit.Framework;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// The abstract class that encapsulates mech part logic. Holds the animation controllers that 
/// (will) make the modular animation system function (eventually). This is a base class and shouldn't be used. 
/// </summary>
abstract public class MechPart : MonoBehaviour
{
    // I realized that I don't actually need to track these references since they are contained in the animator.
    /// <summary>
    /// The base animation controller. Has the same state machine structure as all its sub-types. 
    /// This allows us to basically do polymorphism with animations.
    /// </summary>
    // public AnimatorController baseAnimController = null;

    /// <summary>
    /// The override animation controller. This is what changes the specific part type animation. 
    /// For example, this is where the cyclops head will override the special attack animation with 
    /// a laser beam animation with the head slightly shaking. 
    /// </summary>
    // public AnimatorOverrideController overrideAnimController = null;

    public Animator animator; 
    public MechController mech;
    public PlayerController player;

    // This function will run for all mech parts regardless of type.
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        Assert.NotNull(animator);
    }
    // protected void OnEnable()
    // {
    //     mech = GetComponent<MechController>();
    //     player = GetComponentInParent<PlayerController>();
    // }
}