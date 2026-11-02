using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : Singleton<Menu>
{
    public GameObject menuPanel;
    public Button saveBtn;
    public Button loadBtn;
    public Slider volumeSlider;

    public bool isPause;

    const string SaveFile = "/SaveData.txt";

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
        if (menuPanel != null) menuPanel.SetActive(false);
        Time.timeScale = 1;
        isPause = false;

        // 读取存档文件
        SaveDataSO saveData = SaveSysten.Load(SaveFile);
        if (saveData == null) return;

        // 用 Fade 来跑协程：Fade 是 DontDestroyOnLoad 的，
        // 切场景时协程不会跟着 Menu 一起被销毁（否则会黑屏卡死）
        Fade.Instance.StartCoroutine(LoadRoutine(saveData));
    }

    IEnumerator LoadRoutine(SaveDataSO saveData)
    {
        yield return Fade.Instance.FadeOut();          // ① 先黑屏

        bool needSwitch = !string.IsNullOrEmpty(saveData.sceneName)
                          && saveData.sceneName != SceneManager.GetActiveScene().name;

        if (needSwitch)
        {
            SceneManager.LoadScene(saveData.sceneName); // ② 切到存档所在场景
            yield return null;                          // ③ 关键：等新场景 Awake/Start 跑完
                                                        //    Gate.Start 会先按传送点摆玩家，
                                                        //    所以要等它跑完再覆盖成存档位置
        }

        saveData.Load();                                // ④ 恢复位置 / 血量 / 背包

        yield return Fade.Instance.FadeIn();            // ⑤ 再亮起来
    }

    void ChangeVolume(float value)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.audioSource != null)
            AudioManager.Instance.audioSource.volume = value;
        else if (AudioManagerBack.Instance != null && AudioManagerBack.Instance.audioSource != null)
            AudioManagerBack.Instance.audioSource.volume = value;
    }
}
