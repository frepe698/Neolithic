using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class AbilityData : ObjectData {

    [XmlArray("effects"), XmlArrayItem("AbilityEffectAndTime")]
    public readonly AbilityEffectAndTime[] effects;

    public readonly float totalTime;

    public readonly int energyCost;
    public readonly int healthCost;

    public readonly float cooldown;

    public AbilityData() { }

    public AbilityData(AbilityEdit edit)
        : base(edit)
    {
        effects = edit.effects.ToArray();
        energyCost = edit.energyCost;
        healthCost = edit.healthCost;
        cooldown = edit.cooldown;
    }


}
