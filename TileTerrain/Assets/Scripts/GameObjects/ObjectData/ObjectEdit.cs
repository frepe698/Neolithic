using UnityEngine;
using System.Collections;
using System;

namespace Edit
{
    [Serializable]
    public abstract class ObjectEdit
    {
        public ObjectEdit(ObjectData data)
        {
            name = data.name;
            gameName = data.gameName;
            modelName = data.modelName;
        }

        public ObjectEdit(ObjectEdit data)
        {
            name = data.name;
            gameName = data.gameName;
            modelName = data.modelName;
        }

        public ObjectEdit()
        {
        }
        public string name;
        public string gameName;
        public string modelName;

        public virtual string windowTitle()
        {
            return gameName;
        }


        
    }
    #region Unit Edits

    [Serializable]
    public abstract class UnitEdit : ObjectEdit
    {
        public int health;
        public float lifegen;
        public float energy;
        public float energygen;
        public float movespeed;
        public float size;

        public UnitEdit(UnitData data) : base(data)
        {
            health = data.health;
            lifegen = data.lifegen;
            energy = data.energy;
            energygen = data.energygen;
            movespeed = data.movespeed;
            size = data.size;
        }

        public UnitEdit(UnitEdit data) : base(data)
        {
            health = data.health;
            lifegen = data.lifegen;
            energy = data.energy;
            energygen = data.energygen;
            movespeed = data.movespeed;
            size = data.size;
        }

        public UnitEdit()
        { 
        }
    }

    [Serializable]
    public class AIUnitEdit : UnitEdit
    {
        public int damage;
        public float attackSpeed = 1;
        public string attackSound;
        public bool hostile;
        public int lineofsight;

        public string safeDrops;

        public string randomDrops;

        public int minDrops;
        public int maxDrops;
        
        public AIUnitEdit(AIUnitData data) : base(data)
        {
            damage = data.damage;
            attackSpeed = data.attackSpeed;
            hostile = data.hostile;
            lineofsight = data.lineofsight;

            //data.safeDrops.CopyTo(safeDrops = new string[data.safeDrops.Length], 0);
            //data.randomDrops.CopyTo(randomDrops = new string[data.randomDrops.Length], 0);
            safeDrops = "";
            foreach (string s in data.safeDrops)
            {
                safeDrops += s + ",";
            }
            randomDrops = "";
            foreach (string s in data.randomDrops)
            {
                randomDrops += s + ",";
            }

            minDrops = data.minDrops;
            maxDrops = data.maxDrops;
        }
        
        public AIUnitEdit(AIUnitEdit data) : base(data)
        {
            damage = data.damage;
            attackSpeed = data.attackSpeed;
            hostile = data.hostile;
            lineofsight = data.lineofsight;

            //data.safeDrops.CopyTo(safeDrops = new string[data.safeDrops.Length], 0);
            //data.randomDrops.CopyTo(randomDrops = new string[data.randomDrops.Length], 0);

            safeDrops = data.safeDrops;
            randomDrops = data.randomDrops;

            minDrops = data.minDrops;
            maxDrops = data.maxDrops;
        }
        
        public AIUnitEdit()
        {
            name = "new ai unit";
            gameName = "new ai unit";
        }

        public override string windowTitle()
        {
 	         return "AI - " + gameName;
        }
    }

    [Serializable]
    public class HeroEdit : UnitEdit
    {
        public HeroEdit(HeroData data) : base(data)
        { 
        }

        public HeroEdit(HeroEdit data) : base(data)
        {
        }

        public HeroEdit()
        {
            name = "new hero";
            gameName = "new hero";
        }

        public override string windowTitle()
        {
 	         return "Hero - " + gameName;
        }

    }

    #endregion
    #region Item Edits
    [Serializable]
    public abstract class ItemEdit : ObjectEdit
    {
        public ItemEdit()
        { 
        }
        public ItemEdit(ItemData data)
            : base(data)
        { 
        }

        public ItemEdit(ItemEdit data)
            : base(data)
        {
        }
    }

    [Serializable]
    public abstract class CraftedEdit : ItemEdit
    {
        public CraftedEdit()
        { 
        }
        public CraftedEdit(CraftedItemData data) : base(data)
        {
            durability = data.durability;
        }
        public CraftedEdit(CraftedEdit data)
            : base(data)
        {
            durability = data.durability;
        }

        public int durability;
    }

    [Serializable]
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

        public WeaponEdit(WeaponEdit data)
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

    [Serializable]
    public class MeleeWeaponEdit : WeaponEdit
    {
        public MeleeWeaponEdit(MeleeWeaponData data)
            : base(data)
        {
            data.damage.CopyTo(damage = new int[3], 0);
            data.attackAnims.CopyTo(attackAnims = new string[3], 0);
            data.attackSounds.CopyTo(attackSounds = new string[3], 0);
        }
        public MeleeWeaponEdit(MeleeWeaponEdit data)
            : base(data)
        {
            data.damage.CopyTo(damage = new int[3], 0);
            data.attackAnims.CopyTo(attackAnims = new string[3], 0);
            data.attackSounds.CopyTo(attackSounds = new string[3], 0);
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
    
    [Serializable]
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

        public RangedWeaponEdit(RangedWeaponEdit data)
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
            //
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
#endregion
}