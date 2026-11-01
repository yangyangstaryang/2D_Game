using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 单例的固定写法
    static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    
    public GameObject playerPrefab;
    public GameObject CameraPrefab;

    private GameObject cameraObj;
    
    private void Awake()
    {
        // 将本实例存入公共区域
        // unity单例写法
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    { 
        // 预热淡入淡出遮罩：确保它存在且是透明的，避免开局第一帧被挡住
        Fade.Instance.SetAlphaImmediate(0f);

        Instantiate(playerPrefab);
        
        cameraObj =  Instantiate(CameraPrefab);
        cameraObj.GetComponentInChildren<CinemachineVirtualCamera>().Follow = PlayerController.Instance.transform;
        DontDestroyOnLoad(cameraObj);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Fade.Instance.LoadSceneWithFade("Scene2");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Fade.Instance.LoadSceneWithFade("Scene1");
        }
    }

    public void ReStart()
    {
        if (isReloading) return;      // 死亡瞬间可能被多只怪同时触发，只处理一次
        isReloading = true;
        StartCoroutine(WaitToReload());
    }

    bool isReloading;

    IEnumerator WaitToReload()
    {
        yield return new WaitForSeconds(1);

        yield return Fade.Instance.FadeOut();        // ① 先黑屏，等它完成
        SceneManager.LoadScene("Scene1");
        yield return null;                           // ② 等场景真正切换完成，再重置玩家（否则是在旧场景里重置）

        if (PlayerController.Instance != null)
            PlayerController.Instance.ReSet();

        yield return Fade.Instance.FadeIn();         // ③ 再亮起来

        isReloading = false;
    }
    
}
