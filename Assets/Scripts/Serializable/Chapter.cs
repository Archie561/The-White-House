using System;

[Serializable]
public class Chapter
{
    public string title;
    public string theme;

    public Dialogue[] dialogues;
    public Choice[] choices;

    public Law[] laws;

    public Decision[] decisions;

    public News[] news;

    public Cutscene[] startingCutscenes;
    public Cutscene[] endingCutscenes;
}
