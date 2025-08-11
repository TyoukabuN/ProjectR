using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class XStreamingEditorCache : ScriptableObject
{
    [Serializable]
    public class KeyValuePair<T,U> {
        public T Key;
        public U Value;
        public KeyValuePair(T key, U value)
        {
            Key = key;
            Value = value; 
        }
    }
    public List<KeyValuePair<string,string>> strings = new List<KeyValuePair<string, string>> ();
    public List<KeyValuePair<string, int>> intgers = new List<KeyValuePair<string, int>>();
    public List<KeyValuePair<string, float>> floats = new List<KeyValuePair<string, float>>();
    public List<KeyValuePair<string, bool>> booleans = new List<KeyValuePair<string, bool>>();
    private bool m_isChanged = false;
    public bool isChanged {
        get { 
            return m_isChanged; 
        }
        set {
            if (value == m_isChanged)
                return;
            m_isChanged = value;
        }
    }

    public string GetString(string key, string defaultValue)
    {
        if (string.IsNullOrEmpty(key))
            return defaultValue;

        if (strings == null)
            strings = new List<KeyValuePair<string, string>>();
        int index = strings.FindIndex(x => x.Key == key);
        if (index < 0)
        {
            strings.Add(new KeyValuePair<string, string>(key, defaultValue));
            return defaultValue;
        }

        return strings[index].Value;
    }

    public void SetString(string key, string value) 
    {
        if (strings == null)
            strings = new List<KeyValuePair<string, string>>();
        int index = strings.FindIndex(x => x.Key == key);
        if (index < 0)
            strings.Add(new KeyValuePair<string, string>(key,value));
        else
            strings[index].Value = value;
        isChanged = true;
    }

    public int GetInt(string key, int defaultValue)
    {
        if (string.IsNullOrEmpty(key))
            return defaultValue;

        if (intgers == null)
            intgers = new List<KeyValuePair<string, int>>();
        int index = intgers.FindIndex(x => x.Key == key);
        if (index < 0)
        {
            intgers.Add(new KeyValuePair<string, int>(key, defaultValue));
            return defaultValue;
        }
        return intgers[index].Value;
    }

    public void SetInt(string key, int value)
    {
        if (intgers == null)
            intgers = new List<KeyValuePair<string, int>>();
        int index = intgers.FindIndex(x => x.Key == key);
        if (index < 0)
            intgers.Add(new KeyValuePair<string, int>(key, value));
        else
            intgers[index].Value = value;

        isChanged = true;
    }

    public float GetFloat(string key, float defaultValue)
    {
        if (string.IsNullOrEmpty(key))
            return defaultValue;

        if (floats == null)
            floats = new List<KeyValuePair<string, float>>();
        int index = floats.FindIndex(x => x.Key == key);
        if (index < 0)
        {
            floats.Add(new KeyValuePair<string, float>(key, defaultValue));
            return defaultValue;
        }

        return floats[index].Value;
    }

    public void SetFloat(string key, float value)
    {
        if (floats == null)
            floats = new List<KeyValuePair<string, float>>();
        int index = floats.FindIndex(x => x.Key == key);
        if (index < 0)
            floats.Add(new KeyValuePair<string, float>(key, value));
        else
            floats[index].Value = value;

        isChanged = true;
    }
    public bool GetBool(string key, bool defaultValue)
    {
        if (string.IsNullOrEmpty(key))
            return defaultValue;

        if (booleans == null)
            booleans = new List<KeyValuePair<string, bool>>();
        int index = booleans.FindIndex(x => x.Key == key);
        if (index < 0) 
        {
            booleans.Add(new KeyValuePair<string, bool>(key, defaultValue));
            return defaultValue;
        }

        return booleans[index].Value;
    }

    public void SetBool(string key, bool value)
    {
        if (booleans == null)
            booleans = new List<KeyValuePair<string, bool>>();
        int index = booleans.FindIndex(x => x.Key == key);
        if (index < 0)
            booleans.Add(new KeyValuePair<string, bool>(key, value));
        else
            booleans[index].Value = value;

        isChanged = true;
    }
}
