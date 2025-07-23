using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public BallSpawner spawner;  
    
    private void Start()//初始化
    {  
        EventManager.Instance.AddListener(eventName.GameEndEvent,HandleGameEndEvent);//订阅事件
        
        UIManager.Instance.gameObject.GetComponent<Canvas>().worldCamera = Camera.main;//重新绑定摄像机   

        spawner.Spawner();//生成第一个小球
    }
    
    private void HandleGameEndEvent(object sender,EventArgs e)//触发事件之后执行的方法
    {               
        Debug.Log("收到游戏结束事件");
        EventManager.Instance.RemoveListener(eventName.GameEndEvent,HandleGameEndEvent);//移除事件
        int curSocre = UIManager.Instance.FindUI(UIName.UILevel).gameObject.GetComponent<UILevel>().currentScore;
        if(curSocre > PlayerPrefs.GetInt("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore",curSocre);
        }

        SceneManager.LoadScene("LobbyScene"); 
        
        UIManager.Instance.OpenUI(UIName.UIMenu);
        UIManager.Instance.CloseUI(UIName.UILevel);
        //Time.timeScale = 0;
        //UnityEditor.EditorApplication.isPlaying = false;       
    }      
}
