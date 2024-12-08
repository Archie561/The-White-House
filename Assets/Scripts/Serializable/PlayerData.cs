using System;

[Serializable]
public class PlayerData
{
    public int chapterID;

    public int dialogueID;
    public int lawID;
    public int decisionID;

    public bool startingCutscenesShown;
    public bool gameOver;

    public MadeAction[] madeChoices;
    public MadeAction[] madeDecisions;

    public Characteristics characteristics;
}

[Serializable]
public class MadeAction
{
    public int[] value;
}
