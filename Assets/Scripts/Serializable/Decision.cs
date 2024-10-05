using System;

[Serializable]
public class Decision
{
    public byte decisionID;

    public sbyte lockedByDialogue;
    public sbyte lockedByLaw;

    public Condition condition;

    public string imageName;
    public string characterName;
    public string text;

    public string[] options;

    public Characteristics[] characteristicUpdates;
}
