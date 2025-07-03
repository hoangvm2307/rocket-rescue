using UnityEngine;
using DG.Tweening;

public class Hostage : MonoBehaviour
{
    [SerializeField] private GameEvent onHostageRescued;

    [Header("Rescue Effects")]
    [SerializeField] private Transform visualsTransform;
    [SerializeField] private float rescueAnimationDuration = 1f;
    [SerializeField] private GameObject rescueEffect;

    private bool isRescued = false;

    void Awake()
    {
        if (visualsTransform == null)
            visualsTransform = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isRescued)
        {
            isRescued = true;
            StartRescueSequence();
        }
    }

    private void StartRescueSequence()
    {
     
        // DOTween rescue effects
        Sequence rescueSequence = DOTween.Sequence();

        // 1. Scale up with bounce
        rescueSequence.Append(visualsTransform.DOScale(Vector3.one * 1.2f, 0.3f).SetEase(Ease.OutBack));

        // 2. Rotate with excitement
        rescueSequence.Join(visualsTransform.DORotate(new Vector3(0, 360, 0), 0.6f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad));

        // 3. Float up slightly
        rescueSequence.Join(visualsTransform.DOLocalMoveY(visualsTransform.localPosition.y + 0.5f, 0.4f).SetEase(Ease.OutQuad));

        // 4. Scale down and fade out
        rescueSequence.Append(visualsTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));

        // 5. Create rescue effect
        if (rescueEffect != null)
        {
            rescueSequence.AppendCallback(() =>
            {
                Instantiate(rescueEffect, transform.position, Quaternion.identity);
            });
        }

        // 6. Trigger win condition check and destroy hostage
        rescueSequence.AppendCallback(() =>
        {
            onHostageRescued?.Raise();
            Destroy(gameObject);
        });

        rescueSequence.Play();
    }
}