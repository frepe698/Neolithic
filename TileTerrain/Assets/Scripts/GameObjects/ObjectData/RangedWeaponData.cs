using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;

public class RangedWeaponData : WeaponData {

	public readonly int damage;
	public readonly string attackAnim;
	public readonly string attackSound;

	public readonly string projectileModelName;
	public readonly string projectileName;

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
