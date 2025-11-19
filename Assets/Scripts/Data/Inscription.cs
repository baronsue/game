using UnityEngine;
using TerracottaARPG.Character;

namespace TerracottaARPG.Data
{
    /// <summary>
    /// 铭契：类似Hades的Boon系统
    /// 提供战斗增益效果
    /// </summary>
    [CreateAssetMenu(fileName = "New Inscription", menuName = "TerracottaARPG/Inscription")]
    public class Inscription : ScriptableObject
    {
        [Header("Basic Info")]
        public string id;
        public string displayName;
        [TextArea(3, 6)] public string description;
        public Sprite icon;

        [Header("Rarity")]
        public InscriptionRarity rarity = InscriptionRarity.Common;

        [Header("Effect")]
        public EffectKind effect;
        public float value;
        public float duration;
        public int maxStacks = 0;

        [Header("Conditions")]
        [Tooltip("是否需要特定条件触发")]
        public bool requiresCondition = false;
        public TriggerCondition triggerCondition;

        /// <summary>
        /// 效果类型
        /// </summary>
        public enum EffectKind
        {
            // 属性增强
            AddDamagePct,           // 增加伤害百分比
            AddDRPct,               // 增加减伤百分比
            AddCriticalChance,      // 增加暴击率
            AddCriticalMultiplier,  // 增加暴击伤害

            // 技能增强
            Slam_RadiusUp,          // 千钧坠：范围增加
            Slam_GuaranteedCrit,    // 千钧坠：对击退目标必暴击
            Shield_OnHit_Stack,     // 铜墙铁壁：受击叠加裂纹层
            Shield_DurationUp,      // 铜墙铁壁：持续时间增加

            // 触发效果
            OnExecute_Buff,         // 处决后：伤害增益
            OnExecute_ShatterDamage,// 处决后：震裂伤害
            OnHit_LifeSteal,        // 击中：生命偷取
            OnKill_EnergyRestore,   // 击杀：能量恢复

            // 特殊效果
            Mercury_Resistance,     // 水银抗性
            Mercury_DamageBonus,    // 对汞蚀目标伤害加成
            ClayArmor_SlowRegen,    // 陶层缓慢恢复
        }

        /// <summary>
        /// 触发条件
        /// </summary>
        public enum TriggerCondition
        {
            None,
            OnExecute,      // 处决时
            OnCritical,     // 暴击时
            OnKill,         // 击杀时
            OnDamaged,      // 受伤时
            LowHealth,      // 低血量时
            FullHealth,     // 满血时
        }

        /// <summary>
        /// 应用铭契效果到角色
        /// </summary>
        public void ApplyToWarrior(TerracottaWarrior warrior)
        {
            if (warrior == null) return;

            switch (effect)
            {
                case EffectKind.AddDamagePct:
                    // TODO: 实现伤害增益系统
                    Debug.Log($"应用铭契: {displayName} - 伤害增加 {value * 100}%");
                    break;

                case EffectKind.Slam_RadiusUp:
                    warrior.slamRadius *= (1f + value);
                    Debug.Log($"应用铭契: {displayName} - 千钧坠范围增加 {value * 100}%");
                    break;

                case EffectKind.Shield_OnHit_Stack:
                    // TODO: 实现盾反叠加裂纹层
                    Debug.Log($"应用铭契: {displayName} - 盾墙受击叠加裂纹层");
                    break;

                // 更多效果...
            }
        }

        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        public bool CheckCondition(TriggerCondition condition)
        {
            if (!requiresCondition) return true;
            return triggerCondition == condition;
        }
    }

    /// <summary>
    /// 铭契稀有度
    /// </summary>
    public enum InscriptionRarity
    {
        Common,     // 陶（白）
        Rare,       // 玉（蓝）
        Epic,       // 青铜（紫）
        Legendary   // 金（橙）
    }
}
