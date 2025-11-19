using UnityEngine;

namespace TerracottaARPG.Core
{
    public enum DamageType { Physical, Mercury, Fire, Spirit, Corrosion }

    [System.Serializable]
    public struct Resistances
    {
        [Range(0, 1)] public float Physical;
        [Range(0, 1)] public float Mercury;
        [Range(0, 1)] public float Fire;
        [Range(0, 1)] public float Spirit;
        [Range(0, 1)] public float Corrosion;

        public float Get(DamageType t) => t switch
        {
            DamageType.Physical => Physical,
            DamageType.Mercury => Mercury,
            DamageType.Fire => Fire,
            DamageType.Spirit => Spirit,
            DamageType.Corrosion => Corrosion,
            _ => 0f
        };
    }

    public struct DamagePacket
    {
        public float amount;
        public DamageType type;
        public bool isHeavy;     // 重击：可被"铜墙铁壁"反震并眩晕
        public bool isCritical;  // 暴击标记
        public Transform source; // 伤害来源（用于朝向/击退）

        public DamagePacket(float a, DamageType t, bool heavy, Transform s)
        {
            amount = a;
            type = t;
            isHeavy = heavy;
            isCritical = false;
            source = s;
        }

        public DamagePacket WithCritical(bool crit)
        {
            isCritical = crit;
            return this;
        }
    }

    public interface IDamageable
    {
        void ApplyDamage(DamagePacket packet);
        Transform GetTransform();
        bool IsDead { get; }
    }
}
