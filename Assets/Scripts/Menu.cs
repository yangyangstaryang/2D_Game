using UnityEngine;
using UnityEngine.UI;

public class Menu : Singleton<Menu>
{
    public GameObject menuPanel;
    public Button saveBtn;
    public Button loadBtn;
    public Slider volumeSlider;

    public bool isPause;

    // 和开始页面共用同一个存档文件
    const string SaveFile = SaveSysten.DefaultFileName;

    protected override void Awake()
    {
        base.Awake();

        // 判空：UI 没绑好也不会一开局就空引用
        if (saveBtn != null) saveBtn.onClick.AddListener(SaveData);
        if (loadBtn != null) loadBtn.onClick.AddListener(LoadData);
        if (volumeSlider != null) volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    private void Start()
    {
        if (volumeSlider != null && AudioManager.Instance != null && AudioManager.Instance.audioSource != null)
            volumeSlider.value = AudioManager.Instance.audioSource.volume;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (menuPanel == null) return;

        menuPanel.SetActive(!menuPanel.activeSelf);
        isPause = menuPanel.activeSelf;
        Time.timeScale = menuPanel.activeSelf ? 0 : 1;
    }

    // ---------------- 存档 ----------------

    void SaveData()
    {
        // 创建存档文件
        SaveDataSO saveData = ScriptableObject.CreateInstance<SaveDataSO>();
        saveData.Save();                        // 收集 场景名 + 位置 + 血量 + 背包
        SaveSysten.Save(saveData, SaveFile);
    }

    // ---------------- 读档 ----------------

    void LoadData()
    {
        if (!SaveSysten.HasSave(SaveFile))
        {
            Debug.LogWarning("[读档] 没有存档文件");
            return;
        }

        if (menuPanel != null) menuPanel.SetActive(false);
        Time.timeScale = 1;
        isPause = false;

        // 切场景 / 恢复数据统一交给 GameFlow（和开始页面的「继续游戏」是同一套逻辑）
        GameFlow.ContinueGame(SaveFile);
    }

    void ChangeVolume(float value)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.audioSource != null)
            AudioManager.Instance.audioSource.volume = value;
        else if (AudioManagerBack.Instance != null && AudioManagerBack.Instance.audioSource != null)
            AudioManagerBack.Instance.audioSource.volume = value;
    }
}
