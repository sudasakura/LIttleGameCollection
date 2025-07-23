using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallSpawner : MonoBehaviour
{
    public List<GameObject> ballPrefabList;//小球预制体列表
    public Transform spawnPoint;//生成点 
    private bool isnewBallSpawned = false;// 标记是否已生成新球
    private CircleBall currentSpawnBall = null;//当前的顶部新生成的小球
    private float spawnCooldown = 1f;// 冷却时间
    private float lastSpawnTime = -100f;
    private EventManager eventManager;//事件管理器

    private void Awake()
    {
        eventManager = EventManager.Instance;
    }   
    public void Spawner()//生成随机小球
    {
        isnewBallSpawned = false;
        int randomBallType = Random.Range(0, 6);//生成【0,6)的随机整数
        Debug.Log("生成新小球"+randomBallType);
        GameObject ballObj = Instantiate(ballPrefabList[randomBallType], spawnPoint.position,Quaternion.identity);
        ballObj.transform.SetParent(spawnPoint);
        CircleBall circleBall = ballObj.GetComponent<CircleBall>();
        if (circleBall != null)
        {
            currentSpawnBall = circleBall;
            //circleBall.ballObj = ballObj;//小球实例
            circleBall.isfalling = false;//打开鼠标输入控制移动
            circleBall.isMarge = false;//打开小球合成检测
            circleBall.isFalleFinish = false;//打开小球落地检测
            ballObj.GetComponent<Rigidbody2D>().gravityScale = 0;//关闭重力
                     
        }
    }
    public void Marge(int type,Vector3 lastBallPos)//合成小球
    {
        Debug.Log("合成小球"+ type);
        if(type > ballPrefabList.Count) return;

        GameObject ballObj = Instantiate(ballPrefabList[type], lastBallPos,Quaternion.identity);
        ballObj.transform.SetParent(spawnPoint);
        CircleBall circleBall = ballObj.GetComponent<CircleBall>();
        if (circleBall != null)
        {   
            this.TriggerEvent(eventName.GetScoreEvent,new IntEventArgs(type));//触发计分事件

            if (circleBall.ballType == circleBall.maxBallType)//如果是最大球
            {
                //circleBall.ballObj = ballObj;//小球实例 
                ballObj.GetComponent<Rigidbody2D>().gravityScale = 1;//打开重力
                circleBall.isfalling = true;//关闭鼠标输入控制移动
                circleBall.isMarge = false;//关闭小球合成检测
                circleBall.isFalleFinish = true;//关闭小球落地检测   
                //eventManager.AddListener(eventName.BallFallenFinshEvent,circleBall.HandleBallFallenFinishedEvent);//订阅事件
                //eventManager.TriggerEvent(circleBall,eventName.BallFallenFinshEvent,EventArgs.Empty);//手动触发落地事件
                BallFallenFinished(circleBall,ballObj);
            }
            else
            {
                //circleBall.ballObj = ballObj;//小球实例 
                ballObj.GetComponent<Rigidbody2D>().gravityScale = 1;//打开重力
                circleBall.isfalling = true;//关闭鼠标输入控制移动
                circleBall.isMarge = false;//打开小球合成检测                 
                circleBall.isFalleFinish = false;//打开小球落地检测   
                //eventManager.AddListener(eventName.BallFallenFinshEvent,circleBall.HandleBallFallenFinishedEvent);//订阅事件
                //eventManager.AddListener(eventName.MargeBallEvent,circleBall.HandleMargeBallEvent);
                
            }            
        }        
    }               

    private IEnumerator HandleWaitForSpawner(float time)// 等待足够时间后生成新小球
    {
        // 等待足够时间
        yield return new WaitForSeconds(time);
        Debug.Log("等待"+time+"秒 生成");

        Spawner();//生成新小球 
    }
    private IEnumerator HandleWaitForMarge(float time,int type,Vector3 lastBallPos)// 等待足够时间后合成小球
    {
        // 等待足够时间
        yield return new WaitForSeconds(time);
        Debug.Log("等待"+time+"秒 合成");

        Marge(type,lastBallPos);//生成新小球 
    }
    private IEnumerator HandleWaitForDestory(GameObject ballObj,float time)// 等待足够时间后销毁小球
    {
        // 等待足够时间
        yield return new WaitForSeconds(time);
        Debug.Log("等待"+time+"秒 销毁");

        Destroy(ballObj);       
    }   
    
    public void BallMarge(CircleBall circleball)//触发小球合成事件的处理方法
    {
        StartCoroutine(HandleWaitForMarge(0.2f,circleball.ballType,circleball.transform.position));
    }
    public void BallFallenFinished(CircleBall circleball,GameObject ball)//触发小球落地事件的处理方法
    {                           
        //eventManager.RemoveListenerByTarget(circleball.ballObj,eventName.BallFallenFinshEvent);//取消该小球的订阅 
        if (circleball != null && !circleball.isMarge && isnewBallSpawned == false )
        {
            //eventManager.RemoveListenerByTarget(sender,eventName.BallFallenFinshEvent);//取消小球下落事件订阅
            if (circleball.ballType == circleball.maxBallType)//最大小球就销毁
            {
                StartCoroutine(HandleWaitForDestory(ball, 0.2f));//等待一段时间 销毁
            }
            //Debug.Log("isfalling: "+ currentSpawnBall.isfalling +",isFalleFinish: "+ currentSpawnBall.isFalleFinish);        
            if (currentSpawnBall != null)//这句不等于if(!currentSpawnBall)哈，终于找出来错在哪了
            {
                if (currentSpawnBall.isfalling == false && currentSpawnBall.isFalleFinish == false)
                {
                    Debug.Log("当前有顶部还没下落的小球");  
                    return;//判断如果当前有顶部还没下落的小球就不生成
                }
            }            
            isnewBallSpawned = true;
            StartCoroutine(HandleWaitForSpawner(0.2f));//等待一段时间 生成
        } 
               
    }
}