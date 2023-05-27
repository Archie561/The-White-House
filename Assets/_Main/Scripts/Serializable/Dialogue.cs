using System;

[Serializable]
public class Dialogue
{
    public byte dialogueID;

    public Condition condition;

    public sbyte lockedByDecision;
    public sbyte lockedByLaw;

    public string[] imageName;
    public string[] characterName;
    public string[] replicas;
    public sbyte[] choiceID;

    public SubDialogue[] subDialogueOption1;
    public SubDialogue[] subDialogueOption2;
}
