// File: Scripts/Audio/BackgroundMusicPlayer.cs
using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    [Header("Event Channel")]
    [Tooltip("Kênh sự kiện để yêu cầu phát âm thanh")]
    [SerializeField] private AudioCueEventChannelSO bgmEventChannel;

    [Header("Music")]
    [Tooltip("Nhạc nền sẽ được phát khi bắt đầu")]
    [SerializeField] private AudioCueSO backgroundMusic;

    private void Start()
    {
        if (bgmEventChannel != null && backgroundMusic != null)
        {
            // Gửi yêu cầu phát âm thanh 2D (không cần vị trí)
            bgmEventChannel.RaiseEvent(backgroundMusic);
        }
    }
}