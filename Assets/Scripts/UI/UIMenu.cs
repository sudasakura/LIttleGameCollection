using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : UIBase
{
    private Button startBtn;
    private Text scoreText;


    public override void Init()
    {
        startBtn = transform.Find("StartButton").GetComponent<Button>();
        scoreText = transform.Find("ScoreText").GetComponent<Text>();
        

        startBtn.onClick.AddListener(OnStartButtonClick);

        base.Init();
    }

    public override void Open()
    {      
        scoreText.text = "HighestScore: "+PlayerPrefs.GetInt("MaxScore").ToString();      
        base.Open();
    }
    public override void Close()
    {
        base.Close();
    }
    private void OnStartButtonClick()
    {
        Debug.Log("游戏开始！");
        
        SceneManager.LoadScene("LevelScene"); 
    
        UIManager.Instance.LoadUI(UIName.UILevel);
        UIManager.Instance.OpenUI(UIName.UILevel);
        UIManager.Instance.CloseUI(UIName.UIMenu);

    }

    public void OnDestroy()
    {
        startBtn.onClick.RemoveListener(OnStartButtonClick);
    }

}
