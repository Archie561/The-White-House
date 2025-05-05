using System;

[Serializable]
public class Choice
{
    public byte choiceID;

    public string option1;
    public string option2;

    public Characteristic characteristicsUpdateOption1;
    public Characteristic characteristicsUpdateOption2;
}
