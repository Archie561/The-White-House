using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Linq;

public static class GameDataManager
{
    private static PlayerData _playerData;

    private static List<ChapterBatch> _chapterBatches;

    private static Dictionary<Type, object> _activities;

    //for debug purposes
    private static List<Dialogue> _dialogues;

    public static string ActivePresident { get; private set; }
    public static bool IsChapterCompleted;

    private static ChapterBatch CurrentBatch => _chapterBatches[_playerData.BatchIndex];

    public static void Load(string activePresident)
    {
        ActivePresident = activePresident;
        string path = Application.persistentDataPath + $"/{activePresident}.json";

        _playerData = File.Exists(path)
            ? JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(path))
            : GenerateNewPlayerData();

        LoadChapterData();
    }

    private static PlayerData GenerateNewPlayerData()
    {
        return new PlayerData
        {
            ChapterID = 0,
            BatchIndex = 0,
            Characteristics = Enum.GetValues(typeof(Characteristic))
                .Cast<Characteristic>()
                .ToDictionary(c => c, _ => 50),
            UsedLawIds = new HashSet<int>(),
            PickedResponses = new HashSet<string>(),
            ProceededLawsCount = 0,
            DialogueIndex = 0
        };
    }

    private static void LoadChapterData()
    {
        string gameLanguage = PlayerPrefs.GetString("gameLanguage", "en");

        _chapterBatches = LoadJsonResource<List<ChapterBatch>>($"Dialogues/{ActivePresident}/{_playerData.ChapterID}/batches");

        _dialogues = LoadJsonResource<List<Dialogue>>($"Dialogues/{ActivePresident}/{_playerData.ChapterID}/{gameLanguage}");
        var allLaws = LoadJsonResource<List<Law>>($"Laws/{gameLanguage}");
        var availableLaws = allLaws.Where(law => !_playerData.UsedLawIds.Contains(law.lawID)).ToList();

        _activities = new Dictionary<Type, object>
        {
            { typeof(Dialogue), new DialogueActivity(_dialogues) },
            { typeof(Law), new LawActivity(availableLaws) }
        };
    }

    private static T LoadJsonResource<T>(string path)
    {
        var data = Resources.Load<TextAsset>(path);
        if (data == null)
        {
            Debug.LogError($"Missing resource at: {path}");
            return default;
        }

        var result = JsonConvert.DeserializeObject<T>(data.text);
        Resources.UnloadAsset(data);
        return result;
    }

    public static void Save()
    {
        TryProceedToNextBatch();

        string jsonData = JsonConvert.SerializeObject(_playerData, Formatting.Indented);
        string path = Application.persistentDataPath + $"/{ActivePresident}.json";
        File.WriteAllText(path, jsonData);
    }

    private static void TryProceedToNextBatch()
    {
        if (!AreBatchRequirementsMet(CurrentBatch)) return;

        var nextIndex = _playerData.BatchIndex + 1;
        if (nextIndex < _chapterBatches.Count)
        {
            _playerData.BatchIndex = nextIndex;
        }
        else
        {
            Debug.Log("All batches completed. Proceed to next chapter if needed.");
            //_playerData.ChapterID++;
            //_playerData.BatchIndex = 0;
            //додати змінну яка буде визначати чи можна переходити до наступної глави
        }
    }

    private static bool AreBatchRequirementsMet(ChapterBatch batch)
    {
        return _playerData.DialogueIndex >= batch.requiredDialogueIndex &&
               _playerData.ProceededLawsCount >= batch.requiredProceededLaws;
    }

    public static bool IsPending<T>()
    {
        if (_activities.TryGetValue(typeof(T), out var activity))
        {
            if (activity is IGameActivity<T> a)
            {
                return a.IsPending(_playerData, CurrentBatch);
            }

            Debug.LogError($"Activity for type {typeof(T)} does not implement IGameActivity<{typeof(T).Name}>.");
            return false;
        }
        Debug.LogError($"No activity found for type {typeof(T)}");
        return false;
    }

    public static bool TryGetData<T>(out T data)
    {
        data = default;
        if (_activities.TryGetValue(typeof(T), out var activity))
        {
            if (activity is IGameActivity<T> a)
            {
                return a.TryGetData(_playerData, CurrentBatch, out data);
            }

            Debug.LogError($"Activity for type {typeof(T)} does not implement IGameActivity<{typeof(T).Name}>.");
            return false;
        }
        Debug.LogError($"No activity found for type {typeof(T)}");
        return false;
    }

    public static void ApplyResult<T>(T data, object context)
    {
        if (_activities.TryGetValue(typeof(T), out var activity))
        {
            if (activity is IResultableActivity<T> a)
            {
                a.ApplyResult(_playerData, data, context);
                return;
            }

            Debug.LogError($"Activity for type {typeof(T)} does not implement IResultableActivity<{typeof(T).Name}>.");
            return;
        }
        Debug.LogError($"No activity found for type {typeof(T)}");
    }

    public static int GetCharacteristicValue(Characteristic c) => _playerData.Characteristics[c];

    public static void SaveResponse(string responseId) => _playerData.PickedResponses.Add(responseId);

    public static bool IsResponsePicked(string responseId) => _playerData.PickedResponses.Contains(responseId);

    public static void FinishDialogue() => _playerData.DialogueIndex++;



    //Debug field to get dialogues count
    public static int DialoguesCount => _dialogues.Count;
    //Debug field to get current dialogue ID
    public static int DialogueID => _playerData.DialogueIndex;
    // Debug function to modify the dialogue ID
    public static void ModifyDialogueId(int value)
    {
        var newId = _playerData.DialogueIndex + value;
        if (newId < 0 || newId >= _dialogues.Count)
            return;

        _playerData.PickedResponses.Clear(); // Clear responses when modifying dialogue ID
        _playerData.DialogueIndex = newId;
    }

    //Debug function to clear the used law ids
    public static void ClearUsedLawIds()
    {
        _playerData.UsedLawIds.Clear();
        LoadChapterData();
    }

    //Debug function to reset player characteristics
    public static void ResetPlayerCharacteristics()
    {
        _playerData.Characteristics = new Dictionary<Characteristic, int>();
        foreach (Characteristic characteristic in Enum.GetValues(typeof(Characteristic)))
            _playerData.Characteristics[characteristic] = 20;
    }

    //Debug function to reset batch data
    public static void ResetBatchData()
    {
        _playerData.BatchIndex = 0;
        _playerData.ProceededLawsCount = 0;
    }
}
