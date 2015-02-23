using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
            attackSound = data.attackSound;
            hostile = data.hostile;
            lineofsight = data.lineofsight;

            safeDrops = "";
            foreach (string s in data.safeDrops)
            {
                safeDrops += s + "\n";
            }
            randomDrops = "";
            foreach (string s in data.randomDrops)
            {
                randomDrops += s + "\n";
            }

            minDrops = data.minDrops;
            maxDrops = data.maxDrops;
        }
        
        public AIUnitEdit(AIUnitEdit data) : base(data)
        {
            damage = data.damage;
            attackSpeed = data.attackSpeed;
            attackSound = data.attackSound;
            hostile = data.hostile;
            lineofsight = data.lineofsight;

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
        public string description;
        public ItemEdit()
        { 
        }
        public ItemEdit(ItemData data)
            : base(data)
        {
            description = data.description;
        }

        public ItemEdit(ItemEdit data)
            : base(data)
        {
            description = data.description;
        }
    }

    [Serializable]
    public class MaterialEdit : ItemEdit
    {
        public MaterialEdit()
        {
            name = "new material";
            gameName = "new material";
        }

        public MaterialEdit(MaterialData data)
            : base(data)
        {
        }

        public MaterialEdit(MaterialEdit data)
            : base(data)
        {
        }
    }

    [Serializable]
    public class ConsumableEdit : ItemEdit
    {
        public int hungerChange;

        public ConsumableEdit()
        {
            name = "new consumable";
            gameName = "new consumable";
        }

        public ConsumableEdit(ConsumableItemData data)
            : base(data)
        {
            hungerChange = data.hungerChange;
        }

        public ConsumableEdit(ConsumableEdit data)
            : base(data)
        {
            hungerChange = data.hungerChange;
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

    [Serializable]
    public class ResourceEdit : ObjectEdit
    {
        public int health;
        public DamageType.dtype damageType;

        public string safeDrops;
        public string randomDrops;
        public int minDrops = 0;
        public int maxDrops = 0;

        public int variances = 1;

        public string hitParticle;

        public bool blocksProjectile;

        public ResourceEdit(ResourceEdit data) : base(data)
        {
            health = data.health;
            damageType = data.damageType;

            safeDrops = data.safeDrops;
            
            randomDrops = data.randomDrops;
            
            minDrops = data.minDrops;
            maxDrops = data.maxDrops;

            variances = data.variances;
            hitParticle = data.hitParticle;
            blocksProjectile = data.blocksProjectile;
        }

        public ResourceEdit(ResourceData data) : base(data)
        {
            health = data.health;
            damageType = DamageType.intToDamageType(data.damageType);

            safeDrops = "";
            if (data.safeDrops != null)
            {
                foreach (string s in data.safeDrops)
                {
                    safeDrops += s + "\n";
                }
            }
            randomDrops = "";
            if (data.randomDrops != null)
            {
                foreach (string s in data.randomDrops)
                {
                    randomDrops += s + "\n";
                }
            }

            minDrops = data.minDrops;
            maxDrops = data.maxDrops;

            variances = data.variances;
            hitParticle = data.hitParticle;
            blocksProjectile = data.blocksProjectile;
        }

        public ResourceEdit()
        {
        }
    }

    [Serializable]
    public class RecipeEdit : ObjectEdit
    {
        public string product;
        public List<IngredientEdit> ingredients;
        public string description;

        public RecipeEdit()
        {
            name = "new recipe";
            gameName = "new recipe";

            ingredients = new List<IngredientEdit>();
        }

        public RecipeEdit(RecipeData data)
            : base(data)
        {
            product = data.product;
            ingredients = new List<IngredientEdit>();
            foreach(Ingredient i in data.ingredients)
            {
                ingredients.Add(new IngredientEdit(i));
            }

            description = data.description;
        }

        public RecipeEdit(RecipeEdit data)
            : base(data)
        {
            product = data.product;
            ingredients = new List<IngredientEdit>();
            foreach(IngredientEdit i in data.ingredients)
            {
                ingredients.Add(new IngredientEdit(i));
            }
            description = data.description;
        }

        public Ingredient[] getIngredients()
        {
            Ingredient[] result = new Ingredient[ingredients.Count];
            for (int i = 0; i < ingredients.Count; i++)
            {
                result[i] = new Ingredient(ingredients[i]);
            }

            return result;
        }
    }

    [Serializable]
    public class EquipmentRecipeEdit : RecipeEdit
    {
        public EquipmentRecipeEdit()
        { 
        }
        public EquipmentRecipeEdit(EquipmentRecipeData data)
            : base(data)
        {
        }

        public EquipmentRecipeEdit(EquipmentRecipeEdit data)
            : base(data)
        {
        }
    }

    [Serializable]
    public class MaterialRecipeEdit : RecipeEdit
    {
        public MaterialRecipeEdit()
        {
        }
        public MaterialRecipeEdit(MaterialRecipeData data)
            : base(data)
        {
        }

        public MaterialRecipeEdit(MaterialRecipeEdit data)
            : base(data)
        {
        }
    }

    [Serializable]
    public class ConsumableRecipeEdit : RecipeEdit
    {
        public ConsumableRecipeEdit()
        {
        }
        public ConsumableRecipeEdit(ConsumableRecipeData data)
            : base(data)
        {
        }

        public ConsumableRecipeEdit(ConsumableRecipeEdit data)
            : base(data)
        {
        }
    }

    [Serializable]
    public class IngredientEdit
    {
        public string name;
        public int amount;

        public IngredientEdit()
        { 
        }

        public IngredientEdit(Ingredient data)
        {
            this.name = data.name;
            this.amount = data.amount;
        }

        public IngredientEdit(IngredientEdit data)
        {
            this.name = data.name;
            this.amount = data.amount;
        }
    }


}