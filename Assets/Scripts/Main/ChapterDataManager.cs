using UnityEngine;

public class ChapterDataManager
{
    private Chapter _chapter;

    public ChapterDataManager(string presidentName, int chapterID)
    {
        TextAsset jsonData = Resources.Load<TextAsset>("Story/" + presidentName + "/" + PlayerPrefs.GetString("gameLanguage", "en") + "/" + chapterID);
        _chapter = JsonUtility.FromJson<Chapter>(jsonData.text);
    }

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