using UnityEngine;
using System.Collections;

/// <summary>
/// 简化版镜头震动系统（无命名空间）
/// 提供各种镜头震动效果
/// </summary>
public class SimpleCameraShaker : MonoBehaviour
{
    private static SimpleCameraShaker _instance;
    public static SimpleCameraShaker Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SimpleCameraShaker");
                _instance = go.AddComponent<SimpleCameraShaker>();
            }
            return _instance;
        }
    }

    [Header("Settings")]
    public Camera mainCamera;
    public float maxShakeDistance = 2f;

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

        Debug.Log("✅ SimpleCameraShaker 初始化成功！");
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
    /// 冲击波震动
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
            // 使用Perlin噪声生成平滑震动
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
