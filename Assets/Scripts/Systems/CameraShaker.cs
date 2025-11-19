using UnityEngine;
using System.Collections;

namespace TerracottaARPG.Systems
{
    /// <summary>
    /// 镜头震动系统：提供各种镜头震动效果
    /// 支持简单震动、衰减震动、方向性震动等
    /// </summary>
    public class CameraShaker : MonoBehaviour
    {
        private static CameraShaker _instance;
        public static CameraShaker Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("CameraShaker");
                    _instance = go.AddComponent<CameraShaker>();
                }
                return _instance;
            }
        }

        [Header("Settings")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float maxShakeDistance = 2f;

        private Vector3 _originalPosition;
        private Coroutine _currentShake;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera != null)
                _originalPosition = mainCamera.transform.localPosition;
        }

        /// <summary>
        /// 简单震动
        /// </summary>
        public static void Shake(float duration, float amplitude)
        {
            if (Instance != null)
                Instance.DoShake(duration, amplitude);
        }

        /// <summary>
        /// 衰减震动（强度逐渐减弱）
        /// </summary>
        public static void ShakeWithDecay(float duration, float amplitude, float decay = 2f)
        {
            if (Instance != null)
                Instance.DoShakeWithDecay(duration, amplitude, decay);
        }

        /// <summary>
        /// 方向性震动
        /// </summary>
        public static void ShakeDirectional(float duration, float amplitude, Vector2 direction)
        {
            if (Instance != null)
                Instance.DoShakeDirectional(duration, amplitude, direction);
        }

        /// <summary>
        /// 冲击波震动（快速强力震动）
        /// </summary>
        public static void ImpulseShake(float amplitude)
        {
            if (Instance != null)
                Instance.DoImpulseShake(amplitude);
        }

        private void DoShake(float duration, float amplitude)
        {
            if (_currentShake != null)
                StopCoroutine(_currentShake);

            _currentShake = StartCoroutine(ShakeCoroutine(duration, amplitude));
        }

        private void DoShakeWithDecay(float duration, float amplitude, float decay)
        {
            if (_currentShake != null)
                StopCoroutine(_currentShake);

            _currentShake = StartCoroutine(ShakeWithDecayCoroutine(duration, amplitude, decay));
        }

        private void DoShakeDirectional(float duration, float amplitude, Vector2 direction)
        {
            if (_currentShake != null)
                StopCoroutine(_currentShake);

            _currentShake = StartCoroutine(ShakeDirectionalCoroutine(duration, amplitude, direction));
        }

        private void DoImpulseShake(float amplitude)
        {
            if (_currentShake != null)
                StopCoroutine(_currentShake);

            _currentShake = StartCoroutine(ImpulseShakeCoroutine(amplitude));
        }

        private IEnumerator ShakeCoroutine(float duration, float amplitude)
        {
            if (mainCamera == null) yield break;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                // 使用Perlin噪声生成平滑的随机震动
                float x = (Mathf.PerlinNoise(Time.time * 10f, 0f) - 0.5f) * 2f * amplitude;
                float y = (Mathf.PerlinNoise(0f, Time.time * 10f) - 0.5f) * 2f * amplitude;

                Vector3 offset = new Vector3(x, y, 0f);
                offset = Vector3.ClampMagnitude(offset, maxShakeDistance);

                mainCamera.transform.localPosition = _originalPosition + offset;

                elapsed += Time.deltaTime;
                yield return null;
            }

            mainCamera.transform.localPosition = _originalPosition;
            _currentShake = null;
        }

        private IEnumerator ShakeWithDecayCoroutine(float duration, float amplitude, float decay)
        {
            if (mainCamera == null) yield break;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                float progress = elapsed / duration;
                float currentAmplitude = amplitude * Mathf.Pow(1f - progress, decay);

                float x = (Mathf.PerlinNoise(Time.time * 10f, 0f) - 0.5f) * 2f * currentAmplitude;
                float y = (Mathf.PerlinNoise(0f, Time.time * 10f) - 0.5f) * 2f * currentAmplitude;

                Vector3 offset = new Vector3(x, y, 0f);
                offset = Vector3.ClampMagnitude(offset, maxShakeDistance);

                mainCamera.transform.localPosition = _originalPosition + offset;

                elapsed += Time.deltaTime;
                yield return null;
            }

            mainCamera.transform.localPosition = _originalPosition;
            _currentShake = null;
        }

        private IEnumerator ShakeDirectionalCoroutine(float duration, float amplitude, Vector2 direction)
        {
            if (mainCamera == null) yield break;

            float elapsed = 0f;
            direction.Normalize();

            while (elapsed < duration)
            {
                float progress = elapsed / duration;
                float currentAmplitude = amplitude * Mathf.Pow(1f - progress, 2f);

                // 沿特定方向震动
                float noise = Mathf.PerlinNoise(Time.time * 15f, 0f) - 0.5f;
                Vector3 offset = new Vector3(direction.x, direction.y, 0f) * noise * currentAmplitude * 2f;
                offset = Vector3.ClampMagnitude(offset, maxShakeDistance);

                mainCamera.transform.localPosition = _originalPosition + offset;

                elapsed += Time.deltaTime;
                yield return null;
            }

            mainCamera.transform.localPosition = _originalPosition;
            _currentShake = null;
        }

        private IEnumerator ImpulseShakeCoroutine(float amplitude)
        {
            if (mainCamera == null) yield break;

            float duration = 0.2f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float progress = elapsed / duration;
                float currentAmplitude = amplitude * (1f - progress);

                float x = Random.Range(-1f, 1f) * currentAmplitude;
                float y = Random.Range(-1f, 1f) * currentAmplitude;

                Vector3 offset = new Vector3(x, y, 0f);
                offset = Vector3.ClampMagnitude(offset, maxShakeDistance);

                mainCamera.transform.localPosition = _originalPosition + offset;

                elapsed += Time.deltaTime;
                yield return null;
            }

            mainCamera.transform.localPosition = _originalPosition;
            _currentShake = null;
        }

        public void ResetPosition()
        {
            if (_currentShake != null)
            {
                StopCoroutine(_currentShake);
                _currentShake = null;
            }

            if (mainCamera != null)
                mainCamera.transform.localPosition = _originalPosition;
        }

        private void OnDestroy()
        {
            ResetPosition();
        }
    }
}
