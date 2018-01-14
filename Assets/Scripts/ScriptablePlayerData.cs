using System.Collections.Generic;
using UnityEngine;

public class ScriptablePlayerData : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    public float health;

    [SerializeField]
    public string name;

    [SerializeField]
    public Vector3 startingPosition;

    public Dictionary<string, int> inventory;

    [HideInInspector]
    [SerializeField]
	public List<string> keys;

	[HideInInspector]
    [SerializeField]
    public List<int> values;

    public void OnBeforeSerialize()
    {
        keys = new List<string>();
        values = new List<int>();

        foreach (var keyValuePair in inventory)
        {
            keys.Add(keyValuePair.Key);
            values.Add(keyValuePair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        inventory = new Dictionary<string, int>(keys.Count);

        for (int i = 0; i < keys.Count; i++)
        {
            inventory[keys[i]] = values[i];
        }
    }
}
