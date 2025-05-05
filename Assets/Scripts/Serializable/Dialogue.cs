using System;

[Serializable]
public class Dialogue
{
    public ActivityType Type => ActivityType.Dialogue;
    public int ID => 0;

    public byte dialogueID;

    public sbyte lockedByDecision;
    public sbyte lockedByLaw;

    public Condition condition;

    public string backgroundImageName;
    public string backgroundMusic;

    public Replica[] replicas;
}
