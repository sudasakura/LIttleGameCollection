using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class CircleBall : MonoBehaviour
{
    public int ballType;//球类型
    public int maxBallType = 8;//最大球类型
    //public GameObject ballObj;//球实例
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    public Camera _camera;
    public bool isMarge = false;//是否合成 
    public bool isfalling = false;//是否下落 
    public bool isFalleFinish = false; //是否下落完成
    private BallSpawner spawner; 
    private void Start()
    {
        //if(ballObj == null) ballObj = gameObject;
        
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _camera = Camera.main;    
        spawner = GameObject.Find("spawner").GetComponent<BallSpawner>();    

        //EventManager.Instance.AddListener(eventName.BallFallenFinshEvent,HandleBallFallenFinishedEvent);//订阅事件
        //EventManager.Instance.AddListener(eventName.MargeBallEvent,HandleMargeBallEvent);                                             
    }
    
    private void Update()
    {
        if(isfalling == true) return;
        //输入检测(移动控制)
        if(Input.GetMouseButton(0))
        {
            //屏幕坐标系计算
            // float mouseX = Input.mousePosition.x;
            // float screenWidth = Screen.width;
            // float ballHalfWidth = _spriteRenderer.bounds.size.x / 2;
            // float clampMouseX = Mathf.Clamp(mouseX, ballHalfWidth, screenWidth - ballHalfWidth);
            // transform.position = new Vector3(clampMouseX, transform.position.y, transform.position.z);

            //世界坐标系下计算（2D正交摄像机)
            Vector3 mousePos = Input.mousePosition;//鼠标位置
            mousePos.z = Mathf.Abs(_camera.transform.position.z);
            Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(mousePos);//鼠标坐标转为世界坐标
            float ballHalfWidth = _spriteRenderer.bounds.size.x / 2;//获取小球半径
            
            float screenWidthWorld = (_camera.orthographicSize * 2) * _camera.aspect;//世界坐标的屏幕宽度，也就是摄像机视野宽度
            //Camera.orthographicSize 表示摄像机视野中垂直方向的半高 orthographicSize * 2 就是摄像机高度
            //摄像机的宽高比（Camera.aspect）等于屏幕宽度除以屏幕高度 所以摄像机高 = (_camera.orthographicSize * 2) * _camera.aspect

            float screenWidthWorldL = _camera.transform.position.x - screenWidthWorld / 2;//屏幕左边界（世界坐标下）
            float screenWidthWorldR = _camera.transform.position.x + screenWidthWorld / 2;//屏幕右边界（世界坐标下）

            float clampMouseX = Mathf.Clamp(mouseWorldPos.x, screenWidthWorldL + ballHalfWidth,
                screenWidthWorldR - ballHalfWidth);//限制鼠标位置

            transform.position = new Vector3(clampMouseX, transform.position.y, transform.position.z);
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("鼠标松开");
            isfalling = true;
            _rigidbody2D.gravityScale = 1;
        }
    }

    //碰撞检测
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(!isfalling) return;//鼠标没松开之前不执行碰撞检测的逻辑

        CircleBall otherBall = other.gameObject.GetComponent<CircleBall>();
        if(other.gameObject.tag == "Ball" && otherBall?.ballType == ballType)//触碰到同类型小球
        {             
            if (isMarge || (otherBall != null && otherBall.isMarge)) //双边标记法：确保两个小球都不在合成中
            {
                return;
            }
            isMarge = true;
            otherBall.isMarge = true;//把另一个小球也标记为已经合成，避免触发两次合成            
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            otherBall.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            spawner.BallMarge(this);//不用事件直接就合成了 
            //this.TriggerEvent(eventName.MargeBallEvent);//触发小球合成事件                                 
            //合成动画
            StartCoroutine(HandleWaitForDestory(gameObject,0.1f));
            StartCoroutine(HandleWaitForDestory(other.gameObject,0.1f));
            //Destroy(gameObject);//小球销毁
            //Destroy(other.gameObject);//另一个小球也销毁
        }
        else if(other.gameObject.tag == "Round" || 
                (other.gameObject.tag == "Ball" && otherBall?.ballType != ballType))//触碰到地面or触碰到其他类型小球
        {  
            if (isMarge || (otherBall != null && otherBall.isMarge)) //确保两个小球都不在合成中
            {
                return; //这句很关键，可以防止合成小球和落地同时触发，生成多个小球
            }         
            if (!isFalleFinish)
            {
                Debug.Log("落地");
                isFalleFinish = true; 
                spawner.BallFallenFinished(this,this.gameObject);      
                //this.TriggerEvent(eventName.BallFallenFinshEvent);//触发小球落地事件        
                
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Top" && isfalling)
        {
            EventManager.Instance.TriggerEvent(this,eventName.GameEndEvent);//触发游戏结束                        
        }
    } 

    private IEnumerator HandleWaitForDestory(GameObject ballObj,float time)// 等待足够时间后销毁小球
    {
        // 等待足够时间
        yield return new WaitForSeconds(time);
        Debug.Log("等待"+time+"秒 销毁");

        Destroy(ballObj);       
    }   

    // public void HandleMargeBallEvent(object sender,EventArgs e)//触发合成事件的处理方法
    // { 
    //     if(sender != this)  return;//事件管理器里的触发逻辑是全局广播,所有小球都订阅了这个事件，所以事件触发时所有小球都会响应
    //     CircleBall circleball = sender as CircleBall;
    //     Debug.Log("收到小球合成事件" + sender);
    //     if (circleball != null)
    //     {    
    //         EventManager.Instance.RemoveListener(eventName.MargeBallEvent,HandleMargeBallEvent);//取消该小球的订阅                         
    //         spawner.BallMarge(circleball);            
    //     }        
    // }  
    // public void HandleBallFallenFinishedEvent(object sender,EventArgs e)//触发小球落地事件的处理方法  
    // {
    //     if(sender != this)  return;
    //     CircleBall circleball = sender as CircleBall;
    //     Debug.Log("收到当前小球落地事件"+ sender);
    //     if(circleball != null)
    //     {
    //         EventManager.Instance.RemoveListener(eventName.BallFallenFinshEvent,HandleBallFallenFinishedEvent);//取消该小球的订阅
    //         spawner.BallFallenFinished(circleball);
    //     }
    // }
    
    // private void OnDestroy()//取消订阅
    // {
    //     EventManager.Instance.RemoveListener(eventName.BallFallenFinshEvent,HandleBallFallenFinishedEvent);
    //     EventManager.Instance.RemoveListener(eventName.MargeBallEvent,HandleMargeBallEvent);
    // }
}