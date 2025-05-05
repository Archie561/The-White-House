using System;
using System.Collections.Generic;

[Serializable]
public class Law
{
    public byte lawID;

    public string header;
    public string mainText;
    public string detailedText;
    public string preparedBy;

    public Dictionary<InternalCharacteristic, int> affectedCharacteristics;
}