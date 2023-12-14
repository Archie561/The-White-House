using System;

[Serializable]
public class CutsceneFrame
{
    public string backgroundImage;
    public int frameDuration;
    public string animationType;

    public string[] characterNames;
    public int[] characterDurations;
    public string[] characterPositions;
    public string[] characterSubtitles;
}