using System;

[Serializable]
public class Condition
{
    public bool isUsed;

    public string characteristicName;
    public sbyte characteristicValue;

    public sbyte choiceChapterID;
    public byte choiceID;
    public byte choiceValue;

    public sbyte decisionChapterID;
    public byte decisionID;
    public byte decisionValue;
}
