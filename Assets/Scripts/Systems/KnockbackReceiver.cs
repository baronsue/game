using UnityEngine;

namespace TerracottaARPG.Systems
{
    /// <summary>
    /// 击退接收器：处理物理击退效果
    /// </summary>
    public class KnockbackReceiver : MonoBehaviour
    {
        [Header("Settings")]
        public Rigidbody2D rb;
        public float knockbackResistance = 0f; // 0-1，击退抗性

        [Header("Limits")]
        public float maxKnockbackForce = 20f;

        private void Awake()
        {
            if (!rb) rb = GetComponent<Rigidbody2D>();
        }

        public void AddImpulse(Vector2 dir, float force)
        {
            if (!rb) return;

            // 应用击退抗性
            float effectiveForce = force * (1f - knockbackResistance);
            effectiveForce = Mathf.Min(effectiveForce, maxKnockbackForce);

            rb.AddForce(dir.normalized * effectiveForce, ForceMode2D.Impulse);
        }

        public void AddKnockback(Vector2 direction, float force, float duration = 0.2f)
        {
            if (!rb) return;

            float effectiveForce = force * (1f - knockbackResistance);
            effectiveForce = Mathf.Min(effectiveForce, maxKnockbackForce);

            rb.velocity = Vector2.zero;
            rb.AddForce(direction.normalized * effectiveForce, ForceMode2D.Impulse);
        }
    }
}
