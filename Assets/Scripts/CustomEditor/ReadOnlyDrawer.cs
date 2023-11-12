/// -----------------------------------------------------------------
/// ReadOnlyDrawer.cs�@�G�f�B�^�[�g�� - �ҏW�s����
/// 
/// �쐬���F2023/11/12
/// �쐬�ҁF�R�s�y https://kazupon.org/unity-no-edit-param-view-inspector/
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