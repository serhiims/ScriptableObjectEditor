using System;
using UnityEngine;
using System.Collections.Generic;
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

		SerializedProperty keysProperty = localSerializedObject.FindProperty(GetMemberName(() => scriptablePlayerData.keys));
		SerializedProperty valuesProperty = localSerializedObject.FindProperty(GetMemberName(() => scriptablePlayerData.values));

        EditorGUILayout.PropertyField(nameProperty);
        EditorGUILayout.PropertyField(healthProperty);

        positionProperty.isExpanded =
            EditorGUILayout.Foldout(positionProperty.isExpanded, "Starting Position:");

        if (positionProperty.isExpanded)
        {
            positionProperty.vector3Value = EditorGUILayout.Vector3Field("", positionProperty.vector3Value);
        }

		showDictionary (keysProperty, valuesProperty);

        localSerializedObject.ApplyModifiedProperties();
		if (GUI.changed) {
			Debug.Log ("Changed");
			//EditorApplication.SaveScene();
		}
    }

	private void showDictionary(SerializedProperty keys, SerializedProperty values){
		for (int i = 0; i < keys.arraySize; i++) {
			EditorGUILayout.DelayedTextField(keys.GetArrayElementAtIndex(i));
			EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i));
		}
		//EditorGUILayout.PropertyField(m_IntProp, new GUIContent("Int Field"), GUILayout.Height(20));
	}

    public static string GetMemberName<TValue>(Expression<Func<TValue>> memberAccess)
    {
        return ((MemberExpression)memberAccess.Body).Member.Name;
    }
}
