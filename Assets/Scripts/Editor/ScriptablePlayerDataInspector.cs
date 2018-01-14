using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ScriptablePlayerData))]
public class ScriptablePlayerDataInspector : Editor
{
	private SerializedObject _localSerializedObject;
	 
    public override void OnInspectorGUI()
    {
		_localSerializedObject = new SerializedObject(target);
        var scriptablePlayerData = (ScriptablePlayerData) target;

        SerializedProperty nameProperty = _localSerializedObject.FindProperty(GetMemberName(() => scriptablePlayerData.name));
        var healthProperty = _localSerializedObject.FindProperty(GetMemberName(() => scriptablePlayerData.health));
        var positionProperty = _localSerializedObject.FindProperty(GetMemberName(() => scriptablePlayerData.startingPosition));
		EditorGUI.BeginChangeCheck ();
		SerializedProperty keysProperty = _localSerializedObject.FindProperty(GetMemberName(() => scriptablePlayerData.keys));
		SerializedProperty valuesProperty = _localSerializedObject.FindProperty(GetMemberName(() => scriptablePlayerData.values));
	

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
				EditorUtility.SetDirty (target);
			}
		}

		if(EditorGUI.EndChangeCheck()){
			Debug.Log ("OnChanged");
			saveCurrentScene();
		}
    }

	private void saveCurrentScene(){		
		if (_localSerializedObject != null) {
			_localSerializedObject.ApplyModifiedProperties();
		}
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
				EditorUtility.SetDirty (target);
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
