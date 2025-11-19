using UnityEngine;
using System.Collections.Generic;
using TerracottaARPG.Data;
using TerracottaARPG.Character;

namespace TerracottaARPG.Systems
{
    /// <summary>
    /// 铭契管理器：管理玩家当前拥有的所有铭契
    /// </summary>
    public class InscriptionManager : MonoBehaviour
    {
        private static InscriptionManager _instance;
        public static InscriptionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("InscriptionManager");
                    _instance = go.AddComponent<InscriptionManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        [Header("Active Inscriptions")]
        [SerializeField] private List<Inscription> activeInscriptions = new List<Inscription>();

        [Header("Inscription Pool")]
        [SerializeField] private List<Inscription> availableInscriptions = new List<Inscription>();

        private TerracottaWarrior _playerWarrior;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 初始化玩家角色引用
        /// </summary>
        public void InitializePlayer(TerracottaWarrior warrior)
        {
            _playerWarrior = warrior;
        }

        /// <summary>
        /// 添加铭契
        /// </summary>
        public void AddInscription(Inscription inscription)
        {
            if (inscription == null) return;

            activeInscriptions.Add(inscription);

            // 应用效果
            if (_playerWarrior != null)
                inscription.ApplyToWarrior(_playerWarrior);

            Debug.Log($"<color=yellow>获得铭契: {inscription.displayName}</color>");
        }

        /// <summary>
        /// 移除铭契
        /// </summary>
        public void RemoveInscription(Inscription inscription)
        {
            if (inscription == null) return;

            activeInscriptions.Remove(inscription);

            // TODO: 移除效果

            Debug.Log($"<color=gray>失去铭契: {inscription.displayName}</color>");
        }

        /// <summary>
        /// 获取所有激活的铭契
        /// </summary>
        public List<Inscription> GetActiveInscriptions()
        {
            return new List<Inscription>(activeInscriptions);
        }

        /// <summary>
        /// 检查是否拥有某个铭契
        /// </summary>
        public bool HasInscription(string id)
        {
            return activeInscriptions.Exists(i => i.id == id);
        }

        /// <summary>
        /// 随机获取铭契选项（用于关卡结束后的选择）
        /// </summary>
        public List<Inscription> GetRandomInscriptionOptions(int count = 2)
        {
            List<Inscription> options = new List<Inscription>();

            if (availableInscriptions.Count == 0)
            {
                Debug.LogWarning("没有可用的铭契");
                return options;
            }

            // 从铭契池中随机选择
            List<Inscription> pool = new List<Inscription>(availableInscriptions);

            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, pool.Count);
                options.Add(pool[randomIndex]);
                pool.RemoveAt(randomIndex);
            }

            return options;
        }

        /// <summary>
        /// 添加铭契到可用池
        /// </summary>
        public void RegisterInscription(Inscription inscription)
        {
            if (!availableInscriptions.Contains(inscription))
                availableInscriptions.Add(inscription);
        }

        /// <summary>
        /// 清空所有铭契（用于新游戏）
        /// </summary>
        public void ClearAllInscriptions()
        {
            activeInscriptions.Clear();
            Debug.Log("清空所有铭契");
        }

        /// <summary>
        /// 获取铭契统计信息
        /// </summary>
        public Dictionary<InscriptionRarity, int> GetInscriptionStats()
        {
            Dictionary<InscriptionRarity, int> stats = new Dictionary<InscriptionRarity, int>();

            foreach (var inscription in activeInscriptions)
            {
                if (stats.ContainsKey(inscription.rarity))
                    stats[inscription.rarity]++;
                else
                    stats[inscription.rarity] = 1;
            }

            return stats;
        }
    }
}
