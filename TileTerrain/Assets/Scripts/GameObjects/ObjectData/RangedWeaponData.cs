using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using Edit;

public class RangedWeaponData : WeaponData {

	public readonly int damage;
	public readonly string attackAnim;
	public readonly string attackSound;

	public readonly string projectileModelName;
	public readonly string projectileName;

    public RangedWeaponData()
    { 
    }
    public RangedWeaponData(RangedWeaponEdit edit) : base(edit)
    {
        damage = edit.damage;
        if(!edit.attackAnim.Equals("")) attackAnim = edit.attackAnim;
        if(!edit.attackSound.Equals("")) attackSound = edit.attackSound;
        if(!edit.projectileModelName.Equals("")) projectileModelName = edit.projectileModelName;
        if(!edit.projectileName.Equals("")) projectileName = edit.projectileName;
    }

	public override int getDamage(int damageType)
	{
		return damage;
	}
	public override string getAttackAnim(int damageType)
	{
		return attackAnim;
	}
	public override string getAttackSound(int damageType)
	{
		return attackSound;
	}

	public override string getOffhandModelName ()
	{
		return "shortarrow";
	}

	public override bool isMelee ()
	{
		return false;
	}

    public override string getTooltipStatsString()
    {
        return "Damage: " + damage +
            "\nAttackspeed: " + attackSpeed +
            "\nProjectile: " + projectileName;
    }
}
