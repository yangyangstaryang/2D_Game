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
            SceneManager.LoadScene("Scene2");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene("Scene1");
        }
    }

    public void ReStart()
    {
        StartCoroutine(WaitToReload());
    }

    IEnumerator WaitToReload()
    {
        yield return new WaitForSeconds(3);
        // 读取最初的场景
        SceneManager.LoadScene("Scene1");
        // 重置玩家
        PlayerController.Instance.ReSet();
    }
    
}
