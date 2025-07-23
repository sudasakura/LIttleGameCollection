using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nail : MonoBehaviour
{
    public enum NailState //钉子状态枚举
    {
        Inserted,
        Selected,
        Flying
    }
    public NailState currentState = NailState.Inserted; //当前状态

    public Hole currentHole;//当前所钉入的孔洞
    private SpriteRenderer _spriteRenderer;
    public Sprite nailInsertSprite; //钉子插入图片
    public Sprite nailPullSprite; //钉子拔出图片
    private Collider2D _collider2D;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
        
        currentHole.Occupy(GetComponent<Rigidbody2D>());//开始时初始化，让钉子钉入孔洞
        //currentHole没有初始化，现在是手动挂上去，后面优化
        
    }

    public void Select() //选中钉子
    {
        if (currentState == NailState.Inserted)
        {
            currentState = NailState.Selected;
            _spriteRenderer.sprite = nailPullSprite;
        }
    }

    public void Unselect() //取消选中
    {
        if (currentState == NailState.Selected)
        {
            currentState = NailState.Inserted;
            _spriteRenderer.sprite = nailInsertSprite;
        }
    }
    
    public void FlyToHole(Hole targetHole, float duration = 0.3f) //钉子飞入孔洞
    {
        currentState = NailState.Flying;
        StartCoroutine(FlyCoroutine(targetHole, duration));
    }
    
    private IEnumerator FlyCoroutine(Hole targetHole, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = targetHole.transform.position;

        _collider2D.enabled = false;
        
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration; //匀速移动
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        _collider2D.enabled = true;

        if (currentHole != null)//如果还链接着旧孔洞，断开
        {
            currentHole.Vacate();
        }
        currentHole = targetHole;//绑定新孔洞
        currentHole.Occupy(GetComponent<Rigidbody2D>());
        
        currentState = NailState.Inserted;
        _spriteRenderer.sprite = nailInsertSprite;
    }

    public bool IsBusy() => currentState == NailState.Flying;
}