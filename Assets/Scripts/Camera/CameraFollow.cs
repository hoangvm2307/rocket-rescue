using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Vị trí của Player
    public float smoothSpeed = 0.125f; // Tốc độ camera di chuyển
    public Vector3 offset; // Khoảng cách giữa camera và player

    void Start()
    {
        // Tự động tìm Player nếu target chưa được gán
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
            else
            {
                Debug.LogError("CameraFollow: Không tìm thấy đối tượng có tag 'Player'. Hãy chắc chắn rằng Player đã được gán tag 'Player'.");
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}