using UnityEngine;
using System.Collections;
using System;

public class Hero : Unit {

    public EventHandler onAbilityUpdatedListener;

	private Inventory inventory;
	private WeaponData heldItem;

    private ArmorData[] equipedArmor = new ArmorData[3];

    protected SkillManager skillManager;

    public const int MAX_ABILITY_COUNT = 4;
    private Ability meleeBasicAttack;
    private Ability rangedBasicAttack;

    private int team;

	//HUNGER
	private float maxHunger = 100;
	private float hunger;
	private readonly float BASE_HUNGER_GAIN;

    protected bool colliderActive = true;

    private float respawnTimer;
    private const float RESPAWN_TIME = 10;
    private bool waitingForRespawn = false;
    private Vector2 respawnPosition;

	public Hero(string unit, Vector3 position, Vector3 rotation, int id, int team) 
		: base(unit, position, rotation, id, new Vector3(1,1,1))
	{
		HeroData data = DataHolder.Instance.getHeroData(unit);
		if(data == null) 
		{
			hostile = true;
			//health = 100.0f;
			//maxHealth = 100.0f;
			//energy = 100.0f;
			//maxEnergy = 100.0f;
			//movespeed = 4;
			lineOfSight = 8;
			size = 0.5f;
			hunger = maxHunger;
			//BASE_ENERGY_GAIN = 10;
			//BASE_LIFE_GAIN = 5;
			BASE_HUNGER_GAIN = -0.2f;
			Debug.Log ("The unit data you are looking for does not exist: " + unit);
		}
		else
		{
			hostile = true;
			//health = (float)data.health;
			//maxHealth = (float)data.health;
			//energy = data.energy;
			//maxEnergy = data.energy;
			hunger = maxHunger;
			//BASE_LIFE_GAIN = data.lifegen;
			//BASE_ENERGY_GAIN = data.energygen;
			BASE_HUNGER_GAIN = -0.2f;
			//movespeed = data.movespeed;
			lineOfSight = 0;
			size = data.size;
            modelName = data.modelName;
		}
        init();
		inventory = new Inventory();

        this.skillManager = new SkillManager(this);
        unitstats = new HeroStats(this, 0, data);
       
		setItem(DataHolder.Instance.getEquipmentData("unarmed"));

        unitstats.updateStats();

        meleeBasicAttack = new Ability("meleebasicattack", this);
        rangedBasicAttack = new Ability("rangedbasicattack", this);

        this.team = team;
        respawnPosition = get2DPos();
		activate();
	}

	public override void activate()
	{
		if(isActive()) return; // already active
		this.unit = ObjectPoolingManager.Instance.GetObject(unitName);
		if(unit == null) return; //Object pool was full and no more objects are available
		
		ground();
		unit.transform.position = position;
		unit.transform.eulerAngles = rotation;
		unit.transform.localScale = scale;
		this.unitController = unit.GetComponent<UnitController>();
		unitController.setID(id);
		unitController.setWeapon(heldItem);

        unit.GetComponent<Collider>().enabled = colliderActive;
	}

	public void updateStats()
	{
		hunger += getHungerGain()*Time.deltaTime;
		hunger = Mathf.Clamp(hunger, 0, maxHunger);

        unitstats.getEnergy().addCurValue(getEnergyGain() * Time.deltaTime);
        //unitstats.getEnergy().addCurValue(unitstats.getEnergyRegen().getValue()*Time.deltaTime);
		unitstats.getHealth().addCurValue(getHealthGain()*Time.deltaTime);
		//health = Mathf.Clamp(health, 0, maxHealth);

	}

	public Inventory getInventory()
	{
		return inventory;
	}

	public void setItem(int itemIndex)
	{
        EquipmentData newItemData;
        bool equip = inventory.setEquipedItem(itemIndex, out newItemData);

        if (newItemData is WeaponData)
        {
            heldItem = (WeaponData)newItemData;
            if (isActive()) unit.GetComponent<UnitController>().setWeapon(heldItem);
        }
        else if (newItemData is ArmorData)
        {
            ArmorData data = (ArmorData)newItemData;
            if (equip)
            {
                equipedArmor[data.armorType] = data;
                if (isActive()) unit.GetComponent<UnitController>().equipArmor(unitName, data);
            }
            else //unequip
            {
                equipedArmor[data.armorType] = null;
                if (isActive()) unit.GetComponent<UnitController>().unequipArmor(data.armorType);
            }
        }
        //Update stats
        unitstats.updateStats();
	}

    public void setItem(EquipmentData itemData)
    {
        if (itemData is WeaponData)
        {
            heldItem = (WeaponData)itemData;
            if (isActive()) unit.GetComponent<UnitController>().setWeapon(heldItem);
        }
        else if (itemData is ArmorData)
        {
            ArmorData data = (ArmorData)itemData;
            equipedArmor[data.armorType] = data;
            if (isActive()) unit.GetComponent<UnitController>().equipArmor(unitName, data);
        }
        //Update stats
        unitstats.updateStats();

    }

    public override Ability getBasicAttack()
    {
        return isMelee() ? meleeBasicAttack : rangedBasicAttack;
    }

    public override float getBaseAttackSpeed()
    {
        return heldItem.attackSpeed;
    }

	public override float getAttackSpeed()
	{
		return unitstats.getStatV(Stat.Attackspeed);
	}
	
