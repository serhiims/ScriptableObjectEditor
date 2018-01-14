using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class SerializationTestWindow : EditorWindow
{
    [MenuItem("Window/Serialization Test")]
    public static void OpenWindow()
    {
        var window = EditorWindow.GetWindow<SerializationTestWindow>();
        window.titleContent = new GUIContent("Serialization");
    }

    private ScriptablePlayerData _currPlayerData;

    public void OnGUI()
    {
        AssetStuff();
        EditorGUILayout.Space();

        XmlStuff();
        EditorGUILayout.Space();

        JsonUtilityStuff();
        EditorGUILayout.Space();

        var savePath = CreateSaveDataPath("SerializablePlayerDataDOTNET.json");

        if (GUILayout.Button("Try Save JSON.NET"))
        {
            var sdata = new SimpleSerializableData();
            sdata.health = 200f;
            sdata.name = "John";
            sdata.inventory = new Dictionary<string, int>();
            sdata.inventory["money"] = 150;
            sdata.inventory["arrows"] = 32;

            using (var streamWriter = new StreamWriter(savePath))
            {
                streamWriter.Write(JsonConvert.SerializeObject(sdata));
            }

            AssetDatabase.Refresh();
        }
    }

    private void JsonUtilityStuff()
    {
        var savePath = CreateSaveDataPath("SerializablePlayerData.json");

        if (GUILayout.Button("Try Save JSON"))
        {
            var scriptableObject = CreateTestScriptableObject();
            var jsonString = JsonUtility.ToJson(scriptableObject);

            using (var streamWriter = new StreamWriter(savePath))
            {
                streamWriter.Write(jsonString);
            }
        }
        if (GUILayout.Button("Try Load JSON"))
        {
            if (!File.Exists(savePath))
            {
                EditorUtility.DisplayDialog("No serialized file found", "Please make sure the file exists", "OK");
            }
            else
            {
                ScriptablePlayerData scriptableObjectSaveData = ScriptableObject.CreateInstance<ScriptablePlayerData>();

                using (var textReader = new StreamReader(savePath))
                {
                    JsonUtility.FromJsonOverwrite(textReader.ReadToEnd(), scriptableObjectSaveData);
                }

                Debug.Log(scriptableObjectSaveData.name);
            }
        }
    }

    private void XmlStuff()
    {
        var savePath = CreateSaveDataPath("SerializableData.xml");
        if (GUILayout.Button("Try XML Serialize"))
        {
            var serializableData = new SimpleSerializableData();
            serializableData.health = 200f;
            serializableData.name = "John";

            var xmlSerializer = new XmlSerializer(typeof(SimpleSerializableData));

            using (var streamWriter = new StreamWriter(savePath))
            {
                xmlSerializer.Serialize(streamWriter, serializableData);
            }

            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Try XML Deserialize"))
        {
            if (!File.Exists(savePath))
            {
                EditorUtility.DisplayDialog("No scriptable object", "Please make sure the file exists", "OK");
            }
            else
            {
                var xmlSerializer = new XmlSerializer(typeof(SimpleSerializableData));
                using (var fileStream = new FileStream(savePath, FileMode.Open))
                {
                    var serializableData = (SimpleSerializableData) xmlSerializer.Deserialize(fileStream);

                    var sb = new StringBuilder();

                    sb.AppendLine("health: " + serializableData.health);
                    sb.AppendLine("name: " + serializableData.name);

                    EditorUtility.DisplayDialog("Deserialized: ", sb.ToString(), "OK");
                }
            }
        }
    }

    private string CreateSaveDataPath(string filename)
    {
        return Path.Combine(Application.dataPath, filename);
    }

    private void AssetStuff()
    {
        var fileName = "ScriptablePlayerData.asset";
        var savePath = "Assets/" + fileName;

        if (GUILayout.Button("Try save asset"))
        {
            var scriptableObject = CreateTestScriptableObject();

            AssetDatabase.CreateAsset(scriptableObject, savePath);
        }

        if (GUILayout.Button("Try load asset"))
        {
            _currPlayerData = AssetDatabase.LoadAssetAtPath<ScriptablePlayerData>(savePath);
            if (_currPlayerData == null)
            {
                EditorUtility.DisplayDialog("No scriptable object", "Please make sure the file exists", "OK");
            }
        }

        if (_currPlayerData != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.TextField("Scriptable object is not null:");
            EditorGUILayout.LabelField("Health: " + _currPlayerData.health);
            EditorGUILayout.LabelField("Name: " + _currPlayerData.name);

            int indent = EditorGUI.indentLevel;

            EditorGUI.indentLevel = 1;
            foreach (var keyValuePair in _currPlayerData.inventory)
            {
                EditorGUILayout.LabelField(keyValuePair.Key + " : " + keyValuePair.Value);
            }
            EditorGUI.indentLevel = indent;
        }
    }

    private static ScriptablePlayerData CreateTestScriptableObject()
    {
        var scriptableObject = ScriptableObject.CreateInstance<ScriptablePlayerData>();
        scriptableObject.health = 100f;
        scriptableObject.name = "Nick";
        scriptableObject.inventory = new Dictionary<string, int>(3);
        scriptableObject.inventory["money"] = 150;
        scriptableObject.inventory["arrows"] = 15;
        scriptableObject.inventory["shovels"] = 2;
        return scriptableObject;
    }
}
