using UnityEngine;

public class MechPartSlot : MonoBehaviour
{
    public GameObject currentPartPrefab;
    public GameObject currentPartInstance;
    void Start()
    {
        
    }

    void Update()
    {

    }

    /// <summary>
    /// Sets the part slot by instantiating a prefab. 
    /// </summary>
    /// <param name="partPrefab">Prefab of the mech part to set.</param>
    public void SetPart(GameObject partPrefab)
    {
        if (currentPartInstance != null)
        {
            Destroy(currentPartInstance);
        }
        currentPartInstance = Instantiate(partPrefab, transform);
        currentPartPrefab = partPrefab;
    }
}
