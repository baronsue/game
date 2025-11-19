using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TerracottaARPG.Character;
using TerracottaARPG.Systems;

namespace TerracottaARPG.UI
{
    /// <summary>
    /// 游戏HUD：显示玩家血量、能量、技能冷却等
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("Health")]
        public Slider healthBar;
        public TextMeshProUGUI healthText;
        public Image healthFill;

        [Header("Spirit Energy")]
        public Slider spiritBar;
        public TextMeshProUGUI spiritText;
        public Image spiritFill;

        [Header("Clay Layers")]
        public TextMeshProUGUI clayLayersText;
        public Image[] clayLayerIcons;

        [Header("Skills")]
        public Image slamCooldownOverlay;
        public TextMeshProUGUI slamCooldownText;
        public Image shieldCooldownOverlay;
        public TextMeshProUGUI shieldCooldownText;

        [Header("Info")]
        public TextMeshProUGUI levelNameText;
        public TextMeshProUGUI gameTimeText;

        [Header("Colors")]
        public Color healthNormal = Color.green;
        public Color healthLow = Color.yellow;
        public Color healthCritical = Color.red;
        public Color spiritColor = new Color(0.3f, 0.8f, 1f);

        private TerracottaWarrior _player;

        private void Start()
        {
            // 查找玩家
            FindPlayer();

            // 更新关卡名称
            if (levelNameText != null)
                levelNameText.text = GameManager.Instance.GetCurrentLevelName();
        }

        private void Update()
        {
            if (_player == null)
            {
                FindPlayer();
                return;
            }

            UpdateHealth();
            UpdateSpiritEnergy();
            UpdateSkillCooldowns();
            UpdateGameTime();
        }

        private void FindPlayer()
        {
            _player = FindObjectOfType<TerracottaWarrior>();

            if (_player != null)
            {
                // 订阅事件
                _player.onHealthChanged += OnHealthChanged;
                _player.onSpiritEnergyChanged += OnSpiritEnergyChanged;
                _player.onClayLayerBroken += OnClayLayerBroken;
            }
        }

        #region Update Methods
        private void UpdateHealth()
        {
            if (healthBar != null)
            {
                healthBar.maxValue = _player.maxHealth;
                healthBar.value = _player.currentHealth;
            }

            if (healthText != null)
            {
                healthText.text = $"{_player.currentHealth:F0} / {_player.maxHealth:F0}";
            }

            // 根据血量改变颜色
            if (healthFill != null)
            {
                float healthPercent = _player.currentHealth / _player.maxHealth;

                if (healthPercent <= 0.25f)
                    healthFill.color = healthCritical;
                else if (healthPercent <= 0.5f)
                    healthFill.color = healthLow;
                else
                    healthFill.color = healthNormal;
            }
        }

        private void UpdateSpiritEnergy()
        {
            if (spiritBar != null)
            {
                spiritBar.maxValue = _player.maxSpiritEnergy;
                spiritBar.value = _player.spiritEnergy;
            }

            if (spiritText != null)
            {
                spiritText.text = $"{_player.spiritEnergy:F0}";
            }

            if (spiritFill != null)
            {
                spiritFill.color = spiritColor;
            }
        }

        private void UpdateSkillCooldowns()
        {
            // TODO: 实现技能冷却显示
            // 需要在TerracottaWarrior中暴露冷却时间信息
        }

        private void UpdateGameTime()
        {
            if (gameTimeText != null)
            {
                gameTimeText.text = GameManager.Instance.GetFormattedGameTime();
            }
        }
        #endregion

        #region Event Handlers
        private void OnHealthChanged(float current, float max)
        {
            // 健康值变化时的额外反馈
            // 例如：屏幕边缘红色闪烁
        }

        private void OnSpiritEnergyChanged(float current, float max)
        {
            // 能量变化时的额外反馈
        }

        private void OnClayLayerBroken(int remainingLayers)
        {
            // 更新陶层显示
            if (clayLayersText != null)
            {
                clayLayersText.text = $"陶层: {remainingLayers}";
            }

            // 更新陶层图标
            if (clayLayerIcons != null)
            {
                for (int i = 0; i < clayLayerIcons.Length; i++)
                {
                    if (clayLayerIcons[i] != null)
                    {
                        clayLayerIcons[i].enabled = i < remainingLayers;
                    }
                }
            }

            // 播放破碎音效
            AudioHub.Play("UI_ClayLayerBreak");
        }
        #endregion

        private void OnDestroy()
        {
            // 取消订阅事件
            if (_player != null)
            {
                _player.onHealthChanged -= OnHealthChanged;
                _player.onSpiritEnergyChanged -= OnSpiritEnergyChanged;
                _player.onClayLayerBroken -= OnClayLayerBroken;
            }
        }
    }
}
