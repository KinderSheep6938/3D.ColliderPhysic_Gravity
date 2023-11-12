/// -----------------------------------------------------------------
/// ReadOnlyDrawer.cs　エディター拡張 - 編集不可処理
/// 
/// 作成日：2023/11/12
/// 作成者：コピペ https://kazupon.org/unity-no-edit-param-view-inspector/
/// -----------------------------------------------------------------
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(_position, _property, _label);
        EditorGUI.EndDisabledGroup();
    }
}
#endif