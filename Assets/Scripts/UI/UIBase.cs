using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    private string uiName;

    public virtual void Init()
    {
        uiName = gameObject.name;
        gameObject.SetActive(false);
    }
    public virtual void Open()
    {
        Debug.Log($"{uiName} is Open");     
        gameObject.SetActive(true);   
    }
    public virtual void Close()
    {
        Debug.Log($"{uiName} is Close"); 
        gameObject.SetActive(false);
    }
}
