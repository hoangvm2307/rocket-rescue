// File: Scripts/Audio/AudioManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source Pool")]
    [SerializeField] private int initialPoolSize = 10;
    private Queue<AudioSource> audioSourcePool;
    private Transform poolParent;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Giữ AudioManager tồn tại giữa các scene

        InitializePool();
    }

    private void InitializePool()
    {
        audioSourcePool = new Queue<AudioSource>();
        poolParent = new GameObject("AudioSourcePool").transform;
        poolParent.SetParent(this.transform);

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateAndPoolAudioSource();
        }
    }

    private AudioSource CreateAndPoolAudioSource()
    {
        GameObject newAudioSourceGO = new GameObject("PooledAudioSource");
        newAudioSourceGO.transform.SetParent(poolParent);
        AudioSource newAudioSource = newAudioSourceGO.AddComponent<AudioSource>();
        newAudioSource.playOnAwake = false;
        newAudioSourceGO.SetActive(false); // Ban đầu tắt đi
        audioSourcePool.Enqueue(newAudioSource);
        return newAudioSource;
    }

    private AudioSource GetAudioSourceFromPool()
    {
        if (audioSourcePool.Count > 0)
        {
            return audioSourcePool.Dequeue();
        }
        // Nếu bể hết, tạo thêm một cái mới
        Debug.LogWarning("Audio source pool exhausted. Creating a new one.");
        return CreateAndPoolAudioSource();
    }

    public void PlaySound(AudioCueSO audioCue, Vector3 position)
    {
        AudioSource source = GetAudioSourceFromPool();
        
        source.transform.position = position;
        ConfigureSource(source, audioCue);
        
        source.gameObject.SetActive(true);
        source.Play();
        
        StartCoroutine(ReturnToPoolAfterPlayback(source, source.clip.length));
    }
    
    // Hàm này dành cho âm thanh 2D như UI, BGM
    public void PlaySound(AudioCueSO audioCue)
    {
        AudioSource source = GetAudioSourceFromPool();
        
        source.transform.position = Vector3.zero; // Hoặc vị trí camera
        ConfigureSource(source, audioCue);
        source.spatialBlend = 0f; // 2D sound
        
        source.gameObject.SetActive(true);
        source.Play();
        
        if (!audioCue.IsLooping())
        {
            StartCoroutine(ReturnToPoolAfterPlayback(source, source.clip.length));
        }
    }
    
    private void ConfigureSource(AudioSource source, AudioCueSO audioCue)
    {
        source.clip = audioCue.GetRandomClip();
        source.volume = audioCue.GetVolume();
        source.pitch = audioCue.GetPitch();
        source.loop = audioCue.IsLooping();
        source.outputAudioMixerGroup = audioCue.GetAudioMixerGroup();
        source.spatialBlend = 1f; // 3D sound by default
    }

    private IEnumerator ReturnToPoolAfterPlayback(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.gameObject.SetActive(false);
        audioSourcePool.Enqueue(source);
    }
}