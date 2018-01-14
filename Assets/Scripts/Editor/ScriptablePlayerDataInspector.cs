using System;
using System.Linq.Expressions;
using UnityEditor;

[CustomEditor(typeof(ScriptablePlayerData))]
public class ScriptablePlayerDataInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var scriptablePlayerData = (ScriptablePlayerData) target;
        var localSerializedObject = new SerializedObject(target);

        SerializedProperty nameProperty = localSerializedObject.FindProperty(GetMemberName(() => scriptablePlayerData.name));
        var healthProperty = localSerializedObject.FindProperty(GetMemberName(() => scriptablePlayerData.health));
        var positionProperty = localSerializedObject.FindProperty(GetMemberName(() => scriptablePlayerData.startingPosition));

        EditorGUILayout.PropertyField(nameProperty);
        EditorGUILayout.PropertyField(healthProperty);

        positionProperty.isExpanded =
            EditorGUILayout.Foldout(positionProperty.isExpanded, "Starting Position:");

        if (positionProperty.isExpanded)
        {
            positionProperty.vector3Value = EditorGUILayout.Vector3Field("", positionProperty.vector3Value);
        }

        localSerializedObject.ApplyModifiedProperties();
    }

    public static string GetMemberName<TValue>(Expression<Func<TValue>> memberAccess)
    {
        return ((MemberExpression)memberAccess.Body).Member.Name;
    }
}
