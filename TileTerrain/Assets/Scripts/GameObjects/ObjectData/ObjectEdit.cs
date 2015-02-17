using UnityEngine;
using System.Collections;

namespace Edit
{
    public abstract class ObjectEdit
    {
        public ObjectEdit(ObjectData data)
        {
            name = data.name;
            gameName = data.gameName;
        }

        public ObjectEdit()
        {
        }

        public string name;
        public string gameName;

        public virtual string windowTitle()
        {
            return gameName;
        }

        
    }

    public abstract class ItemEdit : ObjectEdit
    {
        public ItemEdit()
        { 
        }
        public ItemEdit(ItemData data)
            : base(data)
        { 
        }
    }

    public abstract class CraftedEdit : ItemEdit
    {
        public CraftedEdit()
        { 
        }
        public CraftedEdit(CraftedItemData data) : base(data)
        {
            durability = data.durability;
        }

        public int durability;
    }

    public abstract class WeaponEdit : CraftedEdit
    {
        public WeaponEdit(WeaponData data)
            : base(data)
        {
            rightHand = data.rightHand;
            idleAnim = data.idleAnim;
            runAnim = data.runAnim;
            lootAnim = data.lootAnim;
            attackSpeed = data.attackSpeed;
            weaponAttackAnim = data.weaponAttackAnim;
        }

        public WeaponEdit()
        {
        }

        public bool rightHand = true;

        public string idleAnim;
        public string runAnim;
        public string lootAnim;

        public float attackSpeed;

        public string weaponAttackAnim;
    }
    public class MeleeWeaponEdit : WeaponEdit
    {
        public MeleeWeaponEdit(MeleeWeaponData data)
            : base(data)
        {
            damage = data.damage;
            attackAnims = data.attackAnims;
            attackSounds = data.attackSounds;
        }
        public MeleeWeaponEdit()
        {
            name = "new melee weapon";
            gameName = "new melee weapon";
            damage = new int[3];
            attackAnims = new string[3];
            attackSounds = new string[3];
        }
        public int[] damage;

        public string[] attackAnims;

        public string[] attackSounds;

        public override string windowTitle()
        {
            return "Melee - " + gameName;
        }
    }

    public class RangedWeaponEdit : WeaponEdit
    {
        public RangedWeaponEdit(RangedWeaponData data)
            : base(data)
        {
            damage = data.damage;
            attackAnim = data.attackAnim;
            attackSound = data.attackSound;
            projectileModelName = data.projectileModelName;
            projectileName = data.projectileName;
        }

        public RangedWeaponEdit()
        {
            name = "new ranged weapon";
            gameName = "new ranged weapon";

        }

        public int damage;
        public string attackAnim;
        public string attackSound;

        public string projectileModelName;
        public string projectileName;

        public override string windowTitle()
        {
            return "Ranged - " + gameName;
        }
    }
}