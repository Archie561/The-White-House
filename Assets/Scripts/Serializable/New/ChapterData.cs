using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ChapterData
{
    private static Dictionary<Type, IList> _activities = new();

    public static List<T> GetActivity<T>() where T : IGameActivity
    {
        if (_activities.TryGetValue(typeof(T), out var obj) && obj is List<T> activityList)
        {
            //if (id < 0 || id >= activityList.Count)
                //throw new IndexOutOfRangeException($"Invalid index {id} for activity type {typeof(T).Name}");

            return activityList;
        }

        // Завантаження даних
        var presidentName = "KenRothwell";
        var chapterId = 0;
        var data = Resources.Load<TextAsset>($"Story/{presidentName}/{chapterId}/{PlayerPrefs.GetString("gameLanguage", "en")}/{typeof(T).Name}");

        if (data == null)
            throw new FileNotFoundException($"File not found for activity type {typeof(T).Name}");

        var deserializedList = JsonConvert.DeserializeObject<List<T>>(data.text);
        if (deserializedList == null || deserializedList.Count == 0)
            throw new Exception($"Failed to deserialize or empty list for {typeof(T).Name}");

        _activities[typeof(T)] = deserializedList;

        return deserializedList;
    }
}

public abstract class BaseActivityManager<T> where T : IGameActivity
{
    private static Dictionary<Type, IList> _cache = new();

    protected T GetActivity(int id)
    {
        if (_cache.TryGetValue(typeof(T), out var obj) && (obj is List<T> activityList))
        {
            if (id < 0 || id >= activityList.Count)
                throw new IndexOutOfRangeException($"Invalid index {id} for activity type {typeof(T).Name}");

            return activityList[id];
        }

        LoadActivityList();
        return GetActivity(id);
    }

    private void LoadActivityList()
    {
        var jsonData = Resources.Load<TextAsset>($"Story/{PlayerData.ActivePresident}/{PlayerData.ChapterId}/{PlayerPrefs.GetString("gameLanguage", "en")}/{typeof(T).Name}");
        if (jsonData == null)
            throw new FileNotFoundException($"File not found for activity type {typeof(T).Name}");

        var deserializedData = JsonConvert.DeserializeObject<List<T>>(jsonData.text);
        if (deserializedData == null || deserializedData.Count == 0)
            throw new Exception($"Failed to deserialize or empty list for {typeof(T).Name}");

        _cache[typeof(T)] = deserializedData;
    }
}

public interface IGameActivity
{
    //Skip if Choice[id] != value
}