using System;

[Serializable]
public class AWSceneryAbout : AWThingAbout
{
    public new AWSceneryParts parts;
}

[Serializable]
public class AWSceneryParts : AWParts
{
    public new string body;
}
