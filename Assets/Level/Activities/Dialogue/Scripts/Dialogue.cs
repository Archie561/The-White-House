using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    public string requiredResponseId;

    public string backgoundMusic;
    public string backgroundImage;
    public List<DialogueLine> lines;
}
