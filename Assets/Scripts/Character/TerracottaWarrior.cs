using UnityEngine;
using System;
using System.Collections;
using TerracottaARPG.Core;
using TerracottaARPG.Systems;

namespace TerracottaARPG.Character
{
    /// <summary>
    /// 陶俑战士：核心角色控制器
    /// 实现陶土护甲、灵火外泄、技能系统
    /// </summary>
    public class TerracottaWarrior : MonoBehaviour, IDamageable
    {
        #region Stats
        [Header("Stats")]
        public float maxHealth = 1000f;
        public float currentHealth;
        public float spiritEnergy = 100f;
        public float maxSpiritEnergy = 100f;

        [Header("Combat Stats")]
        [Range(0, 1f)] public float criticalChance = 0.15f;
        public float criticalMultiplier = 2.0f;
        #endregion

        #region Clay Armor System
        [Header("Clay Armor - 陶土护甲")]
        [Range(0, 0.9f)] public float baseDamageReduction = 0.25f; // 完整陶层时减伤
        public int clayLayers = 3;            // 陶层段数，破一层减伤降低
        private int layersLeft;

        [Tooltip("每碎一层增加的伤害加成")]
        public float damageBuffPerLayer = 0.05f; // 每碎一层 +5% 伤害
        #endregion

        #region Resistances
        [Header("Resistances - 抗性")]
        public Resistances resistances;
        #endregion

        #region Visuals & Audio
        [Header("Visuals & Audio")]
        public SpriteRenderer bodyRenderer;
        public ParticleSystem clayChipsVFX;
        public Light spiritLight;             // URP 2D Light 或 3D Light 均可
        public AnimationCurve emissionByHealth = AnimationCurve.Linear(0, 5f, 1, 1.2f);

        [Header("VFX Prefabs")]
        public GameObject slamVFXPrefab;
        public GameObject shieldVFXPrefab;
        private GameObject _activeShieldVFX;
        #endregion

        #region Skills
        [Header("Shield Wall (W) - 铜墙铁壁")]
        public float shieldDuration = 1.2f;
        [Range(0, 0.9f)] public float shieldDR = 0.40f;
        [Range(0, 1f)] public float reflectPercent = 0.5f;
        public float heavyStunSeconds = 1.5f;
        public float shieldCooldown = 5f;
        private float _shieldCooldownTimer = 0f;

        [Header("Slam (Q) - 千钧坠")]
        public float slamRadius = 3.0f;
        public float slamDamage = 300f;
        public float slamSlowDuration = 1.5f;
        public float slamSlowPercent = 0.4f;
        public float slamCooldown = 3f;
        private float _slamCooldownTimer = 0f;
        public LayerMask damageLayer;

        [Header("Execute (E) - 处决")]
        public float executeRange = 2f;
        public float executeHealthThreshold = 0.3f; // 目标血量低于30%才能处决
        public float executeBaseDamage = 200f;
        public float executeHealthPercentDamage = 0.2f; // 对当前生命上限20%的破甲伤害
        #endregion

        #region Runtime Variables
        public bool IsShielding { get; private set; }
        public bool IsDead { get; private set; }

        private MaterialPropertyBlock _mpb;
        private float _extraDamageBuff;   // 由裂纹层/铭契等叠加
        private float _extraDamageReduction;// 临时 DR（技能/铭契）

        // Components
        private Rigidbody2D _rb;
        private StatusHost _statusHost;
        private KnockbackReceiver _knockbackReceiver;

        // Movement
        [Header("Movement")]
        public float moveSpeed = 5f;
        private Vector2 _moveInput;
        #endregion

        #region Events
        public event Action<float, float> onHealthChanged; // (current, max)
        public event Action<float, float> onSpiritEnergyChanged; // (current, max)
        public event Action onDeath;
        public event Action<int> onClayLayerBroken; // (remaining layers)
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            currentHealth = maxHealth;
            layersLeft = clayLayers;
            _mpb = new MaterialPropertyBlock();

            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null)
                _rb = gameObject.AddComponent<Rigidbody2D>();

