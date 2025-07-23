using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailManager : MonoBehaviour
{
    public LayerMask nailLayer;//钉子所在图层，用于射线检测判断
    public LayerMask holeLayer;//孔洞所在图层
    
    private Nail selectedNail = null;//当前选中的钉子
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            
            if(selectedNail != null && selectedNail.IsBusy())
                return;//如果当前钉子正在飞途中，则不做交互直接返回

            //射线检测是否点击到钉子
            Collider2D nailHit = Physics2D.OverlapPoint(worldPos, nailLayer);
            if (nailHit != null && nailHit.TryGetComponent(out Nail clickedNail))
            {
                if (selectedNail == null) //第一次点击钉子，则选中
                {
                    selectedNail = clickedNail;
                    selectedNail.Select();
                }
                else if (selectedNail == clickedNail) //第二次点击钉子，则取消选中
                {
                    selectedNail.Unselect();
                    selectedNail = null;
                }
                else //点了另一颗钉子，切换选中的钉子
                {
                    selectedNail.Unselect();
                    selectedNail = clickedNail;
                    selectedNail.Select();
                }

                return;//点的是钉子就不处理孔洞逻辑，返回
            }

            //检测是否点击到孔洞
                Collider2D holeHit = Physics2D.OverlapPoint(worldPos, holeLayer);
                if(holeHit == null) return;
                if (holeHit != null & holeHit.TryGetComponent(out Hole clickedHole) && selectedNail != null)
                {
                    if (clickedHole.IsOccpuied)//洞已被占用 返回
                    {
                        Debug.Log("this hole is Occupied");
                        return;
                    }
                    
                    selectedNail.FlyToHole(clickedHole);
                    selectedNail = null;//重置当前选中的钉子为空

                }
            }
            
            
        }
    }
