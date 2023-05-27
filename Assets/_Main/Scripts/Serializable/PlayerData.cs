using System;

[Serializable]
public class PlayerData
{
    public byte chapterIndex;

    public byte dialogueIndex;
    public byte lawIndex;
    public byte decisionIndex;

    public int[] madeChoices;
    public int[] madeDecisions;
}
