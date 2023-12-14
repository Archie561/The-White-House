using System;

[Serializable]
public class Cutscene
{
    public byte sceneID;
    public Condition condition;
    public string voiceFileName;

    public CutsceneFrame[] frames;
}