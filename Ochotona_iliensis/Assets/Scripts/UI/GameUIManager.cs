using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("=== 界面面板引用 ===")]
    public GameObject settingsPanel;       //设置界面
    public GameObject gameOverPanel;       //失败结算界面
    public GameObject victoryPanel;        //胜利结算界面

    [Header("=== 设置界面的按钮 ===")]
    public Button continueBtn;             //继续游戏 / 继续关卡
    public Button restartLevelBtn;         //重新当前关卡
    public Button exitBtn;                 //退出游戏 / 退出关卡

    [Header("=== 胜利界面的按钮 ===")]
    public Button nextLevelBtn;            //进入下一关
    public Button victoryMainMenuBtn;       //【新增】胜利界面-返回主菜单按钮

    [Header("=== 失败界面的按钮 ===")]
    public Button gameOverRetryBtn;        //重新关卡按钮
    public Button gameOverMainMenuBtn;     //返回主菜单按钮

    [Header("=== 音量组件 ===")]
    //public Slider volumeSlider;            //音量条

    [Header("=== 状态调试 ===")]
    public bool isMainMenu = false;        //勾选代表是在主菜单，不勾选代表在关卡内

    private bool isSettingsOpen = false;

    void Start()
    {
        //初始化隐藏所有界面
        if (settingsPanel) settingsPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (victoryPanel) victoryPanel.SetActive(false);

        //动态配置设置界面的按钮
        ConfigureSettingsUI();

        //绑定失败界面的按钮
        if (gameOverRetryBtn != null)
        {
            gameOverRetryBtn.onClick.AddListener(RestartCurrentLevel);
        }
        if (gameOverMainMenuBtn != null)
        {
            gameOverMainMenuBtn.onClick.AddListener(ReturnToMainMenuScene);
        }

        //绑定胜利界面的按钮
        if (victoryMainMenuBtn != null)
        {
            victoryMainMenuBtn.onClick.AddListener(ReturnToMainMenuScene);
        }
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null && nextLevelBtn != null)
        {
            nextLevelBtn.onClick.AddListener(levelManager.EnterNextLevel);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("【按下Esc】按键监听成功！"); 

            if (settingsPanel == null) Debug.LogError("错误：SettingsPanel槽位是空的！");

            if (!gameOverPanel.activeSelf && !victoryPanel.activeSelf)
            {
                ToggleSettings();
            }
        }
    }

    //根据场景动态排布“设置界面”的选项
    void ConfigureSettingsUI()
    {
        if (isMainMenu)
        {
            //主菜单：只需要 3 个选项
            if (restartLevelBtn) restartLevelBtn.gameObject.SetActive(false); // 隐藏“重新当前关卡”

            if (continueBtn) { SetButtonText(continueBtn, "继续游戏"); continueBtn.onClick.AddListener(CloseSettings); }
            if (exitBtn) { SetButtonText(exitBtn, "退出游戏"); exitBtn.onClick.AddListener(QuitGameApplication); }
        }
        else
        {
            //关卡内：需要 4 个选项
            if (restartLevelBtn) { restartLevelBtn.gameObject.SetActive(true); restartLevelBtn.onClick.AddListener(RestartCurrentLevel); }

            if (continueBtn) { SetButtonText(continueBtn, "继续关卡"); continueBtn.onClick.AddListener(CloseSettings); }
            if (exitBtn) { SetButtonText(exitBtn, "退出关卡"); exitBtn.onClick.AddListener(ReturnToMainMenuScene); }
        }

        //音量条初始化（这里你可以绑定你的 AudioMixer 或 AudioSource）
        //if (volumeSlider)
        //{
        //    volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        //}
    }

    //快捷修改按钮文本的辅助方法
    void SetButtonText(Button btn, string text)
    {
        Text t = btn.GetComponentInChildren<Text>();
        if (t != null) t.text = text;
    }

    #region 设置界面业务逻辑
    public void ToggleSettings()
    {
        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);

        // 关卡内打开设置需要暂停游戏
        if (!isMainMenu)
        {
            Time.timeScale = isSettingsOpen ? 0f : 1f; // 0代表时空静止，1代表正常
        }
    }

    public void CloseSettings()
    {
        isSettingsOpen = false;
        settingsPanel.SetActive(false);
        if (!isMainMenu) Time.timeScale = 1f;
    }

    void OnVolumeChanged(float value)
    {
        //音量调整逻辑：AudioListener.volume = value;
        Debug.Log($"当前音量调整为: {value}");
    }
    #endregion

    #region 通用交互业务逻辑 (给所有界面上的各种按钮绑定)
    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f; //记得恢复时间流速
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenuScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGameApplication()
    {
        Debug.Log("正在退出游戏程序...");
        Application.Quit();
    }
    #endregion

    #region 外部通知接口（供 LevelManager 触发）
    public void ShowGameOverUI()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0f; //失败后冻结游戏
    }

    public void ShowVictoryUI(bool hasNextLevel)
    {
        if (victoryPanel) victoryPanel.SetActive(true);
        Time.timeScale = 0f; // 胜利后冻结游戏

        // 如果没有下一关了，隐藏“进入下一关”按钮
        if (nextLevelBtn)
        {
            nextLevelBtn.gameObject.SetActive(hasNextLevel);
        }
    }
    #endregion
}