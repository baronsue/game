using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using TerracottaARPG.Data;
using TerracottaARPG.Systems;

namespace TerracottaARPG.UI
{
    /// <summary>
    /// 铭契选择UI：Hades风格的铭契选择界面
    /// </summary>
    public class InscriptionSelectionUI : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject selectionPanel;
        public Transform optionsContainer;
        public GameObject optionButtonPrefab;

        [Header("Display")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI subtitleText;

        private List<Inscription> _currentOptions = new List<Inscription>();
        private List<GameObject> _optionButtons = new List<GameObject>();

        private void Awake()
        {
            if (selectionPanel != null)
                selectionPanel.SetActive(false);
        }

        /// <summary>
        /// 显示铭契选择界面
        /// </summary>
        public void ShowSelection(List<Inscription> options)
        {
            if (options == null || options.Count == 0)
            {
                Debug.LogWarning("没有可选的铭契");
                return;
            }

            _currentOptions = options;

            // 清除旧按钮
            ClearOptions();

            // 创建新按钮
            foreach (var inscription in options)
            {
                CreateOptionButton(inscription);
            }

            // 显示面板
            if (selectionPanel != null)
                selectionPanel.SetActive(true);

            // 更新标题
            if (titleText != null)
                titleText.text = "选择铭契";

            if (subtitleText != null)
                subtitleText.text = "选择一个增益效果继续旅程";

            // 暂停游戏
            Time.timeScale = 0f;

            AudioHub.Play("InscriptionSelection_Open");
        }

        /// <summary>
        /// 创建铭契选项按钮
        /// </summary>
        private void CreateOptionButton(Inscription inscription)
        {
            if (optionButtonPrefab == null || optionsContainer == null)
            {
                Debug.LogWarning("铭契选项预制体或容器未设置");
                return;
            }

            GameObject buttonObj = Instantiate(optionButtonPrefab, optionsContainer);
            _optionButtons.Add(buttonObj);

            // 设置按钮内容
            var nameText = buttonObj.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
                nameText.text = inscription.displayName;

            var descText = buttonObj.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();
            if (descText != null)
                descText.text = inscription.description;

            var iconImage = buttonObj.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null && inscription.icon != null)
                iconImage.sprite = inscription.icon;

            // 根据稀有度设置边框颜色
            var rarityBorder = buttonObj.transform.Find("RarityBorder")?.GetComponent<Image>();
            if (rarityBorder != null)
            {
                rarityBorder.color = GetRarityColor(inscription.rarity);
            }

            // 设置按钮点击事件
            var button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnInscriptionSelected(inscription));
            }
        }

        /// <summary>
        /// 铭契被选择
        /// </summary>
        private void OnInscriptionSelected(Inscription inscription)
        {
            AudioHub.Play("InscriptionSelection_Confirm");

            // 添加铭契
            InscriptionManager.Instance.AddInscription(inscription);

            // 关闭UI
            HideSelection();

            Debug.Log($"<color=yellow>选择了铭契: {inscription.displayName}</color>");
        }

        /// <summary>
        /// 清除所有选项按钮
        /// </summary>
        private void ClearOptions()
        {
            foreach (var button in _optionButtons)
            {
                if (button != null)
                    Destroy(button);
            }

            _optionButtons.Clear();
        }

        /// <summary>
        /// 隐藏选择界面
        /// </summary>
        public void HideSelection()
        {
            if (selectionPanel != null)
                selectionPanel.SetActive(false);

            ClearOptions();

            // 恢复游戏
            Time.timeScale = 1f;

            AudioHub.Play("InscriptionSelection_Close");
        }

        /// <summary>
        /// 获取稀有度对应的颜色
        /// </summary>
        private Color GetRarityColor(InscriptionRarity rarity)
        {
            return rarity switch
            {
                InscriptionRarity.Common => new Color(0.8f, 0.8f, 0.8f),      // 陶（白）
                InscriptionRarity.Rare => new Color(0.3f, 0.5f, 0.8f),        // 玉（蓝）
                InscriptionRarity.Epic => new Color(0.6f, 0.3f, 0.8f),        // 青铜（紫）
                InscriptionRarity.Legendary => new Color(1f, 0.8f, 0f),       // 金（橙）
                _ => Color.white
            };
        }
    }
}
