using NUnit.Framework;
using UnityEngine;

/// <summary>
/// The abstract class that encapsulates mech part logic. Holds the animation controllers that 
/// (will) make the modular animation system function (eventually). This is a base class and shouldn't be used. 
/// </summary>
abstract public class MechPart : MonoBehaviour
{
    public Animator animator;
    public MechController mech;
    public PlayerController player;

    [SerializeField] protected string description;
    public string Description => description;

    // This function will run for all mech parts regardless of type.
    protected virtual void Awake()
    {
    }
}