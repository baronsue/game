using UnityEngine;
using System.Collections;

namespace TerracottaARPG.Systems
{
    /// <summary>
    /// 状态宿主：管理眩晕、减速等状态效果
    /// </summary>
    public class StatusHost : MonoBehaviour
    {
        // 状态属性（只读）
        public bool Stunned { get; private set; }
        public bool Slowed { get; private set; }
        public float SlowPercent { get; private set; }

        [Header("Visual Feedback")]
        public GameObject stunVFX;
        public GameObject slowVFX;

        private Coroutine _stunCoroutine;
        private Coroutine _slowCoroutine;

        public void ApplyStun(float seconds)
        {
            if (Stunned) return;

            if (_stunCoroutine != null)
                StopCoroutine(_stunCoroutine);

            _stunCoroutine = StartCoroutine(StunCO(seconds));
        }

        public void ApplySlow(float seconds, float percent)
        {
            if (_slowCoroutine != null)
                StopCoroutine(_slowCoroutine);

            _slowCoroutine = StartCoroutine(SlowCO(seconds, percent));
        }

        private IEnumerator StunCO(float s)
        {
            Stunned = true;

            // 启用眩晕特效
            if (stunVFX != null)
                stunVFX.SetActive(true);

            Debug.Log($"{gameObject.name} 被眩晕 {s}s");

            yield return new WaitForSeconds(s);

            Stunned = false;

            if (stunVFX != null)
                stunVFX.SetActive(false);

            _stunCoroutine = null;
        }

        private IEnumerator SlowCO(float s, float percent)
        {
            Slowed = true;
            SlowPercent = percent;

            if (slowVFX != null)
                slowVFX.SetActive(true);

            yield return new WaitForSeconds(s);

            Slowed = false;
            SlowPercent = 0f;

            if (slowVFX != null)
                slowVFX.SetActive(false);

            _slowCoroutine = null;
        }

        public void ClearAllStatus()
        {
            if (_stunCoroutine != null)
            {
                StopCoroutine(_stunCoroutine);
                Stunned = false;
                if (stunVFX != null) stunVFX.SetActive(false);
            }

            if (_slowCoroutine != null)
            {
                StopCoroutine(_slowCoroutine);
                Slowed = false;
                SlowPercent = 0f;
                if (slowVFX != null) slowVFX.SetActive(false);
            }
        }
    }
}
