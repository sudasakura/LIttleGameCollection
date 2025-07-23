using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 调用的时候需要EventManager.Instance.TriggerEvent(xxx) 实例化很麻烦，我们改用扩展方法优化
/// 就可以直接.方法名 出来
/// </summary>
public static class EventManagerExtend 
{                           
                        
    public static void TriggerEvent(this object sender,string eventName)//this指向的object类，所以所有子类都能直接.TriggerEvent调用
    {
        EventManager.Instance.TriggerEvent(sender,eventName);
        Debug.Log($"{sender}触发了事件{eventName}");
    }

    public static void TriggerEvent(this object sender,string eventName,EventArgs args)
    {
        EventManager.Instance.TriggerEvent(sender,eventName,args);
        Debug.Log($"{sender}触发了事件{eventName}");
    }
}

/// <summary>
///存全局事件名的类
/// nameof()表达式可生成变量、类型或成员的名称作为字符串常量，（可以.代码提示跳出来）
/// </summary>
public static class eventName
{
    //public const string BallFallenFinshEvent = nameof(BallFallenFinshEvent);

    //public const string MargeBallEvent = nameof(MargeBallEvent);

    public const string GetScoreEvent = nameof(GetScoreEvent);
    public const string GameEndEvent = nameof(GameEndEvent);


}

/// <summary>
///自定义事件类 传EventArgs类型参数时需要自己定义一个继承自EventArgs的类，规定好要传的参数类型和个数
/// </summary>
public class IntEventArgs : EventArgs
{
    public int intMessage;
    public IntEventArgs(int intmessage)
    {
        intMessage = intmessage;
    }

}

/// <summary>
/// 事件管理器是把所有事件放一个字典里，提供一个通用的订阅方法，触发方法
// 这样就不用在要触发某个事件的时候去获取注册这个事件的那个类的实例
///比如我们之前写的小球触发事件，就要得到小球实例：circleBall.OnBallFalleFinishEvent(ballObj,EventArgs.Empty);
///如果后期加上UI上的触发事件，等等，就需要很多很多实例，类与类之间耦合性高
///
/// ps:事件管理器不是把所有类里的跟事件有关的代码的都挪进来哈（写的一些屁用咩呀的搞笑东西）
/// </summary>
public class EventManager : Singleton<EventManager>
{
    /// <summary>
    /// 存放事件的字典 key = 事件名 value = 事件
    /// 这里也可以把string改为enum,但是大量enum可能产生拆箱装箱的性能损耗（也不多）
    /// </summary>
    private Dictionary<string,EventHandler> handlerDic = new Dictionary<string, EventHandler>();

    private Stack<string> _eventSendingStack = new Stack<string>(); // 事件发送堆栈，防止递归触发
    private const int MAX_EVENT_STACK = 3; // 限制最大次数，防止无限递归

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventHandler">这个事件里会传入触发事件后的处理方法函数</param>
    public void AddListener(string eventName, EventHandler eventHandler)
    {
        if(handlerDic.ContainsKey(eventName))
        {
            handlerDic[eventName] += eventHandler;
        }
        else
        {
            handlerDic.Add(eventName,eventHandler);
        }

    }
    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="eventName"></param>
    public void RemoveListener(string eventName,EventHandler eventHandler)
    {
        if(handlerDic.ContainsKey(eventName))
        {            
            handlerDic[eventName] -= eventHandler;

            if(handlerDic[eventName] == null)
            {
                handlerDic.Remove(eventName);
            }
        }       

    }

    /// <summary>
    /// 触发事件（无参）
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="sender"></param>
    public void TriggerEvent(object sender,string eventName)
    {
        if(_eventSendingStack.Count >= MAX_EVENT_STACK)
        {
            Debug.Log($"事件{eventName}递归调用过深，触发终止！");
            return;
        }

        if(handlerDic.ContainsKey(eventName))
        {
            _eventSendingStack.Push(eventName);

            var eventList = handlerDic[eventName].GetInvocationList();
            foreach(Delegate d in eventList)
            {
                if(d is EventHandler handler)
                {
                    handler?.Invoke(sender,EventArgs.Empty);
                }
            }
            //handlerDic[eventName]?.Invoke(sender,EventArgs.Empty);
            _eventSendingStack.Pop();
        }

    }
    /// <summary>
    /// 触发事件（带参）
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void TriggerEvent(object sender,string eventName,EventArgs args)
    {
        if(_eventSendingStack.Count >= MAX_EVENT_STACK)
        {
            Debug.Log($"事件{eventName}递归调用过深，触发终止！");
            return;
        }

        if(handlerDic.ContainsKey(eventName))
        {
            _eventSendingStack.Push(eventName);

            var eventList = handlerDic[eventName].GetInvocationList();
            foreach(Delegate d in eventList)
            {
                if(d is EventHandler handler)
                {
                    handler?.Invoke(sender,args);
                }
            }
            //handlerDic[eventName]?.Invoke(sender,args);
            _eventSendingStack.Pop();
        }
    }
    /// <summary>
    /// 清空事件字典
    /// </summary>
    public void Clear()
    {
        handlerDic.Clear();
    }
}
