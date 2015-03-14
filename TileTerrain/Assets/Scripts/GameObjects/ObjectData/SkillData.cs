using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class SkillData : ObjectData{

    [XmlArray("requiredExp")]
    public readonly int[] requiredExp;

    [XmlArray("statsPerLevel"), XmlArrayItem("StatChange")]
    public readonly StatChange[] statsPerLevel;

    [XmlArray("learnableAbilities"), XmlArrayItem("LearnableAbility")]
    public readonly LearnableAbility[] abilities;

    public SkillData()
    {

    }

    public SkillData(SkillEdit data)
        :  base(data)
    {
        requiredExp = data.requiredExp;
        statsPerLevel = new StatChange[data.statsPerLevel.Count];
        for(int i = 0; i< statsPerLevel.Length; i++)
        {
            statsPerLevel[i] = new StatChange(data.statsPerLevel[i]);
        }
        abilities = data.abilities.ToArray();
    }
}
