using System;

[Serializable]
public class PlayerData
{
    public byte chapterID;

    public byte dialogueID;
    public byte lawID;
    public byte decisionID;

    public MadeAction[] madeChoices;
    public MadeAction[] madeDecisions;

    public Characteristics characteristics;
}

[Serializable]
public class MadeAction
{
    public int[] value;
}
