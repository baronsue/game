using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace TerracottaARPG.UI
{
    /// <summary>
    /// 处决提示UI：显示"处决！(E)"提示
    /// </summary>
    public class ExecutePromptUI : MonoBehaviour
    {
        private static ExecutePromptUI _instance;
        public static ExecutePromptUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("ExecutePromptUI");
                    _instance = go.AddComponent<ExecutePromptUI>();
                }
                return _instance;
            }
        }

        [Header("UI References")]
        public Canvas canvas;
        public TextMeshProUGUI promptText;
        public Image promptBackground;
        public GameObject promptPanel;

        [Header("Settings")]
        public float displayDuration = 1.2f;
        public float fadeInDuration = 0.2f;
        public float fadeOutDuration = 0.3f;

        [Header("Animation")]
        public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0.8f, 1f, 1.2f);
        public Color normalColor = Color.white;
        public Color highlightColor = Color.yellow;

        private Coroutine _currentPrompt;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            SetupUI();
        }

        private void SetupUI()
        {
            // 创建Canvas
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("ExecutePromptCanvas");
                canvasObj.transform.SetParent(transform);
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;

                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // 创建提示面板
            if (promptPanel == null)
            {
                promptPanel = new GameObject("PromptPanel");
                promptPanel.transform.SetParent(canvas.transform, false);

                RectTransform rt = promptPanel.AddComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(300f, 100f);

                _canvasGroup = promptPanel.AddComponent<CanvasGroup>();
                _canvasGroup.alpha = 0f;
            }

            // 创建背景
            if (promptBackground == null)
            {
                GameObject bgObj = new GameObject("Background");
                bgObj.transform.SetParent(promptPanel.transform, false);

                RectTransform bgRt = bgObj.AddComponent<RectTransform>();
                bgRt.anchorMin = Vector2.zero;
                bgRt.anchorMax = Vector2.one;
                bgRt.sizeDelta = Vector2.zero;

                promptBackground = bgObj.AddComponent<Image>();
                promptBackground.color = new Color(0f, 0f, 0f, 0.7f);
            }

            // 创建文本
            if (promptText == null)
            {
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(promptPanel.transform, false);

                RectTransform textRt = textObj.AddComponent<RectTransform>();
                textRt.anchorMin = Vector2.zero;
                textRt.anchorMax = Vector2.one;
                textRt.sizeDelta = Vector2.zero;

                promptText = textObj.AddComponent<TextMeshProUGUI>();
                promptText.text = "处决！(E)";
                promptText.fontSize = 48;
                promptText.alignment = TextAlignmentOptions.Center;
                promptText.color = highlightColor;
                promptText.fontStyle = FontStyles.Bold;
            }

            // 初始隐藏
            if (promptPanel != null)
                promptPanel.SetActive(false);
        }

        /// <summary>
        /// 显示处决提示
        /// </summary>
        public static void Show(float duration = -1f)
        {
            if (Instance != null)
            {
                float displayTime = duration > 0 ? duration : Instance.displayDuration;
                Instance.ShowPrompt(displayTime);
            }
        }

        /// <summary>
        /// 隐藏处决提示
        /// </summary>
        public static void Hide()
        {
            if (Instance != null)
                Instance.HidePrompt();
        }

        private void ShowPrompt(float duration)
        {
            if (_currentPrompt != null)
                StopCoroutine(_currentPrompt);

            _currentPrompt = StartCoroutine(ShowPromptCoroutine(duration));
        }

        private void HidePrompt()
        {
            if (_currentPrompt != null)
            {
                StopCoroutine(_currentPrompt);
                _currentPrompt = null;
            }

            if (promptPanel != null)
                promptPanel.SetActive(false);
        }

        private IEnumerator ShowPromptCoroutine(float duration)
        {
            if (promptPanel == null) yield break;

            promptPanel.SetActive(true);

            // 淡入
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeInDuration;

                if (_canvasGroup != null)
                    _canvasGroup.alpha = t;

                if (promptPanel != null)
                {
                    float scale = scaleCurve.Evaluate(t);
                    promptPanel.transform.localScale = Vector3.one * scale;
                }

                yield return null;
            }

            if (_canvasGroup != null)
                _canvasGroup.alpha = 1f;

            if (promptPanel != null)
                promptPanel.transform.localScale = Vector3.one * 1.2f;

            // 脉冲动画
            float pulseElapsed = 0f;
            while (pulseElapsed < duration)
            {
                pulseElapsed += Time.deltaTime;

                // 颜色脉冲
                if (promptText != null)
                {
                    float pulse = Mathf.Sin(pulseElapsed * 8f) * 0.5f + 0.5f;
                    promptText.color = Color.Lerp(normalColor, highlightColor, pulse);
                }

                yield return null;
            }

            // 淡出
            elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = 1f - (elapsed / fadeOutDuration);

                if (_canvasGroup != null)
                    _canvasGroup.alpha = t;

                yield return null;
            }

            if (promptPanel != null)
                promptPanel.SetActive(false);

            _currentPrompt = null;
        }

        private void OnDestroy()
        {
            if (_currentPrompt != null)
                StopCoroutine(_currentPrompt);
        }
    }
}
