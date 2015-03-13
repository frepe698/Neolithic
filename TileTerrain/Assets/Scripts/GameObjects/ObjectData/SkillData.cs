using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class SkillData : ObjectData{

    [XmlArray("requiredExp")]
    public readonly int[] requiredExp;

    public SkillData()
    {

    }

    public SkillData(SkillEdit data)
        :  base(data)
    {
        requiredExp = data.requiredExp;
        
    }
}
