using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEditor.SceneManagement;

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
	
		EditorGUI.BeginChangeCheck ();
        EditorGUILayout.PropertyField(nameProperty);
        EditorGUILayout.PropertyField(healthProperty);

        positionProperty.isExpanded =
            EditorGUILayout.Foldout(positionProperty.isExpanded, "Starting Position:");

        if (positionProperty.isExpanded)
        {
            positionProperty.vector3Value = EditorGUILayout.Vector3Field("", positionProperty.vector3Value);
        }

		keysProperty.isExpanded = EditorGUILayout.Foldout(keysProperty.isExpanded, "Inventory:");
		if (keysProperty.isExpanded) {
			showDictionary (keysProperty, valuesProperty, scriptablePlayerData);
			if (GUILayout.Button ("+", GUILayout.MinWidth (10))) {
				string newKey = "newKey" + keysProperty.arraySize;
				scriptablePlayerData.inventory.Add (newKey, 0);
				saveCurrentScene();
			}
		}

		if(EditorGUI.EndChangeCheck()){
			localSerializedObject.ApplyModifiedProperties();
			saveCurrentScene();
		}
    }

	private void saveCurrentScene(){
		EditorSceneManager.SaveScene (EditorSceneManager.GetActiveScene ());
	}

	private void showDictionary(SerializedProperty keys, SerializedProperty values, ScriptablePlayerData data){
		if (keys == null || values == null) {
			Debug.Log ("Dictionary is empty");
			return;
		}
		for (int i = 0, l = keys.arraySize; i < l; i++) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.PropertyField (keys.GetArrayElementAtIndex(i), GUIContent.none, true, GUILayout.MinWidth(60));
			GUILayout.Space (8);
			EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i), GUIContent.none, true, GUILayout.MinWidth(60));
			GUILayout.Space (8);
			if (GUILayout.Button ("-", GUILayout.MaxWidth(30))) {
				Debug.Log ("Remove key: " + keys.GetArrayElementAtIndex(i).stringValue);
				data.inventory.Remove (keys.GetArrayElementAtIndex (i).stringValue);
				saveCurrentScene();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space ();
		}
	}
		
    public static string GetMemberName<TValue>(Expression<Func<TValue>> memberAccess)
    {
        return ((MemberExpression)memberAccess.Body).Member.Name;
    }
}
