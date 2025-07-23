using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
public class UILevel : UIBase
{
    public int currentScore = 0;
    private Text scoreText;
    private int LastScore = 0;    
    public override void Init()
    {        
        scoreText = transform.Find("ScoreText").GetComponent<Text>();
                
        EventManager.Instance.AddListener(eventName.GetScoreEvent,HandleGetScoreEvent);//订阅事件
        base.Init();
    }

    public override void Open()
    {     
        scoreText.text = "Score : " + LastScore.ToString();                   
        base.Open();
    }  
    public override void Close()
    {
        base.Close();
    }
    private void HandleGetScoreEvent(object sender,EventArgs e)//触发事件之后执行的方法
    {               
        Debug.Log("收到计分事件");
        IntEventArgs ie = (IntEventArgs)e;       
        if(ie != null)
        {     
            currentScore = currentScore + LastScore + (ie.intMessage * 100);      
            scoreText.text = "Score : " + currentScore;
            Debug.Log("收到计分事件"+ scoreText.text + ",,,,"+ currentScore);
        }       
    }      

    public void ODestroy()
    {
        EventManager.Instance.RemoveListener(eventName.GetScoreEvent,HandleGetScoreEvent);//移除事件
    }

}
