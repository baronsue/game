using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 简化版音频系统（无命名空间）
/// 管理所有游戏音效和音乐
/// </summary>
public class SimpleAudioHub : MonoBehaviour
{
    private static SimpleAudioHub _instance;
    public static SimpleAudioHub Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SimpleAudioHub");
                _instance = go.AddComponent<SimpleAudioHub>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Volumes")]
    [Range(0, 1)] public float masterVolume = 1f;
    [Range(0, 1)] public float musicVolume = 0.7f;
    [Range(0, 1)] public float sfxVolume = 1f;

    private List<AudioSource> _sfxPool = new List<AudioSource>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSources();

        Debug.Log("✅ SimpleAudioHub 初始化成功！");
    }

    private void InitializeAudioSources()
    {
        // 创建音乐源
        if (musicSource == null)
        {
            GameObject musicGO = new GameObject("MusicSource");
            musicGO.transform.SetParent(transform);
            musicSource = musicGO.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        // 创建音效源
        if (sfxSource == null)
        {
            GameObject sfxGO = new GameObject("SFXSource");
            sfxGO.transform.SetParent(transform);
            sfxSource = sfxGO.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        // 创建音效池
        for (int i = 0; i < 5; i++)
        {
            GameObject poolGO = new GameObject($"SFXPool_{i}");
            poolGO.transform.SetParent(transform);
            AudioSource source = poolGO.AddComponent<AudioSource>();
            source.playOnAwake = false;
            _sfxPool.Add(source);
        }
    }

    /// <summary>
    /// 播放音效（简化版）
    /// </summary>
    public static void Play(string soundName)
    {
        if (Instance == null) return;

        // 原型阶段：仅打印日志
        Debug.Log($"[🔊 音效] {soundName}");

        // TODO: 实际播放音效
        // AudioClip clip = Resources.Load<AudioClip>($"Audio/{soundName}");
        // if (clip != null) Instance.PlaySFX(clip);
    }

    /// <summary>
    /// 播放音效（带音量）
    /// </summary>
    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume * volumeScale);
    }

    /// <summary>
    /// 播放音乐
    /// </summary>
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume * masterVolume;
        musicSource.Play();
    }

    /// <summary>
    /// 停止音乐
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
}
