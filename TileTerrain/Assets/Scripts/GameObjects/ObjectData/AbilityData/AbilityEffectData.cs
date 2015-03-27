using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public abstract class AbilityEffectData : ObjectData {

    [XmlArray("hitDamage"), XmlArrayItem("HitDamage")]
    public readonly HitDamage[] hitDamage;
    [XmlArray("hitBuffs"), XmlArrayItem("HitBuff")]
    public readonly HitBuff[] hitBuffs;

    public readonly int expSkill = 0;
    public readonly float expMultiplier = 1;

    public AbilityEffectData() { }
    public AbilityEffectData(AbilityEffectEdit edit) : base(edit)
    {
        hitDamage = new HitDamage[edit.hitDamages.Count];
        for (int i = 0; i < edit.hitDamages.Count; i++)
        {
            hitDamage[i] = new HitDamage(edit.hitDamages[i]);
        }
        hitBuffs = new HitBuff[edit.hitBuffs.Count];
        for(int i = 0; i < edit.hitBuffs.Count; i++)
        {
            hitBuffs[i] = new HitBuff(edit.hitBuffs[i]);
        }

        expSkill = (int)edit.expSkill;
        expMultiplier = edit.expMultiplier;
    }

    public abstract AbilityEffect getAbilityEffect(Unit unit, Vector3 targetPosition);
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

    public override string ToString()
    {
        string stats;
        stats = damageSelf ? "Damage yourself for " : "Damage target for ";
        stats += (percent * 100) + "% ";
        stats += yourStat ? "of your " : "of the targets ";
        stats += stat;
        return stats;
    }

}

public class HitBuff
{
    public readonly BuffType type;
    public readonly Stat stat;
    public readonly float amount;
    public readonly float duration;
    public readonly bool percent;

    public HitBuff() { }

    public HitBuff(HitBuffEdit edit)
    {
        this.type = edit.type;
        this.stat = edit.stat;
        this.amount = edit.amount;
        this.duration = edit.duration;
        this.percent = edit.percent;
    }
}
