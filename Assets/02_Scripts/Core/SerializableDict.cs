using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDict<KEY, VALUE> where KEY : Enum
{
    [SerializeField] private List<SerializeData<KEY, VALUE>> data = new List<SerializeData<KEY, VALUE>>();
    private Dictionary<KEY, VALUE> dict = new Dictionary<KEY, VALUE>();

    public SerializableDict()
    {
        UpDateDict();
    }

    public Dictionary<KEY, VALUE> GetDict()
    {
        UpDateDict();
        return dict;
    }

    public VALUE GetValue(KEY _key)
    {
        UpDateDict();
        return dict[_key];
    }

    public void UpDateDict()
    {
        dict.Clear();

        foreach (var _data in data)
        {
            dict.Add(_data.Key, _data.Value);
        }
    }
}

[Serializable]
public struct SerializeData<KEY, VALUE>
{
    public KEY Key;
    public VALUE Value;
}

