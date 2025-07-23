#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class BoardJointBinder : MonoBehaviour
{
    public void AutoBindAnchors()
    {
        //  找到同层级或所有 Hole
        Hole[] holes = FindObjectsOfType<Hole>();
        HingeJoint2D[] joints = GetComponents<HingeJoint2D>();

        foreach (var joint in joints)
        {
            // 把 joint.transform 的世界坐标 (= board.transform.TransformPoint(anchor)) 
            // 找最近的 hole，设为绑定目标
            Vector2 jointWorldPos = transform.TransformPoint(joint.anchor);
            Hole nearest = null;
            float minDist = Mathf.Infinity;
            foreach (var h in holes)
            {
                float d = Vector2.Distance(jointWorldPos, h.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = h;
                }
            }

            if (nearest != null)
            {
                // 在编辑器里高亮或者给个提示
                Debug.Log($"Joint {joint.name} 绑定到 Hole {nearest.name}");
            }
        }
        // （如果需要把绑定信息保存到某处，可自行扩展）
        EditorUtility.SetDirty(this);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }

    [ContextMenu("自动 Bind Joint Anchor")]
    private void ContextBind() => AutoBindAnchors();
}
#endif

