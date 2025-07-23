using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    public Queue<T> pool;//存放对象的队列
    
    public int poolSize;

    public GameObject prefab;

    public Transform parentTransform;

    public ObjectPool(int poolSize)//初始化队列
    {
        this.poolSize = poolSize;       
        this.pool = new Queue<T>();
        this.parentTransform = null;

        for(int i = 0; i < poolSize; i ++)
        {
            T obj = CreatNewObject();
            pool.Enqueue(obj);
        }

    }
    public T CreatNewObject()//生成对象
    {
        T obj = GameObject.Instantiate(prefab,parentTransform).GetComponent<T>();
        obj.gameObject.SetActive(false);

        return obj;
        
    }

    public T GetObject()//从池中取出
    {
        if(pool.Count > 0)
        {
            T obj = pool.Peek();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            T obj = CreatNewObject();
            obj.gameObject.SetActive(true);
            return obj;
        }

    }
    public void PutObject(T obj)//放回池中
    {
        obj.gameObject.SetActive(false);
    }
}
