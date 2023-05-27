using System;

[Serializable]
public class Decision
{
    public byte decisionID;

    public Condition condition;

    public sbyte lockedByDialogue;
    public sbyte lockedByLaw;

    public string characterName;
    public string imageName;
    public string text;

    public string[] options;

    public Characteristics[] characteristicUpdates;
}
