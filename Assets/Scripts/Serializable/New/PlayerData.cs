using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class PlayerData
{
    public static string ActivePresident { get; set; }
    public static int ChapterId { get; set; }
    public int BatchId { get; set; }

    [JsonProperty] private Dictionary<ActivityType, int> _activityIDs;
    [JsonProperty] private Dictionary<Characteristic, int> _characteristics;
    [JsonProperty] private Dictionary<int, Dictionary<int, int>> _madeChoices;
    [JsonProperty] private Dictionary<int, Dictionary<int, int>> _madeDecisions;

    public PlayerData()
    {
        ChapterId = 0;
        BatchId = 0;

        _activityIDs = new Dictionary<ActivityType, int>();
        _characteristics = new Dictionary<Characteristic, int>();
        _madeChoices = new Dictionary<int, Dictionary<int, int>>();
        _madeDecisions = new Dictionary<int, Dictionary<int, int>>();
    }

    public int GetActivityID(ActivityType activity) => _activityIDs.TryGetValue(activity, out int id) ? id : -1;

    public void UpdateActivityID(ActivityType activity, int id) => _activityIDs[activity] = id;

    public void SaveChoice(int chapterId, int choiceId, int choiceValue)
    {
        if (!_madeChoices.ContainsKey(chapterId))
            _madeChoices[chapterId] = new Dictionary<int, int>();

        _madeChoices[chapterId][choiceId] = choiceValue;
    }

    public void SaveDecision(int chapterId, int decisionId, int decisionValue)
    {
        if (!_madeDecisions.ContainsKey(chapterId))
            _madeDecisions[chapterId] = new Dictionary<int, int>();

        _madeDecisions[chapterId][decisionId] = decisionValue;
    }

    public int GetCharacteristic(Characteristic characteristic) => _characteristics.TryGetValue(characteristic, out int value) ? value : 0;

    public void UpdateCharacteristics(Dictionary<Characteristic, int> characteristics)
    {
        foreach (var (characteristic, value) in characteristics)
        {
            if (!_characteristics.ContainsKey(characteristic))
                _characteristics[characteristic] = 0;

            if (characteristic == Characteristic.Budget)
                _characteristics[characteristic] += value;

            else
                _characteristics[characteristic] = Math.Clamp(_characteristics[characteristic] + value, 0, 100);
        }
    }
}