// File: PlayerMotor.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = 
            RigidbodyConstraints.FreezePositionZ | 
            RigidbodyConstraints.FreezeRotationX | 
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;
    }

    public void ApplyRocketJump(Vector3 explosionPosition, float force)
    {
        Vector3 direction = (transform.position - explosionPosition).normalized;
        direction.z = 0;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
}