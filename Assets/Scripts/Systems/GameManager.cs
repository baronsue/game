using UnityEngine;
using UnityEngine.SceneManagement;
using TerracottaARPG.Character;
using TerracottaARPG.Data;
using System.Collections.Generic;

namespace TerracottaARPG.Systems
{
    /// <summary>
    /// 游戏管理器：管理游戏流程、关卡、角色等
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        [Header("Player")]
        public TerracottaWarrior playerWarrior;
        public GameObject playerPrefab;

        [Header("Game State")]
        public GameState currentState = GameState.MainMenu;
        public int currentLevel = 1;
        public float gameTime = 0f;

        [Header("Level")]
        public string[] levelScenes = { "Level1_TerracottaPit", "Level2_MercuryRiver", "Level3_BronzeCitadel" };

        [Header("Inscriptions")]
        public List<Inscription> startingInscriptions = new List<Inscription>();

        public enum GameState
        {
            MainMenu,
            CharacterSelect,
            InGame,
            Paused,
            LevelComplete,
            GameOver,
            Victory
        }

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

        private void Update()
        {
            if (currentState == GameState.InGame)
            {
                gameTime += Time.deltaTime;

                // 暂停/恢复
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    TogglePause();
                }
            }
        }

        #region Game Flow
        /// <summary>
        /// 开始新游戏
        /// </summary>
        public void StartNewGame(int characterIndex = 0)
        {
            Debug.Log("<color=cyan>=== 开始新游戏 ===</color>");

            // 重置游戏状态
            currentLevel = 1;
            gameTime = 0f;

            // 清空铭契
            InscriptionManager.Instance.ClearAllInscriptions();

            // 加载第一关
            LoadLevel(0);

            currentState = GameState.InGame;
        }

        /// <summary>
        /// 加载关卡
        /// </summary>
        public void LoadLevel(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= levelScenes.Length)
            {
                Debug.LogError($"无效的关卡索引: {levelIndex}");
                return;
            }

            currentLevel = levelIndex + 1;
            string sceneName = levelScenes[levelIndex];

            Debug.Log($"<color=yellow>加载关卡 {currentLevel}: {sceneName}</color>");

            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// 关卡完成
        /// </summary>
        public void OnLevelComplete()
        {
            currentState = GameState.LevelComplete;

            Debug.Log($"<color=green>=== 关卡 {currentLevel} 完成！===</color>");

            AudioHub.Play("LevelComplete");

            // 显示铭契选择
            ShowInscriptionSelection();
        }

        /// <summary>
        /// 显示铭契选择
        /// </summary>
        private void ShowInscriptionSelection()
        {
            // 获取随机铭契选项
            List<Inscription> options = InscriptionManager.Instance.GetRandomInscriptionOptions(2);

            // 显示UI（需要在场景中有InscriptionSelectionUI）
            var selectionUI = FindObjectOfType<UI.InscriptionSelectionUI>();
            if (selectionUI != null)
            {
                selectionUI.ShowSelection(options);
            }
            else
            {
                Debug.LogWarning("未找到InscriptionSelectionUI，跳过铭契选择");
                OnInscriptionSelected();
            }
        }

        /// <summary>
        /// 铭契选择完成
        /// </summary>
        public void OnInscriptionSelected()
        {
            // 继续下一关或胜利
            if (currentLevel < levelScenes.Length)
            {
                LoadLevel(currentLevel); // currentLevel已经是下一关的索引
            }
            else
            {
                OnGameVictory();
            }
        }

        /// <summary>
        /// 游戏胜利
        /// </summary>
        public void OnGameVictory()
        {
            currentState = GameState.Victory;

            Debug.Log("<color=gold>=== 游戏胜利！===</color>");

            AudioHub.Play("GameVictory");

            // TODO: 显示胜利界面
        }

        /// <summary>
        /// 游戏失败
        /// </summary>
        public void OnGameOver()
        {
            currentState = GameState.GameOver;

            Debug.Log("<color=red>=== 游戏失败 ===</color>");

            AudioHub.Play("GameOver");

            // TODO: 显示失败界面
        }

        /// <summary>
        /// 暂停/恢复游戏
        /// </summary>
        public void TogglePause()
        {
            if (currentState == GameState.InGame)
            {
                currentState = GameState.Paused;
                Time.timeScale = 0f;
                Debug.Log("游戏暂停");
            }
            else if (currentState == GameState.Paused)
            {
                currentState = GameState.InGame;
                Time.timeScale = 1f;
                Debug.Log("游戏恢复");
            }
        }

        /// <summary>
        /// 返回主菜单
        /// </summary>
        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            currentState = GameState.MainMenu;
            SceneManager.LoadScene("MainMenu");
        }
        #endregion

        #region Player Management
        /// <summary>
        /// 生成玩家角色
        /// </summary>
        public void SpawnPlayer(Vector3 position)
        {
            if (playerPrefab == null)
            {
                Debug.LogError("玩家预制体未设置");
                return;
            }

            GameObject playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
            playerWarrior = playerObj.GetComponent<TerracottaWarrior>();

            if (playerWarrior != null)
            {
                playerWarrior.onDeath += OnPlayerDeath;
                InscriptionManager.Instance.InitializePlayer(playerWarrior);

                Debug.Log($"玩家生成于 {position}");
            }
        }

        /// <summary>
        /// 玩家死亡
        /// </summary>
        private void OnPlayerDeath()
        {
            Debug.Log("<color=red>玩家死亡</color>");

            // 延迟一段时间后显示游戏失败
            Invoke(nameof(OnGameOver), 2f);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// 获取当前关卡名称
        /// </summary>
        public string GetCurrentLevelName()
        {
            return currentLevel switch
            {
                1 => "地宫一层 · 兵马俑坑",
                2 => "地宫二层 · 水银天河",
                3 => "地宫三层 · 青铜地阙",
                _ => "未知关卡"
            };
        }

        /// <summary>
        /// 格式化游戏时间
        /// </summary>
        public string GetFormattedGameTime()
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }
        #endregion
    }
}
