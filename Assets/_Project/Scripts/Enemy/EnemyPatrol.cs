using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyPatrol : BaseEnemy
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float rotationSpeed = 5f;
    [Tooltip("How close the enemy needs to be to a point to consider it 'arrived'.")]
    [SerializeField] private float arrivalThreshold = 0.5f;
    [Tooltip("The angle in degrees. The enemy will only walk if it's facing the target within this angle.")]
    [SerializeField] private float walkAngleThreshold = 15f;

    private int currentPointIndex = 0; 


    void Update()
    {
        HandlePatrol();
    }

    private void HandlePatrol()
    {
        if (patrolPoints.Length == 0)
        {
            animator.SetBool("isWalking", false);
            return;
        }

        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector3 directionToTarget = targetPoint.position - transform.position;
        directionToTarget.y = 0; 
        if (directionToTarget.magnitude < arrivalThreshold)
        {
            // Lấy tên của điểm vừa đến để log
            string arrivedPointName = patrolPoints[currentPointIndex].name;

            // Chuyển sang điểm tiếp theo
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            targetPoint = patrolPoints[currentPointIndex];



            // Cập nhật lại hướng tới mục tiêu mới
            directionToTarget = (targetPoint.position - transform.position).normalized;
            directionToTarget.y = 0;
        }

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        bool isCurrentlyWalking = animator.GetBool("isWalking");

        if (angleToTarget < walkAngleThreshold)
        {

            animator.SetBool("isWalking", true);
        }
        else
        {

            animator.SetBool("isWalking", false);
        }
    }
}