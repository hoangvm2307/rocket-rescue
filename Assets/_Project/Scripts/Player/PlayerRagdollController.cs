using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerRagdollController : MonoBehaviour
{
    [Header("Ragdoll Settings")]
    [SerializeField] private Transform hipBone; // Reference to the hip/pelvis bone
    [SerializeField] private Transform[] legBones; // Array of leg bone transforms (thighs, calves, feet)
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float blendSpeed = 5f;
 

    [Header("Landing Effects")]
    [Tooltip("Tốc độ rơi tối thiểu (theo chiều dọc) để kích hoạt hiệu ứng tiếp đất mạnh.")]
    [SerializeField] private float hardLandingVelocityThreshold = -5f;
    [Tooltip("Prefab hiệu ứng bụi sẽ được tạo ra khi tiếp đất mạnh.")]
    [SerializeField] private GameObject dustEffectPrefab;
    [Tooltip("Transform của model nhân vật sẽ bị co giãn.")]
    [SerializeField] private Transform playerVisualsTransform;
    private Rigidbody rootRigidbody; 
 
    private bool isGrounded; 
    private void Awake()
    { 
 

        rootRigidbody = GetComponent<Rigidbody>(); 

        if (playerVisualsTransform == null)
        {
            // Cố gắng tìm model trong các object con nếu không được gán
            playerVisualsTransform = transform.Find("Model"); // Giả sử tên model là "Model"
            if (playerVisualsTransform == null)
            {
                playerVisualsTransform = this.transform;
            }
        }
    }

   

 

    private void Update()
    {
        CheckGrounded(); 
    }

    private void CheckGrounded()
    {
        // Cast multiple rays from the hip bone to check for ground
        bool wasGrounded = isGrounded;
        isGrounded = false;

        if (hipBone != null)
        {
            // Cast rays in a small radius around the hip
            Vector3[] checkPoints = new Vector3[]
            {
                hipBone.position,
                hipBone.position + hipBone.right * 0.2f,
                hipBone.position - hipBone.right * 0.2f
            };

            foreach (Vector3 point in checkPoints)
            {
                if (Physics.Raycast(point, Vector3.down, groundCheckDistance, groundLayer))
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        // If we just landed, start blending back to animation
        if (!wasGrounded && isGrounded)
        {
            if (rootRigidbody != null && rootRigidbody.linearVelocity.y < hardLandingVelocityThreshold)
            {
                TriggerHardLandingEffects();
            }
        }
        wasGrounded = isGrounded;

    }
 
 
 
    private void OnDrawGizmos()
    {
        if (hipBone != null)
        {
            // Draw ground check rays
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector3[] checkPoints = new Vector3[]
            {
                hipBone.position,
                hipBone.position + hipBone.right * 0.2f,
                hipBone.position - hipBone.right * 0.2f
            };

            foreach (Vector3 point in checkPoints)
            {
                Gizmos.DrawLine(point, point + Vector3.down * groundCheckDistance);
            }
        }
    }

    private void TriggerHardLandingEffects()
    {
       

        // 3. Hiệu ứng Bụi
        if (dustEffectPrefab != null)
        {
            
            GameObject dust = Instantiate(dustEffectPrefab, new Vector3(hipBone.position.x, hipBone.position.y - 2f, hipBone.position.z), Quaternion.identity);
            Destroy(dust, 2f); // Hủy object sau 2 giây để dọn dẹp
        }
    }

}