using System;

[Serializable]
public enum ActivityType
{
    None,
    Dialogue,
    Law,
    Decision
}

[Serializable]
public enum Characteristic
{
    Budget,
    Navy,
    AirForces,
    Infantry,
    Machinery,
    EuropeanUnion,
    China,
    Africa,
    UnitedKingdom,
    CIS,
    OPEC,
    Science,
    Welfare,
    Education,
    Medicine,
    Ecology,
    Infrastructure
}

[Serializable]
public enum InternalCharacteristic
{
    Science,
    Defense,
    Education,
    Medicine,
    Freedom,
    Economy
}