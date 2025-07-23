using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Hole : MonoBehaviour
{
    public bool IsOccpuied = false;//孔洞是否被占据

    //该孔洞上绑定的木板上的对应的所有Joints
    //public List<HingeJoint2D> jointsToBind = new List<HingeJoint2D>();
    
    //[Header("绑定在此孔洞上的木板")] public List<GameObject> linkedBoards = new List<GameObject>();
    
    //[Header("绑定在此孔洞上的木板")] public List<GameObject> activeJoints = new List<GameObject>();

    // private void Awake()
    // {
    //     if (linkedBoards == null)
    //     {
    //         Debug.Log("Have not Linked Board On Hole !" + gameObject.name);
    //         return;
    //     }
    //     //关卡启动的时候把孔洞对应的木板列上的距离孔洞最近的joint2d绑定到孔洞对应钉子上
    //     foreach (var board in linkedBoards)
    //     {
    //         var joints = board.GetComponents<HingeJoint2D>();//木板上的所有joint2d
    //         foreach (var joint in joints)
    //         {
    //             //把joint2d的anchor的坐标转换到世界坐标，和孔洞坐标进行比对
    //             Vector3 worldAnchor = board.transform.TransformPoint(joint.anchor);
    //             if (Vector2.Distance(worldAnchor, transform.position) < 0.10f)
    //             {
    //                 jointsToBind.Add(joint);//加入孔洞对应joints列表
    //                 Debug.Log("Hole Bind Joint:" + joint.transform.position);

    //             }
    //         }
    //     }
    // }

    public void Occupy(Rigidbody2D nailBody)//钉子插入孔洞时调用
    {
        IsOccpuied = true;//孔洞被占据

        // if (jointsToBind == null)
        // {
        //     Debug.Log(nailBody.name +": New Bind Hole is Empty");
        //     return;
        // }
        
        // foreach (var joint in jointsToBind)
        // {
        //     Debug.Log("Occupy:" + nailBody.name);
        //     joint.connectedBody = nailBody;//joint组件绑定上钉子
        //     joint.enabled = true;//启用
        // }
    }

    public void Vacate()//钉子拔出孔洞时调用
    {
        IsOccpuied = false;//孔洞被清空
        
        // foreach (var joint in jointsToBind)
        // {
        //     Debug.Log("Vacate:"+ joint.connectedBody.name);
        //     joint.enabled = false;//禁用
        //     joint.connectedBody = null;//断开钉子链接
        // }
        // //这里还需要加上：拔出钉子后孔洞上对应的钉子和木板列表也要刷新，清空
        // jointsToBind.Clear();
        // //二编：不能完全clear，可能有一个木板上三个钉子的情况，一颗钉子拔出去之后，再钉另一颗钉子进去时，JointsToBlind还是有之前的值才对
    }
}
