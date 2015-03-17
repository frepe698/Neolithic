using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public abstract class AbilityEffectData : ObjectData {

    [XmlArray("hitDamage"), XmlArrayItem("HitDamage")]
    public readonly HitDamage[] hitDamage;

    public AbilityEffectData() { }
    public AbilityEffectData(AbilityEffectEdit edit) : base(edit)
    {
        hitDamage = new HitDamage[edit.hitDamages.Count];
        for (int i = 0; i < edit.hitDamages.Count; i++)
        {
            hitDamage[i] = new HitDamage(edit.hitDamages[i]);
        }
    }

    public abstract AbilityEffect getAbilityEffect(Unit unit, Vector2 targetPosition);
}

public class HitDamage
{
    public readonly Stat stat;
    public readonly float percent;
    public readonly bool yourStat;
    public readonly bool damageSelf;

    public HitDamage(){}

    public HitDamage(HitDamageEdit edit)
    {
        this.stat = edit.stat;
        this.percent = edit.percent;
        this.yourStat = edit.yourStat;
        this.damageSelf = edit.damageSelf;
    }

}
