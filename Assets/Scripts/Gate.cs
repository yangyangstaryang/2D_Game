using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    public GameObject entry;
    public string selfName;
    public string targetScene;
    public string targetGate;
    
    // 一开始的时候
    private void Start()
    {
        // 通过标签查找玩家
        GameObject player = GameObject.FindWithTag("Player");
        // 如果玩家不为空，也就是找到了玩家
        if (player != null)
        {
            // 判断 玩家目标点就是自己 那就将玩家移动到自己的入口上 
            if (player.GetComponent<PlayerController>().targetGate == selfName)
            {
                player.transform.position = entry.transform.position;
            }
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果是玩家进入检测范围
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            // 告诉玩家你的目标点是什么
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.targetGate = targetGate;
            // 加载场景
            SceneManager.LoadScene(targetScene);
        }
    }
}
