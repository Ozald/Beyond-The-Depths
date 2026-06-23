using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContext
{
    private readonly Dictionary<string, object> attributes;

    public EnemyContext()
    {
        attributes = new Dictionary<string, object>();
    }

    /// <summary>
    /// This method allows you to store any type of data in the EnemyContext using a string key. The value is stored as an object, so it can be retrieved later using the same key and cast back to its original type.
    /// </summary>
    /// <typeparam name="T">The type of the value to be stored.</typeparam>
    /// <param name="key">The key used to store the value in the EnemyContext.</param>
    /// <param name="value">The value to be stored in the EnemyContext.</param>
    public void SetAttribute<T>(string key, T value)
    {
        attributes[key] = value;
    }

    /// <summary>
    /// This method allows you to retrieve a value from the EnemyContext using a string key. The value is returned as the specified type T. If the key does not exist or the value cannot be cast to type T, the default value of type T is returned.
    /// </summary>
    /// <typeparam name="T">The type of the value to be retrieved.</typeparam>
    /// <param name="key">The key used to retrieve the value from the EnemyContext.</param>
    /// <returns>The value associated with the specified key, or the default value of type T if the key does not exist or the value cannot be cast to type T.</returns>
    public T GetAttribute<T>(string key)
    {
        if (attributes.TryGetValue(key, out object objValue) && objValue is T)
        {
            return (T)objValue;
        }
        else
        {
            return default(T);
        }
    }

    /// <summary>
    /// This method allows you to delete a value from the EnemyContext using a string key. If the key exists, the associated value is removed from the EnemyContext. If the key does not exist, a warning message is logged indicating that the key was not found and cannot be deleted.
    /// </summary>
    /// <param name="key">The key used to identify the value to be deleted from the EnemyContext.</param>
    public void DeleteAttribute(string key)
    {
        if (attributes.ContainsKey(key))
            attributes.Remove(key);
        else
            Debug.LogWarning($"SharedContext: Key '{key}' not found. Cannot delete.");
    }

    public bool HasAttribute(string key)
    {
        return attributes.ContainsKey(key);
    }
}
