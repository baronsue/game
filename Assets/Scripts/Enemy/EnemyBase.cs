using UnityEngine;
using System.Collections;
using TerracottaARPG.Core;
using TerracottaARPG.Systems;

namespace TerracottaARPG.Character
{
    /// <summary>
    /// 敌人基类：所有敌人的基础逻辑
    /// </summary>
    public class EnemyBase : MonoBehaviour, IDamageable
    {
        #region Stats
        [Header("Stats")]
        public float maxHealth = 100f;
        public float currentHealth;
        public float moveSpeed = 3f;
        public float attackDamage = 20f;
        public float attackRange = 1.5f;
        public float attackCooldown = 1.5f;
        protected float _attackCooldownTimer = 0f;

        [Header("Combat")]
        public DamageType damageType = DamageType.Physical;
        public bool isHeavyAttack = false;
        public Resistances resistances;
        #endregion

        #region AI
        [Header("AI")]
        public float detectionRange = 8f;
        public float chaseRange = 12f;
        public LayerMask playerLayer;
        protected Transform _target;
        protected EnemyState _currentState = EnemyState.Idle;

        protected enum EnemyState
        {
            Idle,
            Patrol,
            Chase,
            Attack,
            Stunned,
            Dead
        }
        #endregion

        #region Components
        protected Rigidbody2D _rb;
        protected StatusHost _statusHost;
        protected KnockbackReceiver _knockbackReceiver;
        protected SpriteRenderer _spriteRenderer;
        #endregion

        #region Properties
        public bool IsDead { get; protected set; }
        public bool IsStunned => _statusHost != null && _statusHost.Stunned;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            currentHealth = maxHealth;

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

            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void Start()
        {
            // 查找玩家
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                _target = player.transform;
        }

        protected virtual void Update()
        {
            if (IsDead) return;

            UpdateAI();
            UpdateCooldowns();
        }

        protected virtual void FixedUpdate()
        {
            if (IsDead || IsStunned) return;

            ExecuteCurrentState();
        }
        #endregion

        #region AI Logic
        protected virtual void UpdateAI()
        {
            if (_target == null || IsStunned)
            {
                _currentState = EnemyState.Idle;
                return;
            }

            float distanceToTarget = Vector2.Distance(transform.position, _target.position);

            switch (_currentState)
            {
                case EnemyState.Idle:
                    if (distanceToTarget <= detectionRange)
                        _currentState = EnemyState.Chase;
                    break;

                case EnemyState.Chase:
                    if (distanceToTarget <= attackRange)
                        _currentState = EnemyState.Attack;
                    else if (distanceToTarget > chaseRange)
                        _currentState = EnemyState.Idle;
                    break;

                case EnemyState.Attack:
                    if (distanceToTarget > attackRange)
                        _currentState = EnemyState.Chase;
                    break;
            }
        }

        protected virtual void ExecuteCurrentState()
        {
            switch (_currentState)
            {
                case EnemyState.Idle:
                    _rb.velocity = Vector2.zero;
                    break;

                case EnemyState.Chase:
                    ChaseTarget();
                    break;

                case EnemyState.Attack:
                    AttackTarget();
                    break;
            }
        }

        protected virtual void ChaseTarget()
        {
            if (_target == null) return;

            Vector2 direction = (_target.position - transform.position).normalized;
            float speed = moveSpeed;

            // 应用减速
            if (_statusHost.Slowed)
                speed *= (1f - _statusHost.SlowPercent);

            _rb.velocity = direction * speed;

            // 面向目标
            FlipTowardsTarget();
        }

        protected virtual void AttackTarget()
        {
            _rb.velocity = Vector2.zero;

            if (_attackCooldownTimer <= 0f)
            {
                PerformAttack();
                _attackCooldownTimer = attackCooldown;
            }
        }

        protected virtual void PerformAttack()
        {
            if (_target == null) return;

            AudioHub.Play("Enemy_Attack");

            // 检测范围内玩家
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDamageable>(out var damageable))
                {
                    DamagePacket packet = new DamagePacket(attackDamage, damageType, isHeavyAttack, transform);
                    damageable.ApplyDamage(packet);
                }
            }
        }

        protected void FlipTowardsTarget()
        {
            if (_target == null || _spriteRenderer == null) return;

            if (_target.position.x < transform.position.x)
                _spriteRenderer.flipX = true;
            else
                _spriteRenderer.flipX = false;
        }

        protected void UpdateCooldowns()
        {
            if (_attackCooldownTimer > 0)
                _attackCooldownTimer -= Time.deltaTime;
        }
        #endregion

        #region Damage System
        public virtual void ApplyDamage(DamagePacket packet)
        {
            if (IsDead) return;

            // 应用抗性
            float damageReduction = resistances.Get(packet.type);
            float finalDamage = packet.amount * (1f - damageReduction);

            currentHealth -= finalDamage;
            currentHealth = Mathf.Max(0f, currentHealth);

            // 受击反馈
            AudioHub.Play("Enemy_Hit");
            StartCoroutine(FlashRed());

            // 显示伤害数字
            ShowDamageNumber(finalDamage, packet.isCritical);

            if (currentHealth <= 0f)
                Die();
        }

        public Transform GetTransform() => transform;

        protected virtual void ShowDamageNumber(float damage, bool isCritical)
        {
            // TODO: 实现浮动伤害数字
            string color = isCritical ? "red" : "white";
            Debug.Log($"<color={color}>{damage:F0}{(isCritical ? " 暴击!" : "")}</color>");
        }

        protected IEnumerator FlashRed()
        {
            if (_spriteRenderer == null) yield break;

            Color originalColor = _spriteRenderer.color;
            _spriteRenderer.color = Color.red;

            yield return new WaitForSeconds(0.1f);

            _spriteRenderer.color = originalColor;
        }
        #endregion

        #region Death
        protected virtual void Die()
        {
            if (IsDead) return;

            IsDead = true;
            _currentState = EnemyState.Dead;

            AudioHub.Play("Enemy_Death");

            // TODO: 播放死亡动画
            // TODO: 掉落物品/铭契

            Debug.Log($"{gameObject.name} 死亡");

            // 销毁对象
            Destroy(gameObject, 0.5f);
        }
        #endregion

        #region Gizmos
        protected virtual void OnDrawGizmosSelected()
        {
            // 检测范围
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // 攻击范围
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // 追击范围
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
        #endregion
    }
}
