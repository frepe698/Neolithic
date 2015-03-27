using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Edit
{
    [Serializable]
    public abstract class ObjectEdit
    {
        public string name;
        public string gameName;
        public string modelName;

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
        
        public virtual string windowTitle()
        {
            return gameName;
        }
    }

    [Serializable]
    public class AbilityEffectEdit : ObjectEdit
    {
        public List<HitDamageEdit> hitDamages;
        public List<HitBuffEdit> hitBuffs;

        public Skills expSkill;
        public float expMultiplier = 1;

        //On hit effects go here too
        public AbilityEffectEdit() 
        {
            hitDamages = new List<HitDamageEdit>();
            hitBuffs = new List<HitBuffEdit>();
        }
        public AbilityEffectEdit(AbilityEffectData data) : base(data)
        {
            hitDamages = new List<HitDamageEdit>();
            foreach(HitDamage hitDamage in data.hitDamage)
            {
                hitDamages.Add(new HitDamageEdit(hitDamage));
            }
            hitBuffs = new List<HitBuffEdit>();
            if(data.hitBuffs != null)
            {
                foreach (HitBuff buff in data.hitBuffs)
                {
                    hitBuffs.Add(new HitBuffEdit(buff));
                }
            }

            expSkill = (Skills)data.expSkill;
            expMultiplier = data.expMultiplier;
            
        }
        public AbilityEffectEdit(AbilityEffectEdit edit) : base(edit)
        {
            hitDamages = new List<HitDamageEdit>();
            foreach (HitDamageEdit hde in edit.hitDamages)
                hitDamages.Add(new HitDamageEdit(hde));

            hitBuffs = new List<HitBuffEdit>();
            foreach (HitBuffEdit hbe in edit.hitBuffs)
                hitBuffs.Add(new HitBuffEdit(hbe));


            expSkill = edit.expSkill;
            expMultiplier = edit.expMultiplier;
        }
    }

    [Serializable]
    public class SingleTargetEffectEdit : AbilityEffectEdit
    {
        //On hit effects go here too
        public SingleTargetEffectEdit() : base()
        {
            
        }
        public SingleTargetEffectEdit(SingleTargetEffectData data)
            : base(data)
        {
            
        }
        public SingleTargetEffectEdit(AbilityEffectEdit edit)
            : base(edit)
        {
        }
    }

    [Serializable]
    public class AreaOfEffectEdit : AbilityEffectEdit
    {
        public float radius;
        //On hit effects go here too
        public AreaOfEffectEdit()
            : base()
        {

        }
        public AreaOfEffectEdit(AreaOfEffectData data)
            : base(data)
        {
            radius = data.radius;
        }
        public AreaOfEffectEdit(AreaOfEffectEdit edit)
            : base(edit)
        {
            radius = edit.radius;
        }
    }

    [Serializable]
    public class SelfBuffEffectEdit : AbilityEffectEdit
    {
        public SelfBuffEffectEdit() : base()
        { }
        public SelfBuffEffectEdit(SelfBuffEffectData data) : base(data)
        { }
        public SelfBuffEffectEdit(SelfBuffEffectEdit edit) : base(edit)
        { }
    }
    [Serializable]
    public class ProjectileEffectEdit : AbilityEffectEdit
    {
        public string projectileName;
        public bool weaponProjectile;
        public float angle;

        public ProjectileEffectEdit() : base()
        {

        }
        public ProjectileEffectEdit(ProjectileEffectData data) : base(data)
        {
            projectileName = data.projectileName;
            weaponProjectile = data.weaponProjectile;
            angle = data.angle;
        }

        public ProjectileEffectEdit(ProjectileEffectEdit edit) : base(edit)
        {
            projectileName = edit.projectileName;
            weaponProjectile = edit.weaponProjectile;
            angle = edit.angle;
        }
    }

    [Serializable]
    public class MovementEffectEdit : AbilityEffectEdit
    {
        public float range;
        public float travelTime;

        public MovementEffectEdit() : base() { }

        public MovementEffectEdit(MovementEffectData data)
            : base(data)
        {
            range = data.range;
            travelTime = data.travelTime;
        }

        public MovementEffectEdit(MovementEffectEdit data)
            : base(data)
        {
            range = data.range;
            travelTime = data.travelTime;
        }

    }
    

    [Serializable]
    public class HitDamageEdit
    {
        public Stat stat;
        public float percent;
        public bool yourStat;
        public bool damageSelf;

        public HitDamageEdit() 
        {
            yourStat = true;
            damageSelf = false;
        }

        public HitDamageEdit(HitDamage hitDamage)
        {
            stat = hitDamage.stat;
            percent = hitDamage.percent;
            yourStat = hitDamage.yourStat;
            damageSelf = hitDamage.damageSelf;
        }

        public HitDamageEdit(HitDamageEdit hitDamage)
        {
            stat = hitDamage.stat;
            percent = hitDamage.percent;
            yourStat = hitDamage.yourStat;
            damageSelf = hitDamage.damageSelf;
        }
    }

    [Serializable]
    public class HitBuffEdit
    {
        public BuffType type;
        public Stat stat;
        public bool percent;
        public float amount;
        public float duration;
        
        public HitBuffEdit()
        {
            percent = false;
        }

        public HitBuffEdit(HitBuff hitBuff)
        {
            type = hitBuff.type;
            stat = hitBuff.stat;
            percent = hitBuff.percent;
            amount = hitBuff.amount;
            duration = hitBuff.duration;
        }

        public HitBuffEdit(HitBuffEdit edit)
        {
            type = edit.type;
            stat = edit.stat;
            percent = edit.percent;
            amount = edit.amount;
            duration = edit.duration;
        }
    }

    [Serializable]
    public class AbilityEdit : ObjectEdit
    {
        public List<AbilityEffectAndTimeEdit> effects;
        public List<AbilityAnimationEdit> animations;
        public List<SpeedIncreaseEdit> speedIncreases;

        public int tags;
        public static readonly string[] tagNames = new string[]
        {
                "Melee",
                "Ranged",
                "Bow",
                "Slinger",
                "Unarmed"
        };
        public float totalTime;

        public int energyCost;
        public int healthCost;

        public float cooldown;
        public float range;

        public AbilityEdit()
        {
            name = "new ability";
            gameName = "New Ability";
            effects = new List<AbilityEffectAndTimeEdit>();
            animations = new List<AbilityAnimationEdit>();
            speedIncreases = new List<SpeedIncreaseEdit>();
            range = 2;
        }

        public AbilityEdit(AbilityData data)
            : base(data)
        {
            effects = new List<AbilityEffectAndTimeEdit>();
            foreach (AbilityEffectAndTime a in data.effects)
            {
                effects.Add(new AbilityEffectAndTimeEdit(a));
            }
            animations = new List<AbilityAnimationEdit>();
            if (data.animations != null)
            {
                foreach (AbilityAnimation a in data.animations)
                {
                    animations.Add(new AbilityAnimationEdit(a));
                }
            }
            speedIncreases = new List<SpeedIncreaseEdit>();
            if (data.speedIncreases != null)
            {
                foreach (SpeedIncrease s in data.speedIncreases)
                {
                    speedIncreases.Add(new SpeedIncreaseEdit(s));
                }
            }
            tags = data.tags;
         
            energyCost = data.energyCost;
            healthCost = data.healthCost;
            cooldown = data.cooldown;
            totalTime = data.totalTime;
            range = data.range;
        }

        public AbilityEdit(AbilityEdit data)
            : base(data)
        {
            effects = new List<AbilityEffectAndTimeEdit>();
            foreach (AbilityEffectAndTimeEdit a in data.effects)
            {
                effects.Add(new AbilityEffectAndTimeEdit(a));
            }
            animations = new List<AbilityAnimationEdit>();
            if (data.animations != null)
            {
                foreach (AbilityAnimationEdit a in data.animations)
                {
                    animations.Add(new AbilityAnimationEdit(a));
                }
            }
            speedIncreases = new List<SpeedIncreaseEdit>();
            if (data.speedIncreases != null)
            {
                foreach (SpeedIncreaseEdit s in data.speedIncreases)
                {
                    speedIncreases.Add(new SpeedIncreaseEdit(s));
                }
            }
            tags = data.tags;
            energyCost = data.energyCost;
            healthCost = data.healthCost;
            cooldown = data.cooldown;
            totalTime = data.totalTime;
            range = data.range;
            tags = data.tags;
        }
    }

    [Serializable]
    public class SpeedIncreaseEdit
    {
        public Stat stat;
        public float percent;

        public SpeedIncreaseEdit()
        {
            stat = Stat.Attackspeed;
            percent = 1;
        }
        public SpeedIncreaseEdit(SpeedIncrease edit)
        {
            this.stat = edit.stat;
            this.percent = edit.percent;
        }
        public SpeedIncreaseEdit(SpeedIncreaseEdit edit)
        {
            this.stat = edit.stat;
            this.percent = edit.percent;
        }
    }

    [Serializable]
    public class AbilityEffectAndTimeEdit
    {
        public string name;
        public float time;

        public AbilityEffectAndTimeEdit() { }
        public AbilityEffectAndTimeEdit(AbilityEffectAndTime edit)
        {
            this.name = edit.name;
            this.time = edit.time;
        }

        public AbilityEffectAndTimeEdit(AbilityEffectAndTimeEdit edit)
        {
            this.name = edit.name;
            this.time = edit.time;
        }
    }

    [Serializable]
    public class AbilityAnimationEdit
    {
        public bool weaponAttackAnimation;
        public string name;
        public float time;
        public float speed = 1;

        public AbilityAnimationEdit()
        { 
        }

        public AbilityAnimationEdit(AbilityAnimation anim)
        {
            weaponAttackAnimation = anim.weaponAttackAnimation;
            if(!weaponAttackAnimation) name = anim.name;
            time = anim.time;
            speed = anim.speed;
        }

        public AbilityAnimationEdit(AbilityAnimationEdit anim)
        {
            weaponAttackAnimation = anim.weaponAttackAnimation;
            if (!weaponAttackAnimation) name = anim.name;
            time = anim.time;
            speed = anim.speed;
        }
    }

    #region Skills and Abilities Edits

    [Serializable]
    public class SkillEdit : ObjectEdit
    {
        public List<PassiveStat> statsPerLevel;
        public bool reqExpFolded = true;
        public int[] requiredExp;

        public bool passiveStatsFolded = true;
        public List<LearnableAbility> abilities;

        public SkillEdit()
        {
            name = "new skill";
            gameName = "New Skill";
            requiredExp = new int[Skill.MAXLEVEL];
            resetRequiredExp();
            statsPerLevel = new List<PassiveStat>();
            abilities = new List<LearnableAbility>();
        }

        public SkillEdit(SkillData data)
            : base(data)
        {
            requiredExp = data.requiredExp;
            statsPerLevel = new List<PassiveStat>();
            foreach (StatChange stat in data.statsPerLevel)
            {
                statsPerLevel.Add(new PassiveStat(stat));
            }
            abilities = new List<LearnableAbility>();
            foreach (LearnableAbility ab in data.abilities)
            {
                abilities.Add(ab);
            }
        }

        public SkillEdit(SkillEdit data)
            : base(data)
        {
            requiredExp = data.requiredExp;
            statsPerLevel = data.statsPerLevel;
            if (statsPerLevel == null) statsPerLevel = new List<PassiveStat>();
            abilities = data.abilities;
            if (abilities == null) abilities = new List<LearnableAbility>();
        }

        public void resetRequiredExp()
        {
            for (int i = 0; i < Skill.MAXLEVEL; i++)
            {
                int level = i + 1;
                requiredExp[i] = (int)((level + level * (level * 0.353f)) * 60);
            }
        }
    }

    [Serializable]
    public class PassiveStat
    {
        public Stat stat;
        public float amount;
        public bool multiplier;

        public PassiveStat() { }

        public PassiveStat(StatChange data)
        {
            this.stat = data.stat;
            this.amount = data.value;
            this.multiplier = data.isMultiplier;
        }
    }

    [Serializable]
    public class LearnableAbility
    {
        public string name;
        public int reqLevel;
    }
    

    #endregion

    #region Unit Edits

    [Serializable]
    public abstract class UnitEdit : ObjectEdit
    {
        public int health;
        public int healthperlevel;
        public float lifegen;
        public float energy;
        public float energygen;
        public float movespeed;
        public float size;

        public int favouronkill;

        public UnitEdit(UnitData data) : base(data)
        {
            health = data.health;
            healthperlevel = data.healthperlevel;
            lifegen = data.lifegen;
            energy = data.energy;
            energygen = data.energygen;
            movespeed = data.movespeed;
            size = data.size;
            favouronkill = data.favouronkill;
        }

        public UnitEdit(UnitEdit data) : base(data)
        {
            health = data.health;
            healthperlevel = data.healthperlevel;
            lifegen = data.lifegen;
            energy = data.energy;
            energygen = data.energygen;
            movespeed = data.movespeed;
            size = data.size;
            favouronkill = data.favouronkill;

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

        public string abilities;

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
            abilities = "";
            if(data.abilities != null)
            {
                foreach (string s in data.abilities)
                {
                    abilities += s + "\n";
                }
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
            abilities = data.abilities;

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
        public CraftedEdit(EquipmentData data) : base(data)
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
    public class ArmorEdit : CraftedEdit
    {
        public ArmorEdit()
        {
            this.name = "new armor";
            this.gameName = "new armor";
        }

        public ArmorEdit(ArmorData data)
            : base(data)
        {
            this.armor = data.armor;
            this.speedPenalty = data.speedPenalty;
            this.armorType = (ArmorType)data.armorType;
        }

        public ArmorEdit(ArmorEdit data)
            : base(data)
        {
            this.armor = data.armor;
            this.speedPenalty = data.speedPenalty;
            this.armorType = data.armorType;
        }

        public int armor;
        public int speedPenalty;
        public ArmorType armorType;
    }

    [Serializable]
    public enum ArmorType
    {
        Head,
        Chest,
        Boots,
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
            tags = new List<AbilityTags>();
            foreach (AbilityTags tag in Enum.GetValues(typeof(AbilityTags)))
            {
                if( (data.tags & (int)tag) != 0)
                {
                    tags.Add(tag);
                }
            }
            
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
            tags = data.tags;
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

        public List<AbilityTags> tags;
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
        public string rareDrops;
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

            rareDrops = data.rareDrops;
            
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
            rareDrops = "";
            if (data.rareDrops != null)
            {
                foreach (string s in data.rareDrops)
                {
                    rareDrops += s + "\n";
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

    #region Recipe Edits
    [Serializable]
    public class RecipeEdit : ObjectEdit
    {
        public string product;
        public List<IngredientEdit> ingredients;
        public string description;

        public Skills skill;
        public int expAmount;
        public int requiredSkillLevel;

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

            skill = data.skill;
            expAmount = data.expAmount;
            requiredSkillLevel = data.requiredSkillLevel;
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

            skill = data.skill;
            expAmount = data.expAmount;
            requiredSkillLevel = data.requiredSkillLevel;
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
            skill = Skills.Crafting;
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
            skill = Skills.Crafting;
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
            skill = Skills.Alchemy;
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
#endregion


}