using UnityEngine;
using System.Collections.Generic;

namespace TerracottaARPG.Systems
{
    /// <summary>
    /// 音频中心：统一管理所有游戏音效和音乐
    /// 支持动态音量、音效池、空间音效等
    /// </summary>
    public class AudioHub : MonoBehaviour
    {
        private static AudioHub _instance;
        public static AudioHub Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("AudioHub");
                    _instance = go.AddComponent<AudioHub>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private int poolSize = 10;

        [Header("Volumes")]
        [Range(0, 1)] public float masterVolume = 1f;
        [Range(0, 1)] public float musicVolume = 0.7f;
        [Range(0, 1)] public float sfxVolume = 1f;

        private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
        private List<AudioSource> _sfxPool = new List<AudioSource>();
        private int _currentPoolIndex = 0;

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
            InitializeSFXPool();
        }

        private void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicGO = new GameObject("MusicSource");
                musicGO.transform.SetParent(transform);
                musicSource = musicGO.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxGO = new GameObject("SFXSource");
                sfxGO.transform.SetParent(transform);
                sfxSource = sfxGO.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }
        }

        private void InitializeSFXPool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject poolGO = new GameObject($"SFXPool_{i}");
                poolGO.transform.SetParent(transform);
                AudioSource source = poolGO.AddComponent<AudioSource>();
                source.playOnAwake = false;
                _sfxPool.Add(source);
            }
        }

        /// <summary>
        /// 播放音效（简化版，用于原型）
        /// </summary>
        public static void Play(string key)
        {
            if (Instance == null) return;

            // 原型阶段：仅打印日志
            // 正式版：从Resources或Addressables加载音效
            Debug.Log($"[AudioHub] Play SFX: {key}");

            // TODO: 实际播放逻辑
            // AudioClip clip = GetClip(key);
            // if (clip != null)
            //     Instance.PlaySFX(clip);
        }

        /// <summary>
        /// 播放音效（带音量控制）
        /// </summary>
        public static void Play(string key, float volume)
        {
            if (Instance == null) return;
            Debug.Log($"[AudioHub] Play SFX: {key} (Volume: {volume})");
        }

        /// <summary>
        /// 播放空间音效（3D音效，带位置）
        /// </summary>
        public static void PlayAtPosition(string key, Vector3 position, float volume = 1f)
        {
            if (Instance == null) return;
            Debug.Log($"[AudioHub] Play SFX at {position}: {key}");
        }

        /// <summary>
        /// 播放音效（使用音效池）
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null) return;

            AudioSource source = GetNextAvailableSource();
            source.clip = clip;
            source.volume = sfxVolume * masterVolume * volumeScale;
            source.Play();
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

        /// <summary>
        /// 淡入淡出音乐
        /// </summary>
        public void FadeMusic(float targetVolume, float duration)
        {
            if (musicSource != null)
                StartCoroutine(FadeMusicCoroutine(targetVolume, duration));
        }

        private System.Collections.IEnumerator FadeMusicCoroutine(float targetVolume, float duration)
        {
            float startVolume = musicSource.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, targetVolume * musicVolume * masterVolume, elapsed / duration);
                yield return null;
            }

            musicSource.volume = targetVolume * musicVolume * masterVolume;
        }

        private AudioSource GetNextAvailableSource()
        {
            // 轮询使用音效池
            AudioSource source = _sfxPool[_currentPoolIndex];
            _currentPoolIndex = (_currentPoolIndex + 1) % _sfxPool.Count;
            return source;
        }

        private AudioClip GetClip(string key)
        {
            if (_audioClips.TryGetValue(key, out AudioClip clip))
                return clip;

            // 尝试从Resources加载
            clip = Resources.Load<AudioClip>($"Audio/{key}");
            if (clip != null)
            {
                _audioClips[key] = clip;
                return clip;
            }

            Debug.LogWarning($"[AudioHub] Audio clip not found: {key}");
            return null;
        }

        /// <summary>
        /// 预加载音效
        /// </summary>
        public void PreloadClip(string key)
        {
            if (!_audioClips.ContainsKey(key))
            {
                AudioClip clip = Resources.Load<AudioClip>($"Audio/{key}");
                if (clip != null)
                    _audioClips[key] = clip;
            }
        }

        /// <summary>
        /// 批量预加载音效
        /// </summary>
        public void PreloadClips(string[] keys)
        {
            foreach (string key in keys)
                PreloadClip(key);
        }
    }
}
