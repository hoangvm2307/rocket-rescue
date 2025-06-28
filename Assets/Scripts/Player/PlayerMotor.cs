// File: PlayerMotor.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Rigidbody rb;
    [Tooltip("How much to favor vertical movement. 0 = pure physics, 1 = strong bias upwards.")]
    [Range(0, 1)]
    [SerializeField] private float verticalBias = 0.3f;

    [Tooltip("Ensures a minimum upward push, even if the explosion is level with the player. Prevents horizontal-only movement.")]
    [SerializeField] private float minimumUpwardNudge = 0.2f;

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
        Vector3 directionVector = transform.position - explosionPosition;
        
        if (directionVector.y < minimumUpwardNudge)
        {
            directionVector.y = minimumUpwardNudge;
        }

        Vector3 originalDirection = directionVector.normalized;
        originalDirection.z = 0;  
        float verticality = Vector3.Dot(originalDirection, Vector3.up);
        verticality = Mathf.Clamp01(verticality);  
 
        Vector3 biasedDirection = Vector3.Lerp(originalDirection, Vector3.up, verticality * verticalBias);

        rb.AddForce(biasedDirection * force, ForceMode.Impulse);
    }
}