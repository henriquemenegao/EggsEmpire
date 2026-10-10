using UnityEngine;

public class EggBatch : MonoBehaviour
{
    public long Quantity { get; private set; }

    public void Initialize(long quantity)
    {
        Quantity = quantity;
    }
}