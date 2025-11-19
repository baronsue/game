using UnityEngine;
using System.Collections;
using TerracottaARPG.Core;
using TerracottaARPG.Systems;

namespace TerracottaARPG.Character
{
    /// <summary>
    /// 巨型尸将：第一关Boss
    /// 特点：重拳攻击、水银护幕、眩晕窗口、处决机制
    /// </summary>
    public class GiantCorpseGeneral : EnemyBase
    {
        [Header("Boss Settings")]
        public int currentPhase = 1;
        public float phase2Threshold = 0.3f; // 30%血量进入第二阶段

        [Header("Heavy Punch")]
        public float heavyPunchDamage = 50f;
        public float heavyPunchCooldown = 5f;
        private float _heavyPunchTimer = 0f;

        [Header("Bone Chain Sweep")]
        public float sweepDamage = 35f;
        public float sweepRange = 4f;
        public float sweepCooldown = 7f;
        private float _sweepTimer = 0f;

        [Header("Mercury Shield")]
        public float mercuryShieldAmount = 300f;
        public float currentShield = 0f;
        public bool hasShield = true;

        [Header("Phase 2")]
        public float mercuryPulseInterval = 7f;
        public float mercuryPulseDamage = 25f;
        public float mercuryPulseRadius = 8f;
        private float _mercuryPulseTimer = 0f;
        private bool _inPhase2 = false;

        [Header("Visual")]
        public GameObject mercuryShieldVFX;
        public GameObject mercuryPulseVFX;
        private GameObject _activeShieldVFX;

        protected override void Awake()
        {
            base.Awake();

            // Boss属性
            maxHealth = 800f;
            currentHealth = maxHealth;
            moveSpeed = 2.5f;
            attackDamage = 30f;
            attackRange = 2.5f;
            attackCooldown = 2f;
            detectionRange = 15f;

            damageType = DamageType.Physical;
            isHeavyAttack = true;

            // 初始化水银护幕
            currentShield = mercuryShieldAmount;

            // 实例化护盾特效
            if (mercuryShieldVFX != null && hasShield)
            {
                _activeShieldVFX = Instantiate(mercuryShieldVFX, transform);
                _activeShieldVFX.transform.localPosition = Vector3.zero;
            }
        }

        protected override void Update()
        {
            base.Update();

            UpdateBossAbilities();
            CheckPhaseTransition();
        }

        private void UpdateBossAbilities()
        {
            if (IsDead || IsStunned) return;

            // 更新技能冷却
            if (_heavyPunchTimer > 0)
                _heavyPunchTimer -= Time.deltaTime;

            if (_sweepTimer > 0)
                _sweepTimer -= Time.deltaTime;

            if (_inPhase2)
            {
                _mercuryPulseTimer -= Time.deltaTime;
                if (_mercuryPulseTimer <= 0)
                {
                    CastMercuryPulse();
                    _mercuryPulseTimer = mercuryPulseInterval;
                }
            }

            // Boss技能逻辑
            if (_currentState == EnemyState.Attack)
            {
                // 优先使用重拳
                if (_heavyPunchTimer <= 0)
                {
                    CastHeavyPunch();
                    _heavyPunchTimer = heavyPunchCooldown;
                }
                // 次选骨链横扫
                else if (_sweepTimer <= 0)
                {
                    CastBoneChainSweep();
                    _sweepTimer = sweepCooldown;
                }
            }
        }

        private void CheckPhaseTransition()
        {
            if (_inPhase2) return;

            float healthPercent = currentHealth / maxHealth;
            if (healthPercent <= phase2Threshold)
            {
                EnterPhase2();
            }
        }

        #region Boss Abilities
        private void CastHeavyPunch()
        {
            if (_target == null) return;

            AudioHub.Play("Boss_HeavyPunch");

            Debug.Log("<color=red>【巨型尸将】重拳！</color>");

            // 检测玩家是否在范围内
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDamageable>(out var damageable))
                {
                    DamagePacket packet = new DamagePacket(heavyPunchDamage, DamageType.Physical, true, transform);
                    damageable.ApplyDamage(packet);

                    // 击退
                    if (hit.TryGetComponent<KnockbackReceiver>(out var kb))
                    {
                        Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                        kb.AddKnockback(knockbackDir, 12f);
                    }
                }
            }

