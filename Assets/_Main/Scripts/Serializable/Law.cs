using System;

[Serializable]
public class Law
{
    public byte lawID;

    public sbyte lockedByDecision;
    public sbyte lockedByDialogue;

    public string lawName;
    public string lawText;
    public byte lawType;

    public Characteristics characteristicsUpdateWhenApplied;
    public Characteristics characteristicsUpdateWhenDeclined;
}
