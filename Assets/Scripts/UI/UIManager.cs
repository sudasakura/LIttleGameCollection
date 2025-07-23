using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class UIName
{
    public const string UIMenu = nameof(UIMenu);

    public const string UILevel = nameof(UILevel);

}

public class UIManager : Singleton<UIManager>
{
    private Dictionary<string,UIBase> UIDic = new Dictionary<string, UIBase>();

    private Transform uiRoot;

    public void Start()
    {
        uiRoot = GameObject.Find("UIRoot").transform;

        LoadUI(UIName.UIMenu);
        OpenUI(UIName.UIMenu);
    }
                
    public UIBase LoadUI(string uiName)
    {  
        if(UIDic.ContainsKey(uiName))
        {
            return UIDic[uiName];
        }
        
        GameObject uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/UI/{uiName}.prefab");
        if(uiPrefab == null)
        {
            Debug.LogWarning($"UI预制 '{uiName}' 未找到，请检查路径！");
            return null;
        }

        GameObject uiInstance = Instantiate(uiPrefab,uiRoot,false);
        uiInstance.name = uiPrefab.name;
        UIBase uiBase = uiInstance.GetComponent<UIBase>();
        if(uiBase == null)
        {
            Debug.LogWarning($"UI预制 '{uiName}' 未挂载对应UI脚本 请检查！");
            return null;
        }

        UIDic.Add(uiName,uiBase);
        uiBase.Init();

        return uiBase;        
    }

    public void OpenUI(string uiName)
    {
        if(UIDic.ContainsKey(uiName))
        {
            UIDic[uiName].Open();            
        }
        else
        {
            Debug.LogWarning($"UI '{uiName}' 未注册！");
        }

    }

    public UIBase FindUI(string uiName)
    {
        if(UIDic.TryGetValue(uiName,out UIBase ui))
        {
            return ui;
        }
        else
        {
            Debug.LogWarning($"UI '{uiName}' 未找到！");
            return null;
        }
        
    }
    public void CloseUI(string uiName)
    {
        if(UIDic.ContainsKey(uiName))
        {
            UIDic[uiName].Close();            
        }
        else
        {
            Debug.LogWarning($"UI '{uiName}' 未注册！");
        }
    }

}
