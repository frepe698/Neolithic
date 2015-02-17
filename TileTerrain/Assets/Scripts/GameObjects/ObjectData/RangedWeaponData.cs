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
        attackAnim = edit.attackAnim;
        attackSound = edit.attackSound;
        projectileModelName = edit.projectileModelName;
        projectileName = edit.projectileName;
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
}
