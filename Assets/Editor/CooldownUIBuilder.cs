using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// 编辑器工具：把冷却按钮"烘焙"成真实场景物体，挂在 UICanvas 下，
// 之后可在 Hierarchy 里像 PlayerHPBar 一样自由拖拽位置、调整大小。
public static class CooldownUIBuilder
{
    [MenuItem("Tools/生成冷却UI到场景")]
    public static void Build()
    {
        UIController ui = Object.FindObjectOfType<UIController>();
        if (ui == null)
        {
            Debug.LogError("当前场景里没有 UICanvas（UIController），请先打开 Scene1 再执行");
            return;
        }

        SkillCooldownUI cdUI = ui.GetComponent<SkillCooldownUI>();
        if (cdUI == null)
            cdUI = Undo.AddComponent<SkillCooldownUI>(ui.gameObject);

        cdUI.BuildAll();

        EditorSceneManager.MarkSceneDirty(ui.gameObject.scene);
        Selection.activeGameObject = cdUI.gameObject;
        Debug.Log("冷却UI已生成到 UICanvas 下（AttackCDButton / DashCDButton），可自由拖拽、缩放。记得 Ctrl+S 保存场景。", cdUI);
    }
}
