// File: Scripts/Audio/AudioCueSO.cs
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Audio Cue", fileName = "NewAudioCue")]
public class AudioCueSO : ScriptableObject
{
    [Header("Sound")]
    [SerializeField] private AudioClip[] audioClips; // Kéo các file âm thanh biến thể vào đây
    [SerializeField] private bool loop = false;

    [Header("Configuration")]
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    [SerializeField] [Range(0.1f, 3f)] private float pitch = 1f;
    
    [Header("Output")]
    [SerializeField] private AudioMixerGroup audioMixerGroup; // Kênh output (SFX, BGM, v.v.)
    
    public AudioClip GetRandomClip()
    {
        if (audioClips.Length == 0) return null;
        return audioClips[Random.Range(0, audioClips.Length)];
    }

    // Các getters để các script khác có thể đọc thông tin
    public bool IsLooping() => loop;
    public float GetVolume() => volume;
    public float GetPitch() => pitch;
    public AudioMixerGroup GetAudioMixerGroup() => audioMixerGroup;
}