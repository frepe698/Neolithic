using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Edit;

public class AbilityData : ObjectData {

    [XmlArray("effects"), XmlArrayItem("AbilityEffectAndTime")]
    public readonly AbilityEffectAndTime[] effects;

    [XmlArray("animations"), XmlArrayItem("AbilityAnimation")]
    public readonly AbilityAnimation[] animations;

    [XmlArray("speedIncreases"), XmlArrayItem("SpeedIncrease")]
    public readonly SpeedIncrease[] speedIncreases;

    public readonly float totalTime;
    public readonly bool canAlwaysStart;
    public readonly bool canRetarget = true;

    public readonly int energyCost;
    public readonly int healthCost;

    public readonly float cooldown;
    public readonly float range;

    public readonly int tags;

    //Not serialized
    private Sprite icon;

    public AbilityData() { }

    public AbilityData(AbilityEdit edit)
        : base(edit)
    {

        effects = new AbilityEffectAndTime[edit.effects.Count];
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i] = new AbilityEffectAndTime(edit.effects[i]);
        }
        animations = new AbilityAnimation[edit.animations.Count];
        for (int i = 0; i < animations.Length; i++)
        {
            animations[i] = new AbilityAnimation(edit.animations[i]);
        }
        speedIncreases = new SpeedIncrease[edit.speedIncreases.Count];
        for (int i = 0; i < speedIncreases.Length; i++)
        {
            speedIncreases[i] = new SpeedIncrease(edit.speedIncreases[i]);
        }
        tags = edit.tags;
        energyCost = edit.energyCost;
        healthCost = edit.healthCost;
        cooldown = edit.cooldown;
        totalTime = edit.totalTime;
        canAlwaysStart = edit.canAlwaysStart;
        canRetarget = edit.canRetarget;
        range = edit.range;
    }

    public void setIcon(Sprite sprite)
    {
        this.icon = sprite;
    }

    public Sprite getIcon()
    {
        return icon;
    }
}

public class AbilityAnimation
{
    public readonly bool weaponAttackAnimation;

    [XmlElement(IsNullable = false)]
    public readonly string name;

    public readonly float time;
    public readonly float speed;

    public AbilityAnimation() { }
    public AbilityAnimation(AbilityAnimationEdit edit)
    {
        weaponAttackAnimation = edit.weaponAttackAnimation;
        if (!weaponAttackAnimation) name = edit.name;
        time = edit.time;
        speed = edit.speed;
    }
}

public class AbilityEffectAndTime
{
    public readonly string name;
    public readonly float time;

    public AbilityEffectAndTime() { }
    public AbilityEffectAndTime(AbilityEffectAndTimeEdit edit)
    {
        this.name = edit.name;
        this.time = edit.time;
    }
}

public class SpeedIncrease
{
    public readonly Stat stat;
    public readonly float percent;

    public SpeedIncrease() { }
    public SpeedIncrease(SpeedIncreaseEdit edit)
    {
        this.stat = edit.stat;
        this.percent = edit.percent;
    }
}
