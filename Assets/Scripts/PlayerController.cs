using NUnit.Framework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MechController mechPrefab;
    public MechController mechInstance;
    void Start()
    {
        if (mechInstance == null)
        {
            Assert.NotNull(mechPrefab);
            mechInstance = Instantiate(mechPrefab, this.transform);
        }
        Assert.NotNull(mechInstance);
    }

    void Update()
    {
         
    }
}
