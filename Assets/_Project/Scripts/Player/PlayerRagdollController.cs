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

    [Header("Joint Limits")]
    [SerializeField] private float legSwingLimit = 45f; // Maximum angle legs can swing
    [SerializeField] private float legTwistLimit = 15f; // Maximum twist angle for legs

    [Header("Landing Effects")]
    [Tooltip("Tốc độ rơi tối thiểu (theo chiều dọc) để kích hoạt hiệu ứng tiếp đất mạnh.")]
    [SerializeField] private float hardLandingVelocityThreshold = -5f;
    [Tooltip("Prefab hiệu ứng bụi sẽ được tạo ra khi tiếp đất mạnh.")]
    [SerializeField] private GameObject dustEffectPrefab;
    [Tooltip("Transform của model nhân vật sẽ bị co giãn.")]
    [SerializeField] private Transform playerVisualsTransform;
    private Rigidbody rootRigidbody; 

    private Dictionary<Transform, ConfigurableJoint> boneToJoint;
    private Dictionary<Transform, Quaternion> initialRotations;
    private bool isGrounded;
    private float currentBlend;

    private void Awake()
    {
        boneToJoint = new Dictionary<Transform, ConfigurableJoint>();
        initialRotations = new Dictionary<Transform, Quaternion>();

        // SetupRagdoll();

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

    private void SetupRagdoll()
    {
        // Store initial rotations
        foreach (Transform bone in legBones)
        {
            initialRotations[bone] = bone.localRotation;
        }

        // Setup joints for each leg bone
        foreach (Transform bone in legBones)
        {
            // Add rigidbody if not present
            Rigidbody rb = bone.gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = bone.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = false; // We'll control this manually
                rb.linearDamping = 1f;
                rb.angularDamping = 1f;
            }

            // Add ConfigurableJoint
            ConfigurableJoint joint = bone.gameObject.GetComponent<ConfigurableJoint>();
            if (joint == null)
            {
                joint = bone.gameObject.AddComponent<ConfigurableJoint>();
                SetupJointLimits(joint);
            }

            boneToJoint[bone] = joint;
        }

        // Initially disable ragdoll
        SetRagdollState(false);
    }

    private void SetupJointLimits(ConfigurableJoint joint)
    {
        // Configure joint limits
        joint.configuredInWorldSpace = false;
        joint.anchor = Vector3.zero;

        // Set movement limits
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = legSwingLimit;
        joint.lowAngularXLimit = limit;
        joint.highAngularXLimit = limit;

        limit.limit = legTwistLimit;
        joint.angularYLimit = limit;
        joint.angularZLimit = limit;

        // Set joint drives
        JointDrive drive = new JointDrive();
        drive.positionSpring = 1000f;
        drive.positionDamper = 100f;
        drive.maximumForce = 1000f;

        joint.angularXDrive = drive;
        joint.angularYZDrive = drive;
    }

    private void Update()
    {
        CheckGrounded();
        UpdateRagdollState();
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

    private void UpdateRagdollState()
    {
        if (!isGrounded)
        {
            // When in air, enable ragdoll and update blend
            SetRagdollState(true);
            currentBlend = Mathf.MoveTowards(currentBlend, 1f, Time.deltaTime * blendSpeed);
        }
        else
        {
            // When grounded, blend back to animation
            currentBlend = Mathf.MoveTowards(currentBlend, 0f, Time.deltaTime * blendSpeed);
            if (currentBlend <= 0f)
            {
                SetRagdollState(false);
            }
        }

        // Apply the blend
        foreach (Transform bone in legBones)
        {
            if (boneToJoint.TryGetValue(bone, out ConfigurableJoint joint))
            {
                UpdateJointTargetRotation(joint, bone);
            }
        }
    }

    private void SetRagdollState(bool enabled)
    {
        foreach (Transform bone in legBones)
        {
            if (boneToJoint.TryGetValue(bone, out ConfigurableJoint joint))
            {
                Rigidbody rb = bone.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = enabled;
                    rb.isKinematic = !enabled;
                }
            }
        }
    }

    private void StartBlendToAnimation()
    {
        // Store the current ragdoll pose
        foreach (Transform bone in legBones)
        {
            if (boneToJoint.TryGetValue(bone, out ConfigurableJoint joint))
            {
                // Update the target rotation to match current ragdoll pose
                UpdateJointTargetRotation(joint, bone);
            }
        }
    }

    private void UpdateJointTargetRotation(ConfigurableJoint joint, Transform bone)
    {
        if (initialRotations.TryGetValue(bone, out Quaternion initialRotation))
        {
            // Get the animated rotation from the current animation state
            Quaternion animatedRotation = bone.localRotation;

            // Blend between ragdoll and animated rotation
            Quaternion targetRotation = Quaternion.Lerp(animatedRotation, initialRotation, currentBlend);

            // Convert to joint space
            joint.targetRotation = Quaternion.Inverse(targetRotation) * initialRotation;
        }
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