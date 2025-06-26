
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shield : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasBeenHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    /// <summary>
    /// Được gọi bởi RocketController khi rocket va chạm.
    /// </summary>
    public void HandleHit()
    {
        if (hasBeenHit) return;
        hasBeenHit = true;

        transform.SetParent(null);

        rb.isKinematic = false;
        
        rb.AddRelativeTorque(Random.insideUnitSphere * 5f);
    }
}