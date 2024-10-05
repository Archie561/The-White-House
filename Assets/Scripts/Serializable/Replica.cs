using System;

[Serializable]
public class Replica
{
    public string imageName;
    public string characterName;
    public string replicaText;

    public int choiceID;
    public SubReplica[] subreplicasOption1;
    public SubReplica[] subreplicasOption2;
}
