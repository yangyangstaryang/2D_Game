using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : Singleton<Menu>
{
    public GameObject menuPanel;
    public Button saveBtn;
    public Button loadBtn;
    public Slider volumeSlider;

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
            Time.timeScale = menuPanel.activeSelf ? 0 : 1;
        }
    }

    void SaveData()
    {
        Debug.Log("save");
    }

    void LoadData()
    {
        Debug.Log("load");
    }

    void ChangeVolume(float value)
    {
        AudioManager.Instance.audioSource.volume = value;
    }
}
