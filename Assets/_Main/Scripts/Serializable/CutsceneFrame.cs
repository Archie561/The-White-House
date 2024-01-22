using System;

[Serializable]
public class CutsceneFrame
{
    public string backgroundImage;
    public float frameDuration;
    public string animationType;
    public string particlesType;

    public string[] characterNames;
    public float[] characterDurations;
    public string[] characterPositions;
    public string[] characterSubtitles;
}