using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 简化版游戏管理器（无命名空间，方便初学者使用）
/// 管理游戏流程、关卡、角色等
/// </summary>
public class SimpleGameManager : MonoBehaviour
{
    private static SimpleGameManager _instance;
    public static SimpleGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SimpleGameManager");
                _instance = go.AddComponent<SimpleGameManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [Header("Game State")]
    public GameState currentState = GameState.InGame;
    public int currentLevel = 1;
    public float gameTime = 0f;

    public enum GameState
    {
        MainMenu,
        InGame,
        Paused,
        GameOver
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

        Debug.Log("✅ SimpleGameManager 初始化成功！");
    }

    private void Update()
    {
        if (currentState == GameState.InGame)
        {
            gameTime += Time.deltaTime;

            // ESC暂停游戏
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
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
    /// 获取当前关卡名称
    /// </summary>
    public string GetCurrentLevelName()
    {
        return "地宫一层 · 兵马俑坑（测试场景）";
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
}
