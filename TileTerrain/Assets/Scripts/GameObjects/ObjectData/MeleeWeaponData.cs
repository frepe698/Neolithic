using UnityEngine;

using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using Edit;

public class MeleeWeaponData : WeaponData {

	[XmlArray("damage"), XmlArrayItem("int")]
	public readonly int[] damage;

	[XmlArray("attackAnims"), XmlArrayItem("string")]
	public readonly string[] attackAnims;
	
	public readonly string[] attackSounds;

    public MeleeWeaponData()
    {
    }
    public MeleeWeaponData(MeleeWeaponEdit edit) : base(edit)
    {
        damage = edit.damage;
        attackAnims = edit.attackAnims;
        attackSounds = edit.attackSounds;
    }

	public override int getDamage(int damageType)
	{
		return damage[damageType];
	}
	public override string getAttackAnim(int damageType)
	{
		return attackAnims[damageType];
	}
	public override string getAttackSound(int damageType)
	{
		return attackSounds[damageType];
	}

	public override bool isMelee ()
	{
		return true;
	}
}