            CameraShaker.Shake(0.3f, 0.8f);
        }

        private void CastBoneChainSweep()
        {
            AudioHub.Play("Boss_BoneChainSweep");

            Debug.Log("<color=red>【巨型尸将】骨链横扫！</color>");

            // 扇形攻击（简化为圆形）
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, sweepRange, playerLayer);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDamageable>(out var damageable))
                {
                    DamagePacket packet = new DamagePacket(sweepDamage, DamageType.Physical, false, transform);
                    damageable.ApplyDamage(packet);

                    // 轻微击退
                    if (hit.TryGetComponent<KnockbackReceiver>(out var kb))
                    {
                        Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                        kb.AddKnockback(knockbackDir, 5f);
                    }
                }
            }
        }

        private void CastMercuryPulse()
        {
            AudioHub.Play("Boss_MercuryPulse");

            Debug.Log("<color=cyan>【巨型尸将】水银脉冲！</color>");

            // 实例化水银脉冲特效
            if (mercuryPulseVFX != null)
            {
                GameObject vfx = Instantiate(mercuryPulseVFX, transform.position, Quaternion.identity);
                Destroy(vfx, 3f);
            }

            // 延迟伤害（给玩家反应时间）
            StartCoroutine(MercuryPulseDamage());
        }

        private IEnumerator MercuryPulseDamage()
        {
            yield return new WaitForSeconds(1f); // 1秒预警

            // 范围伤害
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, mercuryPulseRadius, playerLayer);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDamageable>(out var damageable))
                {
                    DamagePacket packet = new DamagePacket(mercuryPulseDamage, DamageType.Mercury, false, transform);
                    damageable.ApplyDamage(packet);
                }
            }

            CameraShaker.Shake(0.25f, 0.5f);
        }

        private void EnterPhase2()
        {
            _inPhase2 = true;
            _mercuryPulseTimer = mercuryPulseInterval;

            AudioHub.Play("Boss_Phase2");

            Debug.Log("<color=red>=== 【巨型尸将】进入第二阶段！===</color>");

            // TODO: 播放相变动画
            // TODO: 改变外观
        }
        #endregion

        #region Damage System
        public override void ApplyDamage(DamagePacket packet)
        {
            if (IsDead) return;

            // 水银护幕先吸收伤害
            if (hasShield && currentShield > 0)
            {
                float shieldDamage = Mathf.Min(packet.amount, currentShield);
                currentShield -= shieldDamage;
                packet.amount -= shieldDamage;

                Debug.Log($"<color=cyan>水银护幕吸收 {shieldDamage:F0} 点伤害，剩余护盾: {currentShield:F0}</color>");

                if (currentShield <= 0)
                {
                    hasShield = false;
                    if (_activeShieldVFX != null)
                    {
                        Destroy(_activeShieldVFX);
                        _activeShieldVFX = null;
                    }

                    AudioHub.Play("Boss_ShieldBreak");
                    Debug.Log("<color=red>水银护幕破碎！</color>");
                }

                // 如果伤害被完全吸收，不对本体造成伤害
                if (packet.amount <= 0)
                    return;
            }

            // 应用抗性
            float damageReduction = resistances.Get(packet.type);
            float finalDamage = packet.amount * (1f - damageReduction);

            currentHealth -= finalDamage;
            currentHealth = Mathf.Max(0f, currentHealth);

            // 受击反馈
            AudioHub.Play("Boss_Hit");
            StartCoroutine(FlashRed());

            ShowDamageNumber(finalDamage, packet.isCritical);

            if (currentHealth <= 0f)
                Die();
        }
        #endregion

        #region Death
        protected override void Die()
        {
            if (IsDead) return;

            IsDead = true;
            _currentState = EnemyState.Dead;

            AudioHub.Play("Boss_Death");

            Debug.Log("<color=red>=== 【巨型尸将】被击败！===</color>");

            // TODO: 播放死亡动画
            // TODO: 掉落铭契/装备

            // 掉落「破碎的青铜铭片 · 饕餮」
            Debug.Log("掉落：破碎的青铜铭片 · 饕餮");

            CameraShaker.Shake(0.5f, 1f);

            // 销毁对象
            Destroy(gameObject, 2f);
        }
        #endregion

        #region Gizmos
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            // 骨链横扫范围
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, sweepRange);

            // 水银脉冲范围
            if (_inPhase2)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, mercuryPulseRadius);
            }
        }
        #endregion
    }
}
