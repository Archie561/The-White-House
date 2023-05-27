using System;

[Serializable]
public class Condition
{
    public bool isUsed;

    public string characteristicName;
    public sbyte characteristicValue;

    public sbyte choiceIndex;
    public byte choiceValue;

    public sbyte decisionIndex;
    public byte decisionValue;
}
