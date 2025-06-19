using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int ChapterID;
    public int BatchIndex;

    public Dictionary<Characteristic, int> Characteristics;
    public HashSet<int> UsedLawIds;
    public HashSet<string> PickedResponses;
    public int ProceededLawsCount;
    public int DialogueIndex;
}
