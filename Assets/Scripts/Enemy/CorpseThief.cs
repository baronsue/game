using UnityEngine;
using TerracottaARPG.Core;
using TerracottaARPG.Systems;

namespace TerracottaARPG.Character
{
    /// <summary>
    /// 尸化盗贼：第一关普通敌人
    /// 特点：近战、成群出现、较弱
    /// </summary>
    public class CorpseThief : EnemyBase
    {
        [Header("Corpse Thief Settings")]
        public float chargeSpeed = 6f; // 冲锋速度
        public float chargeDistance = 5f; // 触发冲锋的距离
        private bool _isCharging = false;

        protected override void Awake()
        {
            base.Awake();

            // 尸化盗贼属性
            maxHealth = 80f;
            currentHealth = maxHealth;
            moveSpeed = 3.5f;
            attackDamage = 15f;
            attackRange = 1.2f;
            attackCooldown = 1.2f;
            detectionRange = 6f;

            damageType = DamageType.Physical;
            isHeavyAttack = false;
        }

        protected override void ChaseTarget()
        {
            if (_target == null) return;

            float distance = Vector2.Distance(transform.position, _target.position);

            // 在一定距离内触发冲锋
            if (!_isCharging && distance <= chargeDistance && distance > attackRange)
            {
                _isCharging = true;
                AudioHub.Play("CorpseThief_Charge");
            }

            Vector2 direction = (_target.position - transform.position).normalized;
            float speed = _isCharging ? chargeSpeed : moveSpeed;

            // 应用减速
            if (_statusHost.Slowed)
                speed *= (1f - _statusHost.SlowPercent);

            _rb.velocity = direction * speed;

            // 面向目标
            FlipTowardsTarget();

            // 冲锋结束条件
            if (_isCharging && distance <= attackRange)
            {
                _isCharging = false;
            }
        }

        protected override void PerformAttack()
        {
            base.PerformAttack();

            // 尸化盗贼的特殊攻击动作
            AudioHub.Play("CorpseThief_Attack");
        }

        protected override void Die()
        {
            // 掉落少量资源
            Debug.Log("尸化盗贼死亡，掉落陶片");

            base.Die();
        }
    }
}
