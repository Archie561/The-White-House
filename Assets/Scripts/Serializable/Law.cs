using System;

[Serializable]
public class Law
{
    public byte lawID;

    public sbyte lockedByDecision;
    public sbyte lockedByDialogue;

    public byte lawType;

    public string header;
    public string mainText;
    public string detailedText;
    public string preparedBy;

    public Characteristics characteristicsUpdateWhenApplied;
    public Characteristics characteristicsUpdateWhenDeclined;
}
