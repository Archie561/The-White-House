using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public string requiredResponseId;

    public string characterName;
    public string characterImage;
    public string text;

    public List<Response> responses;
}
