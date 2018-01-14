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
    private List<string> _keys;

    [HideInInspector]
    [SerializeField]
    private List<int> _values;

    public void OnBeforeSerialize()
    {
        _keys = new List<string>();
        _values = new List<int>();

        foreach (var keyValuePair in inventory)
        {
            _keys.Add(keyValuePair.Key);
            _values.Add(keyValuePair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        inventory = new Dictionary<string, int>(_keys.Count);

        for (int i = 0; i < _keys.Count; i++)
        {
            inventory[_keys[i]] = _values[i];
        }
    }
}
