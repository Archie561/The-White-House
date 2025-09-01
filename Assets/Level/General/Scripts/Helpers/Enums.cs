using System;

[Serializable]
public enum ActivityType
{
    Dialogue,
    Law,
    Trade
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

[Serializable]
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

[Serializable]
public enum TradeObjectType
{
    Oil,
    Coal,
    Gas,
    Wood
}