            _rb.gravityScale = 0f;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            _statusHost = GetComponent<StatusHost>();
            if (_statusHost == null)
                _statusHost = gameObject.AddComponent<StatusHost>();

            _knockbackReceiver = GetComponent<KnockbackReceiver>();
            if (_knockbackReceiver == null)
                _knockbackReceiver = gameObject.AddComponent<KnockbackReceiver>();

            UpdateVisuals();
        }

        private void Update()
        {
            if (IsDead) return;

            HandleInput();
            UpdateCooldowns();
        }

        private void FixedUpdate()
        {
            if (IsDead || _statusHost.Stunned) return;

            HandleMovement();
        }
        #endregion

        #region Input Handling
        private void HandleInput()
        {
            // 移动输入
            _moveInput.x = Input.GetAxisRaw("Horizontal");
            _moveInput.y = Input.GetAxisRaw("Vertical");

            // 技能输入
            if (Input.GetKeyDown(KeyCode.Q))
                TryCastSlam();

            if (Input.GetKeyDown(KeyCode.W))
                TryCastShield();

            if (Input.GetKeyDown(KeyCode.E))
                TryExecute();

            // 普通攻击
            if (Input.GetMouseButtonDown(0))
                PerformBasicAttack();
        }

        private void HandleMovement()
        {
            if (_moveInput.sqrMagnitude > 0.01f)
            {
                Vector2 movement = _moveInput.normalized * moveSpeed;

                // 应用减速效果
                if (_statusHost.Slowed)
                    movement *= (1f - _statusHost.SlowPercent);

                _rb.velocity = movement;
            }
            else
            {
                _rb.velocity = Vector2.zero;
            }
        }

        private void UpdateCooldowns()
        {
            if (_slamCooldownTimer > 0)
                _slamCooldownTimer -= Time.deltaTime;

            if (_shieldCooldownTimer > 0)
                _shieldCooldownTimer -= Time.deltaTime;
        }
        #endregion

        #region Combat - Skills
        private void TryCastSlam()
        {
            if (_slamCooldownTimer > 0)
            {
                Debug.Log($"千钧坠冷却中: {_slamCooldownTimer:F1}s");
                return;
            }

            DoSlam();
            _slamCooldownTimer = slamCooldown;
        }

        private void DoSlam()
        {
            AudioHub.Play("Slam_Cast");

            // 实例化地裂特效
            if (slamVFXPrefab != null)
            {
                GameObject vfx = Instantiate(slamVFXPrefab, transform.position, Quaternion.identity);
                Destroy(vfx, 2f);
            }

            // 检测范围内敌人
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, slamRadius, damageLayer);

            foreach (var h in hits)
            {
                if (h.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (damageable == this) continue; // 不伤害自己

                    // 计算伤害
                    float damage = CalculateDamage(slamDamage);
                    bool isCrit = RollCritical();

                    if (isCrit)
                        damage *= criticalMultiplier;

                    DamagePacket packet = new DamagePacket(damage, DamageType.Physical, true, transform);
                    packet.isCritical = isCrit;

                    damageable.ApplyDamage(packet);

                    // 击退
                    if (h.TryGetComponent<KnockbackReceiver>(out var kb))
                    {
                        Vector2 knockbackDir = (h.transform.position - transform.position).normalized;
                        kb.AddImpulse(knockbackDir, 7f);
                    }

                    // 减速
                    if (h.TryGetComponent<StatusHost>(out var status))
                    {
                        status.ApplySlow(slamSlowDuration, slamSlowPercent);
                    }
                }
            }

            // 镜头震动
            CameraShaker.Shake(0.15f, 0.6f);

            Debug.Log($"<color=yellow>千钧坠！击中 {hits.Length} 个目标</color>");
        }

        private void TryCastShield()
        {
            if (_shieldCooldownTimer > 0)
            {
                Debug.Log($"铜墙铁壁冷却中: {_shieldCooldownTimer:F1}s");
                return;
            }

            StartCoroutine(ShieldWallCO());
            _shieldCooldownTimer = shieldCooldown;
        }

        private IEnumerator ShieldWallCO()
        {
            if (IsShielding) yield break;

            IsShielding = true;
            float prevExtraDR = _extraDamageReduction;
            _extraDamageReduction = Mathf.Clamp01(_extraDamageReduction + shieldDR);

            AudioHub.Play("Shield_Up");

            // 盾牌特效
            if (shieldVFXPrefab != null)
            {
                _activeShieldVFX = Instantiate(shieldVFXPrefab, transform);
                _activeShieldVFX.transform.localPosition = Vector3.zero;
            }

            Debug.Log("<color=cyan>铜墙铁壁激活！</color>");

            yield return new WaitForSeconds(shieldDuration);

            AudioHub.Play("Shield_Down");
            _extraDamageReduction = prevExtraDR;
            IsShielding = false;

            if (_activeShieldVFX != null)
                Destroy(_activeShieldVFX);

            Debug.Log("<color=cyan>铜墙铁壁结束</color>");
        }

        private void TryExecute()
        {
            // 查找范围内可处决的敌人
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, executeRange, damageLayer);

            IDamageable target = null;
            float closestDist = float.MaxValue;

            foreach (var h in hits)
            {
                if (h.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (damageable == this) continue;

                    // 检查是否可处决（需要实现生命值接口）
                    if (h.TryGetComponent<EnemyBase>(out var enemy))
                    {
                        float healthPercent = enemy.currentHealth / enemy.maxHealth;
                        if (healthPercent <= executeHealthThreshold || enemy.IsStunned)
                        {
                            float dist = Vector2.Distance(transform.position, h.transform.position);
                            if (dist < closestDist)
                            {
                                closestDist = dist;
                                target = damageable;
                            }
                        }
                    }
                }
            }

            if (target != null)
            {
                PerformExecute(target);
            }
            else
            {
                Debug.Log("没有可处决的目标");
            }
        }

        private void PerformExecute(IDamageable target)
        {
            AudioHub.Play("Execute");

            // 处决伤害：基础伤害 + 目标当前生命百分比伤害
            float damage = executeBaseDamage;

            if (target.GetTransform().TryGetComponent<EnemyBase>(out var enemy))
            {
                damage += enemy.currentHealth * executeHealthPercentDamage;
            }

            DamagePacket packet = new DamagePacket(damage, DamageType.Spirit, true, transform);
            packet.isCritical = true; // 处决必定暴击

            target.ApplyDamage(packet);

            // 处决后获得灵火（能量恢复）
            ModifySpiritEnergy(20f);

            // 镜头效果
            CameraShaker.ImpulseShake(1.2f);

            Debug.Log($"<color=red>处决成功！造成 {damage:F0} 点伤害</color>");

            // TODO: 触发铭契效果（饕餮·贪噬等）
        }

        private void PerformBasicAttack()
        {
            // 简化版普通攻击
            Debug.Log("普通攻击！");
            AudioHub.Play("BasicAttack");

            // TODO: 实现完整的普通攻击逻辑
        }
        #endregion

        #region Combat - Damage System
        public void ApplyDamage(DamagePacket packet)
        {
            if (IsDead) return;

            // 计算有效减伤：陶层DR → 技能DR → 抗性（乘法叠加，避免线性叠穿）
            float layerDR = layersLeft > 0 ? baseDamageReduction * ((float)layersLeft / clayLayers) : 0f;
            float totalDR = 1f;
            totalDR *= (1f - layerDR);
            totalDR *= (1f - _extraDamageReduction);
            totalDR *= (1f - resistances.Get(packet.type));

            float finalDamage = Mathf.Max(1f, packet.amount * totalDR);

            currentHealth -= finalDamage;
            currentHealth = Mathf.Max(0f, currentHealth);

            // 受击反馈
            AudioHub.Play("ClayHit_Heavy");
            if (clayChipsVFX)
                clayChipsVFX.Play();

            // 陶层破裂判定：按"受击阈值"粗略模拟
            float layerBreakThreshold = maxHealth / (clayLayers + 1);
            int expectedLayers = Mathf.Clamp(Mathf.FloorToInt(currentHealth / layerBreakThreshold), 0, clayLayers);

            if (expectedLayers < layersLeft)
            {
                int layersBroken = layersLeft - expectedLayers;
                layersLeft = expectedLayers;

                AudioHub.Play("Terracotta_Crack");

                // 被打碎越多 → 伤害↑，减伤↓（已体现在 layerDR）
                _extraDamageBuff += damageBuffPerLayer * layersBroken;

                onClayLayerBroken?.Invoke(layersLeft);

                Debug.Log($"<color=orange>陶层破裂！剩余 {layersLeft} 层，伤害增幅: +{_extraDamageBuff * 100:F0}%</color>");
            }

            UpdateVisuals();
            onHealthChanged?.Invoke(currentHealth, maxHealth);

            Debug.Log($"受到 {finalDamage:F0} 点{packet.type}伤害 (减伤: {(1f - totalDR) * 100:F0}%) | 剩余生命: {currentHealth:F0}/{maxHealth:F0}");

            if (currentHealth <= 0f)
                Die();

            // 盾反：在防御窗口内，反弹伤害并对重击施加眩晕
            if (IsShielding && packet.source != null)
            {
                if (UnityEngine.Random.value < 0.9f)
                    AudioHub.Play("Shield_Reflect");

                var dmgBack = new DamagePacket(packet.amount * reflectPercent, DamageType.Spirit, false, transform);
                packet.source.GetComponent<IDamageable>()?.ApplyDamage(dmgBack);

                Debug.Log($"<color=cyan>盾反！反弹 {dmgBack.amount:F0} 点伤害</color>");

                if (packet.isHeavy)
                {
                    var st = packet.source.GetComponent<StatusHost>();
                    if (st)
                    {
                        st.ApplyStun(heavyStunSeconds);
                        Debug.Log($"<color=cyan>重击被反震！眩晕 {heavyStunSeconds}s</color>");
                    }
                }

                // 盾反镜头效果
                CameraShaker.ShakeDirectional(0.2f, 0.4f, (packet.source.position - transform.position).normalized);
            }
        }

        public Transform GetTransform() => transform;

        private float CalculateDamage(float baseDamage)
        {
            return baseDamage * (1f + _extraDamageBuff);
        }

        private bool RollCritical()
        {
            return UnityEngine.Random.value < criticalChance;
        }
        #endregion

        #region Resource Management
        public void ModifySpiritEnergy(float amount)
        {
            spiritEnergy += amount;
            spiritEnergy = Mathf.Clamp(spiritEnergy, 0f, maxSpiritEnergy);
            onSpiritEnergyChanged?.Invoke(spiritEnergy, maxSpiritEnergy);
        }

        public void Heal(float amount)
        {
            if (IsDead) return;

            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            onHealthChanged?.Invoke(currentHealth, maxHealth);

            UpdateVisuals();
        }
        #endregion

        #region Visuals
        private void UpdateVisuals()
        {
            float hp01 = Mathf.Clamp01(currentHealth / maxHealth);

            // 发光强度：血越少越亮（灵火外泄）
            float emission = emissionByHealth.Evaluate(hp01);

            if (bodyRenderer != null)
            {
                bodyRenderer.GetPropertyBlock(_mpb);
                _mpb.SetColor("_EmissionColor", Color.green * emission);
                _mpb.SetFloat("_CrackAmount", 1f - hp01); // 自定义材质属性：裂纹填充
                bodyRenderer.SetPropertyBlock(_mpb);
            }

            if (spiritLight)
                spiritLight.intensity = Mathf.Lerp(10f, 2f, hp01);
        }
        #endregion

        #region Death
        private void Die()
        {
            if (IsDead) return;

            IsDead = true;
            AudioHub.Play("Terracotta_Shatter");

            onDeath?.Invoke();

            Debug.Log("<color=red>陶俑破碎！</color>");

            // TODO: 播放死亡动画/特效
            // TODO: 改为对象池碎片

            // 暂时禁用对象
            gameObject.SetActive(false);
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmosSelected()
        {
            // 千钧坠范围
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, slamRadius);

            // 处决范围
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, executeRange);
        }
        #endregion
    }
}
