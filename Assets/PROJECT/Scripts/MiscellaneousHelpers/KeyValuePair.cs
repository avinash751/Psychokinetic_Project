using System;

[Serializable]
public class KeyValuePair
{
    public string key; 
    public float value;

    public KeyValuePair(string key, float value)
    {
        this.key = key;
        this.value = value;
    }
}