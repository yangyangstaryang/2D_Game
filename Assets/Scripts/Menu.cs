using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using Directory = UnityEngine.Windows.Directory;
using File = System.IO.File;
using Input = UnityEngine.Input;

public class Menu : Singleton<Menu>
{
    public GameObject menuPanel;
    public Button saveBtn;
    public Button loadBtn;
    public Slider volumeSlider;

    public bool isPause;

    protected override void Awake()
    {
        base.Awake();
        saveBtn.onClick.AddListener(SaveData);
        loadBtn.onClick.AddListener(LoadData);
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    private void Start()
    {
        volumeSlider.value = AudioManager.Instance.audioSource.volume;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
            isPause = menuPanel.activeSelf;
            Time.timeScale = menuPanel.activeSelf ? 0 : 1;
            
        }
    }
    
    
    
    void SaveData()
    {
        string path = Application.persistentDataPath + "/SaveData";
        Debug.Log("save");
        // 判断存档文件夹是否存在，如果不存在就创造文件夹
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        // 创建存档文件
        FileStream file = File.Create(path + "/savedata.txt");
        
        // 需要存档的2进制数据来写入
        SaveDataSO saveData = ScriptableObject.CreateInstance<SaveDataSO>();
        saveData.playerPos = PlayerController.Instance.transform.position;
        saveData.playerHP = PlayerController.Instance.currentHp;
        
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, JsonUtility.ToJson(saveData));
        
        file.Close();
    }

    void LoadData()
    {
        string path = Application.persistentDataPath + "/SaveData";
        Debug.Log("load");
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(path + "/savedata.txt"))
        {
            FileStream file = File.Open(path + "/savedata.txt", FileMode.Open);
            
            SaveDataSO saveData = ScriptableObject.CreateInstance<SaveDataSO>();
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), saveData);
            PlayerController.Instance.transform.position = saveData.playerPos;
            PlayerController.Instance.currentHp = saveData.playerHP;
            UIController.Instance.SetHpFillAmount((float)saveData.playerHP / PlayerController.Instance.fullHP);
            file.Close();
        }
    }

    void ChangeVolume(float value)
    {
        AudioManagerBack.Instance.audioSource.volume = value;
    }
}
