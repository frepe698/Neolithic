using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using Edit;

public abstract class WeaponData : CraftedItemData {

	public readonly bool rightHand = true;

	public readonly string idleAnim;
	public readonly string runAnim;
	public readonly string lootAnim;
	
	public readonly float attackSpeed;

	public readonly string weaponAttackAnim;

	public abstract int getDamage(int damageType);
	public abstract string getAttackAnim(int damageType);
	public abstract string getAttackSound(int damageType);

    public WeaponData()
    { 
    }

    public WeaponData(WeaponEdit edit) : base(edit)
    {
        rightHand = edit.rightHand;
        idleAnim = edit.idleAnim;
        runAnim = edit.runAnim;
        lootAnim = edit.lootAnim;
        attackSpeed = edit.attackSpeed;
        weaponAttackAnim = edit.weaponAttackAnim;
    }

	public virtual string getOffhandModelName()
	{
		return null;
	}

	public abstract bool isMelee();

}
