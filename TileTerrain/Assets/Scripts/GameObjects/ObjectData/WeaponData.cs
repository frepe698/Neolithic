using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using Edit;

public abstract class WeaponData : CraftedItemData {

	public readonly bool rightHand = true;

    [XmlElement(IsNullable = false)]
	public readonly string idleAnim;
    [XmlElement(IsNullable = false)]
	public readonly string runAnim;
    [XmlElement(IsNullable = false)]
	public readonly string lootAnim;
	
	public readonly float attackSpeed;

    [XmlElement(IsNullable = false)]
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
        if (!edit.idleAnim.Equals("")) idleAnim = edit.idleAnim;
        if (!edit.runAnim.Equals("")) runAnim = edit.runAnim;
        if (!edit.lootAnim.Equals("")) lootAnim = edit.lootAnim;
        attackSpeed = edit.attackSpeed;
        if(edit.weaponAttackAnim != null && !edit.weaponAttackAnim.Trim().Equals("")) weaponAttackAnim = edit.weaponAttackAnim;
    }

	public virtual string getOffhandModelName()
	{
		return null;
	}

	public abstract bool isMelee();

}
