using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// The abstract class that encapsulates mech part logic. Holds the animation controllers that 
/// (will) make the modular animation system function (eventually). This is a base class and shouldn't be used. 
/// </summary>
abstract public class MechPart : MonoBehaviour
{
    /// <summary>
    /// The base animation controller. Has the same state machine structure as all its sub-types. 
    /// This allows us to basically do polymorphism with animations.
    /// </summary>
    public AnimatorController baseAnimController = null;

    /// <summary>
    /// The override animation controller. This is what changes the specific part type animation. 
    /// For example, this is where the cyclops head will override the special attack animation with 
    /// a laser beam animation with the head slightly shaking. 
    /// </summary>
    public AnimatorOverrideController overrideAnimController = null;

    void Start()
    {
    }

    void Update()
    {

    }


}
