using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ScrewLevelEditor : EditorWindow
{
    private int selectedLayer = 0;
    private List<GameObject> boards = new List<GameObject>();
    private List<GameObject> nails = new List<GameObject>();
    private string levelName = "NewLevel";
    private bool isCreatingNail = false;

    [MenuItem("Tools/Screw Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<ScrewLevelEditor>("螺丝关卡编辑工具");
    }

    private void OnGUI()
    {
        //GUILayout.Label("关卡编辑器", EditorStyles.boldLabel);

        if (GUILayout.Button("生成木板"))
        {
            CreateBoard();
        }

        selectedLayer = EditorGUILayout.IntField("木板层级", selectedLayer);

        if (GUILayout.Button("生成钉子"))
        {
            isCreatingNail = true;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        GUILayout.Space(10);
        levelName = EditorGUILayout.TextField("关卡名称", levelName);
        if (GUILayout.Button("保存关卡"))
        {
            SaveLevel();
        }

        if (GUILayout.Button("加载关卡"))
        {
            LoadLevel();
        }
    }

    private void OnDisable()
    {
        // 窗口关闭时注销事件
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void CreateBoard()
    {
        GameObject board = new GameObject("Board");
        Undo.RegisterCreatedObjectUndo(board, "创建木板");
        board.transform.position = Vector3.zero;
        board.transform.rotation = Quaternion.identity;
        board.layer = selectedLayer;

        Rigidbody2D rb = board.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        // 此处可加载预设或设置 PolygonCollider2D 的具体形状
        board.AddComponent<PolygonCollider2D>();

        boards.Add(board);
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isCreatingNail)
            return;

        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
            GameObject targetBoard = FindBoardAtPosition(mousePos);
            if (targetBoard != null)
            {
                CreateNail(mousePos);
                isCreatingNail = false;
                SceneView.duringSceneGui -= OnSceneGUI;
            }
            e.Use();
        }
        // 绘制一个辅助圆，预览钉子位置
        Handles.color = Color.red;
        Handles.DrawSolidDisc(HandleUtility.GUIPointToWorldRay(e.mousePosition).origin, Vector3.forward, 0.1f);
    }

    private GameObject FindBoardAtPosition(Vector2 position)
    {
        foreach (GameObject board in boards)
        {
            Collider2D col = board.GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(position))
            {
                return board;
            }
        }
        return null;
    }

    private void CreateNail(Vector2 position)
    {
        GameObject nail = new GameObject("Nail");
        Undo.RegisterCreatedObjectUndo(nail, "创建钉子");
        nail.transform.position = position;

        Rigidbody2D rb = nail.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        nail.AddComponent<CircleCollider2D>();

        List<GameObject> overlappingBoards = FindOverlappingBoards(position);
        foreach (GameObject board in overlappingBoards)
        {
            // 为每个木板添加 HingeJoint2D
            HingeJoint2D joint = board.AddComponent<HingeJoint2D>();
            joint.connectedBody = nail.GetComponent<Rigidbody2D>();
            joint.anchor = board.transform.InverseTransformPoint(position);
        }
        nails.Add(nail);
    }

    private List<GameObject> FindOverlappingBoards(Vector2 position)
    {
        List<GameObject> overlappingBoards = new List<GameObject>();
        foreach (GameObject board in boards)
        {
            Collider2D col = board.GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(position))
            {
                overlappingBoards.Add(board);
            }
        }
        return overlappingBoards;
    }

    private void SaveLevel()
    {
        List<LevelData.BoardData> boardDataList = new List<LevelData.BoardData>();
        List<LevelData.NailData> nailDataList = new List<LevelData.NailData>();

        foreach (GameObject board in boards)
        {
            boardDataList.Add(new LevelData.BoardData(board.transform.position, board.transform.rotation.eulerAngles.z, board.layer));
        }
        foreach (GameObject nail in nails)
        {
            nailDataList.Add(new LevelData.NailData(nail.transform.position));
        }

        LevelData levelData = new LevelData(boardDataList, nailDataList);
        string json = JsonUtility.ToJson(levelData, true);
        string path = Application.dataPath + "/Levels/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.WriteAllText(path + levelName + ".json", json);
        Debug.Log("关卡已保存：" + levelName);
    }

    private void LoadLevel()
    {
        string path = Application.dataPath + "/Levels/" + levelName + ".json";
        if (!File.Exists(path))
        {
            Debug.LogError("找不到关卡文件：" + path);
            return;
        }

        // 清除现有对象
        foreach (GameObject board in boards)
            Undo.DestroyObjectImmediate(board);
        boards.Clear();
        foreach (GameObject nail in nails)
            Undo.DestroyObjectImmediate(nail);
        nails.Clear();

        string json = File.ReadAllText(path);
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        foreach (LevelData.BoardData boardData in levelData.boards)
        {
            GameObject board = new GameObject("Board");
            Undo.RegisterCreatedObjectUndo(board, "加载木板");
            board.transform.position = boardData.position;
            board.transform.rotation = Quaternion.Euler(0, 0, boardData.rotationZ);
            board.layer = boardData.layer;
            Rigidbody2D rb = board.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            board.AddComponent<PolygonCollider2D>();
            boards.Add(board);
        }
        foreach (LevelData.NailData nailData in levelData.nails)
        {
            GameObject nail = new GameObject("Nail");
            Undo.RegisterCreatedObjectUndo(nail, "加载钉子");
            nail.transform.position = nailData.position;
            Rigidbody2D rb = nail.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            nail.AddComponent<CircleCollider2D>();
            nails.Add(nail);
        }
        Debug.Log("关卡已加载：" + levelName);
    }

    [System.Serializable]
    private class LevelData
    {
        public List<BoardData> boards;
        public List<NailData> nails;

        public LevelData(List<BoardData> boards, List<NailData> nails)
        {
            this.boards = boards;
            this.nails = nails;
        }

        [System.Serializable]
        public class BoardData
        {
            public Vector2 position;
            public float rotationZ;
            public int layer;

            public BoardData(Vector2 pos, float rotZ, int layer)
            {
                this.position = pos;
                this.rotationZ = rotZ;
                this.layer = layer;
            }
        }

        [System.Serializable]
        public class NailData
        {
            public Vector2 position;

            public NailData(Vector2 pos)
            {
                this.position = pos;
            }
        }
    }
    
                                                                                      
}
