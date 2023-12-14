using System;

[Serializable]
public class Dialogue
{
    public byte dialogueID;

    public sbyte lockedByDecision;
    public sbyte lockedByLaw;

    public Condition condition;

    public string backgroundImageName;

    public string[] imageName;
    public string[] characterName;
    public string[] replicas;
    public sbyte[] choiceID;

    public SubDialogue[] subDialogueOption1;
    public SubDialogue[] subDialogueOption2;
}