	public override int getDamage(int damageType)
	{
        Debug.Log((int)unitstats.getDamage(damageType, isMelee()));
        return (int)unitstats.getDamage(damageType, isMelee());
#if false
		if(heldItem != null && damageType >= 0 && damageType < 3)
		{
            float damage;
            if (heldItem.isMelee()) 
                damage = heldItem.getDamage(damageType) * unitstats.getMeleeDamageMultiplier(damageType);
			else
                damage = heldItem.getDamage(damageType) * unitstats.getRangedDamageMultiplier(damageType);

            if(damageType == DamageType.COMBAT)
            {
                //Add tree and stone damage to attack damage
                //TODO(kanske): Change so it only applies to melee attacks
                float treeToDamage = unitstats.getStatV(Stat.TreeToAttack);
                if (treeToDamage > 0)
                    damage += getDamage(DamageType.TREE) * treeToDamage;

                float stoneToDamage = unitstats.getStatV(Stat.StoneToAttack);
                if (stoneToDamage > 0)
                    damage += getDamage(DamageType.STONE) * stoneToDamage;
            }

            return (int)damage;
        }
		else
		{
			Debug.Log (damageType);
			return 0;
		}
#endif
	}

    public override int getBaseDamage(int damageType)
    {
        return heldItem.getDamage(damageType);
    }

	public override string getIdleAnim()
	{
		if(heldItem == null) return null;
		return heldItem.idleAnim;
	}
	
	public override string getRunAnim()
	{
		return heldItem.runAnim;
	}
	
	public override string getLootAnim()
	{
		return heldItem.lootAnim;
	}
	
	public override string getAttackAnim(int damageType)
	{
		return heldItem.getAttackAnim(damageType);
	}

	public override string getAttackSound (int damageType)
	{
		return heldItem.getAttackSound(damageType);
	}

	public float getEnergyGain()
	{
		float punishment = 0;
		if(hunger <= 0)
		{
			return -0.5f;
		}

		if(moving)
		{
			punishment = unitstats.getEnergyRegen().getValue()/2;
		}
		
		return unitstats.getEnergyRegen().getValue() - punishment; //TODO kom på hur det ska va
	}

	public float getHealthGain()
	{

		if(hunger <= 0)
		{
			return -0.5f;
		}
		return unitstats.getHealthRegen().getValue(); 
	}

	public float getHungerGain()
	{
		if(moving)
		{
			return 2*BASE_HUNGER_GAIN;
		}
		return BASE_HUNGER_GAIN;
	}

	public void changeHunger(float change)
	{
		hunger += change;
		hunger = Mathf.Clamp(hunger, 0, maxHunger);
	}

	public void changeEnergy(float change)
	{
		unitstats.getEnergy().addCurValue(change);
		//energy = Mathf.Clamp(energy, 0, maxEnergy);
	}

	public float getEnergy()
	{
		return unitstats.getEnergy().getCurValue();
	}

	public float getHunger()
	{
		return hunger;
	}

	public float getMaxHunger()
	{
		return maxHunger;
	}

	public float getMaxEnergy()
	{
		return unitstats.getEnergy().getValue();
	}

	public override float getMovespeed ()
	{
        /*
		float penalty = 1;
		if(getEnergy() < 10)
		{
			penalty = 0.5f;
		}*/
        return base.getMovespeed();// *penalty;
	}

	public override void playWeaponAttackAnimation(float speed = 1)
	{
		if(isActive()) unitController.setWeaponAnimation(heldItem.weaponAttackAnim, heldItem.rightHand, speed);
	}

	public override bool isMelee()
	{
		return heldItem.isMelee();
	}

	public override string getProjectileName()
	{
		RangedWeaponData rwd = heldItem as RangedWeaponData; 
		if(rwd != null) return rwd.projectileName;
		return null;
	}

    public override ArmorData[] getEquipedArmor()
    {
        return equipedArmor;
    }

    public override int getTeam()
    {
        return team;
    }

    public void activateCollider(bool activate)
    {
        colliderActive = activate;
        if (isActive())
        {
            unit.GetComponent<Collider>().enabled = colliderActive;
        }
    }

    public override void grantExperience(int skill, int exp)
    {
        skillManager.giveExperience(skill, exp);
    }

    public override void increaseSkillLevel()
    {
        unitstats.increaseSkillLevel();
    }


    public override int getWeaponTags()
    {
        return heldItem.tags;
    }

    private void onAbilityUpdated()
    {
        if (onAbilityUpdatedListener != null)
            onAbilityUpdatedListener(this, EventArgs.Empty);
    }

    public void learnAbility(string ability, int index)
    {
        GameMaster.getGameController().requestLearnAbility(ability, getID(), index);
    }

    public override void addAbility(string ability)
    {
        base.addAbility(ability);
        onAbilityUpdated();
    }

    public virtual void addAbility(string ability, int index)
    {
        if (abilities.Count < index || abilities.Count < MAX_ABILITY_COUNT)
            abilities.Add(new Ability(ability, this));
        else
            abilities[index] = new Ability(ability, this);

        onAbilityUpdated();
    }

    public HeroStats getHeroStats()
    {
        return unitstats as HeroStats;
    }

    public SkillManager getSkillManager()
    {
        return skillManager;
    }

    public override void grantAbilityPoint()
    {
        skillManager.grantAbilityPoint();
    }

    public void startRespawn()
    {
        waitingForRespawn = true;
        respawnTimer = RESPAWN_TIME;
    }

    public bool isWaitingRespawn()
    {
        return waitingForRespawn;
    }

    public bool updateRespawnTimer()
    {
        //Debug.Log("Hero is waiting for respawn. " + respawnTimer + " seconds left.");
        respawnTimer -= Time.deltaTime;
        return respawnTimer <= 0;
    }

    public void respawn()
    {
        buffs.Clear();
        unitstats.resetVitals();
        unitstats.updateStats();
        waitingForRespawn = false;
        warp(respawnPosition);

    }

    public override int getFavour()
    {
        return 100;
    }
}
