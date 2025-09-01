using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Linq;

public static class GameDataManager
{
    public static string ActivePresident { get; private set; }

    private static PlayerData _playerData;
    private static List<ChapterBatch> _chapterBatches;

    private static DialogueActivity _dialogueActivity;
    private static LawActivity _lawActivity;

    private static ChapterBatch CurrentBatch => _chapterBatches[_playerData.BatchIndex];

    public static void LoadPlayerData(string activePresident)
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
            TradeObjects = Enum.GetValues(typeof(TradeObjectType))
                .Cast<TradeObjectType>()
                .ToDictionary(t => t, _ => new TradeObjectData(50, 100)),
            UsedLawIds = new HashSet<int>(),
            PickedResponses = new HashSet<string>(),
            ProceededLawsCount = 0,
            DialogueIndex = 0
        };
    }

    private static void LoadChapterData()
    {
        string gameLanguage = PlayerPrefs.GetString("gameLanguage", "en");

        _chapterBatches = LoadJsonResource<List<ChapterBatch>>($"Batches/{ActivePresident}/{_playerData.ChapterID}");

        var dialogues = LoadJsonResource<List<Dialogue>>($"Dialogues/{ActivePresident}/{_playerData.ChapterID}/{gameLanguage}");
        var allLaws = LoadJsonResource<List<Law>>($"Laws/{gameLanguage}");
        var availableLaws = allLaws.Where(law => !_playerData.UsedLawIds.Contains(law.lawID)).ToList();

        _dialogueActivity = new DialogueActivity(dialogues);
        _lawActivity = new LawActivity(availableLaws);


        //for debug purposes
        _dialoguesCount = dialogues.Count;
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

    /// <summary>
    /// Saves all the player data to a JSON file
    /// </summary>
    public static void Save()
    {
        string jsonData = JsonConvert.SerializeObject(_playerData, Formatting.Indented);
        string path = Application.persistentDataPath + $"/{ActivePresident}.json";
        File.WriteAllText(path, jsonData);
    }

    public static bool TryMoveToNextBatch()
    {
        if (_playerData.BatchIndex < _chapterBatches.Count - 1)
        {
            _playerData.BatchIndex++;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if the activity is currently pending
    /// </summary>
    /// <returns></returns>
    public static bool IsPending(ActivityType activityType)
    {
        return activityType switch
        {
            ActivityType.Dialogue => _dialogueActivity.IsPending(_playerData, CurrentBatch),
            ActivityType.Law => _lawActivity.IsPending(_playerData, CurrentBatch),
            _ => throw new Exception($"No data for activity {activityType}")
        };
    }

    /// <summary>
    /// returns the next dialogue if available
    /// </summary>
    public static bool TryGetNextDialogue(out Dialogue dialogue) =>
        _dialogueActivity.TryGetNextDialogue(_playerData, CurrentBatch, out dialogue);

    /// <summary>
    /// saves the player's response
    /// </summary>
    public static void SaveResponse(string result) =>
        _dialogueActivity.SaveResponse(_playerData, result);

    /// <summary>
    /// Returns the next law if available
    /// </summary>
    public static bool TryGetNextLaw(out Law law) =>
        _lawActivity.TryGetNextLaw(_playerData, CurrentBatch, out law);

    /// <summary>
    /// Changes the player's characteristics based on the law decision, saves the law as used, and increments the proceeded laws count.
    /// </summary>
    public static void SaveLawDecision(Law law, bool accepted) =>
        _lawActivity.SaveLawDecision(_playerData, law, accepted);

    public static int GetCharacteristicValue(Characteristic c) => _playerData.Characteristics[c];

    public static bool IsResponsePicked(string responseId) => _playerData.PickedResponses.Contains(responseId);

    public static void FinishDialogue() => _playerData.DialogueIndex++;

    public static TradeObjectData GetTradeObjectData(TradeObjectType t) => _playerData.TradeObjects[t];



    //for debug purposes
    private static int _dialoguesCount = 0;
    //Debug field to get current dialogue ID
    public static int DialogueID => _playerData.DialogueIndex;
    // Debug function to modify the dialogue ID
    public static void ModifyDialogueId(int value)
    {
        var newId = _playerData.DialogueIndex + value;
        if (newId < 0 || newId >= _dialoguesCount)
            return;

        _playerData.PickedResponses.Clear(); // Clear responses when modifying dialogue ID
        _playerData.DialogueIndex = newId;
    }

    public static void ResetPlayerData()
    {
        _playerData = GenerateNewPlayerData();
        LoadChapterData();
    }
}
