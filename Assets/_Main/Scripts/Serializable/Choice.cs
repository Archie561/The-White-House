using System;

[Serializable]
public class Choice
{
    public byte choiceID;

    public string option1;
    public string option2;

    public Characteristics characteristicsUpdateOption1;
    public Characteristics characteristicsUpdateOption2;

    public bool mustBeSaved;
}
