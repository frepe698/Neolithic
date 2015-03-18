using UnityEngine;
using System.Collections;

public class Hero : Unit {

	private Inventory inventory;
	private WeaponData heldItem;

    private ArmorData[] equipedArmor = new ArmorData[3];


	//HERO STATS

	//HUNGER
	private float maxHunger = 100;
	private float hunger;
	private readonly float BASE_HUNGER_GAIN;


	//ENERGY
	//private float maxEnergy = 100;
	//private float energy;
	//private readonly float BASE_ENERGY_GAIN; 

	//LIFE
	//protected readonly float BASE_LIFE_GAIN;
	
	//Attributes
	private int stamina;
	private int vitality;
	private int dexterity;


    protected bool colliderActive = true;

	public Hero(string unit, Vector3 position, Vector3 rotation, int id) 
		: base(unit, position, rotation, id, new Vector3(1,1,1))
	{
		UnitData data = DataHolder.Instance.getUnitData(unit);
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
		setItem(DataHolder.Instance.getEquipmentData("unarmed"));

        unitstats.updateStats();

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
	

	public override float getAttackSpeed()
	{
		return heldItem.attackSpeed*unitstats.getStatV(Stat.IncreasedAttackspeed);
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
		float penalty = 1;
		if(getEnergy() < 10)
		{
			penalty = 0.5f;
		}
		return base.getMovespeed()*penalty;
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
        return 2;
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

}
