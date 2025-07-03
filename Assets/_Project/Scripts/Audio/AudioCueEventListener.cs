// File: Scripts/Audio/AudioCueEventListener.cs
using UnityEngine;

public class AudioCueEventListener : MonoBehaviour
{
    [Header("Event Channels to Listen To")]
    // Thay đổi thành một mảng để có thể lắng nghe nhiều kênh
    [SerializeField] private AudioCueEventChannelSO[] eventChannels;

    private void OnEnable()
    {
        if (eventChannels != null)
        {
            foreach (var channel in eventChannels)
            {
                channel.OnAudioCueRequested3D += PlayAudio3D;
                channel.OnAudioCueRequested2D += PlayAudio2D;
            }
        }
    }

    private void OnDisable()
    {
        if (eventChannels != null)
        {
            foreach (var channel in eventChannels)
            {
                channel.OnAudioCueRequested3D -= PlayAudio3D;
                channel.OnAudioCueRequested2D -= PlayAudio2D;
            }
        }
    }

    private void PlayAudio3D(AudioCueSO audioCue, Vector3 position)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(audioCue, position);
        }
    }
    
    private void PlayAudio2D(AudioCueSO audioCue)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(audioCue);
        }
    }
}