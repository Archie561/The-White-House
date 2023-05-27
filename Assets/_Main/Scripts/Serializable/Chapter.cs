using System;

[Serializable]
public class Chapter
{
    public Dialogue[] dialogues;
    public Choice[] choices;

    public Law[] laws;

    public Decision[] decisions;
}
