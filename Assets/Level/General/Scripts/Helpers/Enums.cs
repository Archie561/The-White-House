using System;

[Serializable]
public enum ActivityType
{
    Dialogue,
    Law
}

[Serializable]
public enum Characteristic
{
    Ecology,
    Education,
    Medicine,
    Infrastructure,
    Science,
    Employment
}

public enum SFXType
{
    Accept,
    Click,
    Decline,
    Document,
    Door,
    Ring,
    Typing
}