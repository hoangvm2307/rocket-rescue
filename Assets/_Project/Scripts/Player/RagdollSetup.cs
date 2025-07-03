using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class RagdollSetup : EditorWindow
{
    private GameObject character;
    private Transform hipBone;
    private Transform leftThighBone;
    private Transform rightThighBone;
    private Transform leftCalfBone;
    private Transform rightCalfBone;
    
    private const string RAGDOLL_LAYER_NAME = "RagdollBones";
    private int ragdollLayer;

    [MenuItem("Tools/Rocket Rescue/Setup Leg Ragdoll")]
    public static void ShowWindow()
    {
        GetWindow<RagdollSetup>("Ragdoll Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Character Ragdoll Setup", EditorStyles.boldLabel);
        
        character = (GameObject)EditorGUILayout.ObjectField("Character", character, typeof(GameObject), true);
        hipBone = (Transform)EditorGUILayout.ObjectField("Hip Bone", hipBone, typeof(Transform), true);
        leftThighBone = (Transform)EditorGUILayout.ObjectField("Left Thigh", leftThighBone, typeof(Transform), true);
        rightThighBone = (Transform)EditorGUILayout.ObjectField("Right Thigh", rightThighBone, typeof(Transform), true);
        leftCalfBone = (Transform)EditorGUILayout.ObjectField("Left Calf", leftCalfBone, typeof(Transform), true);
        rightCalfBone = (Transform)EditorGUILayout.ObjectField("Right Calf", rightCalfBone, typeof(Transform), true);

        if (GUILayout.Button("Setup Ragdoll"))
        {
            SetupRagdoll();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "This tool will:\n" +
            "1. Setup ragdoll components\n" +
            "2. Create RagdollBones layer\n" +
            "3. Configure collision matrix\n" +
            "4. Setup ignore collisions between connected bones", 
            MessageType.Info);
    }

    void SetupRagdoll()
    {
        if (!ValidateSetup()) return;

        // Create and setup layer
        CreateRagdollLayer();
        
        // Setup bones
        SetupBone(hipBone, null, 10f, new Vector3(0.3f, 0.2f, 0.3f));
        SetupBone(leftThighBone, hipBone, 3.5f, new Vector3(0.1f, 0.3f, 0.1f));
        SetupBone(rightThighBone, hipBone, 3.5f, new Vector3(0.1f, 0.3f, 0.1f));
        SetupBone(leftCalfBone, leftThighBone, 2f, new Vector3(0.08f, 0.3f, 0.08f));
        SetupBone(rightCalfBone, rightThighBone, 2f, new Vector3(0.08f, 0.3f, 0.08f));

        // Setup collision ignores
        SetupCollisionIgnores();

        Debug.Log("Ragdoll setup completed!");
    }

    void CreateRagdollLayer()
    {
        ragdollLayer = LayerMask.NameToLayer(RAGDOLL_LAYER_NAME);
        if (ragdollLayer == -1)
        {
            // Find first free layer
            for (int i = 8; i <= 31; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (string.IsNullOrEmpty(layerName))
                {
                    // Create the layer
                    SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                    SerializedProperty layers = tagManager.FindProperty("layers");
                    layers.GetArrayElementAtIndex(i).stringValue = RAGDOLL_LAYER_NAME;
                    tagManager.ApplyModifiedProperties();
                    ragdollLayer = i;
                    break;
                }
            }
        }

        // Disable collisions within the ragdoll layer
        Physics.IgnoreLayerCollision(ragdollLayer, ragdollLayer, true);
    }

    void SetupBone(Transform bone, Transform connectedBone, float mass, Vector3 colliderSize)
    {
        Undo.RecordObject(bone.gameObject, "Setup Ragdoll");

        // Set layer
        bone.gameObject.layer = ragdollLayer;

        // Setup Rigidbody
        Rigidbody rb = bone.gameObject.GetComponent<Rigidbody>();
        if (rb == null) rb = bone.gameObject.AddComponent<Rigidbody>();
        
        rb.mass = mass;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Setup Collider
        CapsuleCollider collider = bone.gameObject.GetComponent<CapsuleCollider>();
        if (collider == null) collider = bone.gameObject.AddComponent<CapsuleCollider>();
        
        collider.radius = Mathf.Min(colliderSize.x, colliderSize.z) * 0.5f;
        collider.height = colliderSize.y;
        collider.direction = 1; // Y-axis
        collider.center = Vector3.zero;

        // Setup Joint (except for hip)
        if (connectedBone != null)
        {
            CharacterJoint joint = bone.gameObject.GetComponent<CharacterJoint>();
            if (joint == null) joint = bone.gameObject.AddComponent<CharacterJoint>();
            
            joint.connectedBody = connectedBone.GetComponent<Rigidbody>();
            joint.enablePreprocessing = false; // Prevents jittering

            // Configure Limits
            SoftJointLimit limit = new SoftJointLimit();
            
            // Twist limits
            limit.limit = 5f;
            joint.lowTwistLimit = limit;
            limit.limit = 10f;
            joint.highTwistLimit = limit;

            // Swing limits
            limit.limit = 45f;
            joint.swing1Limit = limit;
            limit.limit = 10f;
            joint.swing2Limit = limit;

            // Configure drives for smoother motion
            joint.swingAxis = Vector3.right;
            joint.axis = Vector3.forward;

            // Setup collision ignore between connected bodies
            if (joint.connectedBody != null)
            {
                Collider thisCollider = bone.GetComponent<Collider>();
                Collider connectedCollider = connectedBone.GetComponent<Collider>();
                if (thisCollider != null && connectedCollider != null)
                {
                    Physics.IgnoreCollision(thisCollider, connectedCollider, true);
                }
            }
        }
    }

    void SetupCollisionIgnores()
    {
        // Ignore collisions between connected bones
        IgnoreCollisionBetween(hipBone, leftThighBone);
        IgnoreCollisionBetween(hipBone, rightThighBone);
        IgnoreCollisionBetween(leftThighBone, leftCalfBone);
        IgnoreCollisionBetween(rightThighBone, rightCalfBone);
    }

    void IgnoreCollisionBetween(Transform bone1, Transform bone2)
    {
        Collider collider1 = bone1.GetComponent<Collider>();
        Collider collider2 = bone2.GetComponent<Collider>();
        if (collider1 != null && collider2 != null)
        {
            Physics.IgnoreCollision(collider1, collider2, true);
        }
    }

    bool ValidateSetup()
    {
        if (character == null || hipBone == null || 
            leftThighBone == null || rightThighBone == null ||
            leftCalfBone == null || rightCalfBone == null)
        {
            Debug.LogError("Please assign all required bones!");
            return false;
        }
        return true;
    }
}
#endif 