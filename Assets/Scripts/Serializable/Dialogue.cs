using System;

[Serializable]
public class Dialogue
{
    public byte dialogueID;

    public sbyte lockedByDecision;
    public sbyte lockedByLaw;

    public Condition condition;

    public string backgroundImageName;
    public string backgroundMusic;

    public Replica[] replicas;
}
