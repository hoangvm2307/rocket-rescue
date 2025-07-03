// File: Scripts/Audio/AudioCueEventChannelSO.cs
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Audio Cue Event Channel", fileName = "NewAudioCueEventChannel")]
public class AudioCueEventChannelSO : ScriptableObject
{
    // UnityAction<AudioCueSO, Vector3> cho âm thanh 3D
    public UnityAction<AudioCueSO, Vector3> OnAudioCueRequested3D;
    
    // UnityAction<AudioCueSO> cho âm thanh 2D (không cần vị trí)
    public UnityAction<AudioCueSO> OnAudioCueRequested2D;

    public void RaiseEvent(AudioCueSO audioCue, Vector3 position)
    {
        OnAudioCueRequested3D?.Invoke(audioCue, position);
    }
    
    public void RaiseEvent(AudioCueSO audioCue)
    {
        OnAudioCueRequested2D?.Invoke(audioCue);
    }
}