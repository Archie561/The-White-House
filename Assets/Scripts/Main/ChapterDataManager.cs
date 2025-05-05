using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChapterDataManager
{
    private Chapter _chapter;
    private Dictionary<ActivityType, int>[] _batches;

    public ChapterDataManager(string presidentName, int chapterID)
    {
        TextAsset jsonData = Resources.Load<TextAsset>($"Story/{presidentName}/{chapterID}/{PlayerPrefs.GetString("gameLanguage", "en")}/{chapterID}");
        TextAsset batchesJsonData = Resources.Load<TextAsset>($"Story/{presidentName}/{chapterID}/batches");

        _chapter = JsonUtility.FromJson<Chapter>(jsonData.text);
        _batches = JsonConvert.DeserializeObject<Dictionary<ActivityType, int>[]>(batchesJsonData.text);
    }

    public Dictionary<ActivityType, int> GetBatch(int ID) => _batches[ID];

    public int GetBatchesLength() => _batches.Length;

    public Dialogue GetDialogue(int ID) => _chapter.dialogues[ID];

    public Choice GetChoice(int ID) => _chapter.choices[ID];

    public Law GetLaw(int ID) => _chapter.laws[ID];

    public Decision GetDecision(int ID) => _chapter.decisions[ID];

    public int GetDialoguesLength() => _chapter.dialogues.Length;

    public int GetLawsLength() => _chapter.laws.Length;

    public int GetDecisionsLength() => _chapter.decisions.Length;

    public News[] GetNews() => _chapter.news;

    public Cutscene[] GetStartingCutscenes() => _chapter.startingCutscenes;

    public Cutscene[] GetEndingCutscenes() => _chapter.endingCutscenes;
}