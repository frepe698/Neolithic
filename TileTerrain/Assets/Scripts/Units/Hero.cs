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

    //COLDNESS
    public const float MAX_COLDNESS = 100;
    public const float FREEZING_LINE = 75;
    private float coldness;
    private const float COLDNESS_MULTIPLIER = 0.2f;
    private const float BASE_TEMPERATURE_INCREASE = -10.0f;
    private float armorWarmth = 0;
    private float buildingWarmth = 0;

    protected bool colliderActive = true;

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
        this.lineOfSight = 999999; // LOL

        respawnPosition = get2DPos();
        tile = new Vector2i(get2DPos());
        World.tileMap.getTile(tile).addActor(this);
		activate();
	}

	public override void activate()
	{
		if(isActive()) return; // already active
		this.gameObject = ObjectPoolingManager.Instance.GetObject(name);
        if (gameObject == null) return; //Object pool was full and no more objects are available
		
		ground();
        gameObject.transform.position = position;
        gameObject.transform.eulerAngles = rotation;
        gameObject.transform.localScale = scale;
        this.unitController = gameObject.GetComponent<UnitController>();
		unitController.setID(id);
		unitController.setWeapon(heldItem);

        foreach (ArmorData armor in equipedArmor)
        {
            if(armor != null) 
                unitController.equipArmor(name, armor);
        }
        setTag();
	}

    public override bool inactivate()
    {
        if (isActive())
        {
            unitController.unequipAll();
            return base.inactivate();
        }
        return false;
    }

    public override void updateHealthbar()
    {
        if (healthBar == null)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("GUI/hero_healthbar"));
            go.transform.SetParent(GameMaster.getGUIManager().getCanvas().FindChild("WorldUI"));
            healthBar = go.GetComponent<HealthbarController>();
            if(this == GameMaster.getPlayerHero())
                healthBar.setColor(Color.yellow);
            else
                healthBar.setColor(GameMode.teamColors[getTeam() - 2]);

        }

        if (!alive) healthBar.setActive(false);
        else
        {
            healthBar.update(this);
            healthBar.setActive(true);
        }
    }

	public void updateStats()
	{
        if (!alive)
            return;

		hunger += getHungerGain()*Time.deltaTime;
		hunger = Mathf.Clamp(hunger, 0, maxHunger);

        coldness = Mathf.Clamp(coldness + getColdnessGain()*Time.deltaTime, 0, MAX_COLDNESS);

        unitstats.getEnergy().addCurValue(getEnergyGain() * Time.deltaTime);
        //unitstats.getEnergy().addCurValue(unitstats.getEnergyRegen().getValue()*Time.deltaTime);

		unitstats.getHealth().addCurValue(getHealthGain()*Time.deltaTime);

        if (getHealth() <= 0)
            GameMaster.getGameController().requestKillActor(id, -1);
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
            if (isActive()) gameObject.GetComponent<UnitController>().setWeapon(heldItem);
        }
        else if (newItemData is ArmorData)
        {
            ArmorData data = (ArmorData)newItemData;
            if (equip)
            {
                equipedArmor[data.armorType] = data;
                if (isActive()) gameObject.GetComponent<UnitController>().equipArmor(name, data);
            }
            else //unequip
            {
                equipedArmor[data.armorType] = null;
                if (isActive()) gameObject.GetComponent<UnitController>().unequipArmor(data.armorType);
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
            if (isActive()) gameObject.GetComponent<UnitController>().setWeapon(heldItem);
        }
        else if (itemData is ArmorData)
        {
            ArmorData data = (ArmorData)itemData;
            equipedArmor[data.armorType] = data;
            if (isActive()) gameObject.GetComponent<UnitController>().equipArmor(name, data);
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
        float regen = unitstats.getHealthRegen().getValue();
        float change = 0;
		if(hunger <= 0)
		{
            regen = 0;
			change -= 2f;
		}
        if (coldness >= FREEZING_LINE)
        {
            change -= coldness / MAX_COLDNESS * 5;
        }
		return regen * getColdnessMultiplier() + change; 
	}

    public float getColdnessMultiplier()
    {
        return 1 - (coldness + float.Epsilon) / 100.0f;
    }

    public float getColdnessGain()
    {
        float airTemp = TimeManager.Instance.getCurTemperature();

        float total = airTemp + armorWarmth + buildingWarmth + BASE_TEMPERATURE_INCREASE;

        return -total * COLDNESS_MULTIPLIER;
    }

	public float getHungerGain()
	{
		if(moving)
		{
			return 2*BASE_HUNGER_GAIN;
		}
		return BASE_HUNGER_GAIN;
	}

    public void setWarmth(float warmth)
    {
        this.armorWarmth = warmth;
    }

    public void addWarmth(float warmth)
    {
        this.armorWarmth += warmth;
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

    public float getColdness()
    {
        return coldness;
    }

    public float getMaxColdness()
    {
        return MAX_COLDNESS;
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
        return base.getMovespeed() * (1 - (coldness + float.Epsilon) / 200.0f);// *penalty;
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
            //unit.GetComponent<Collider>().enabled = colliderActive;
            gameObject.tag = activate ? "Unit" : "Player";
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

    public override void onLevelUp()
    {
        if (isActive())
        {
            ParticleSystem particles = ParticlePoolingManager.Instance.GetObject("levelupParticles");
            if (particles != null)
            {
                particles.transform.position = new Vector3(position.x, position.y, position.z);
                particles.Play();
            }

            //unitController.playSound("levelup");
        }
    }

    public override void grantAbilityPoint()
    {
        skillManager.grantAbilityPoint();
    }

    public void startRespawn()
    {
        waitingForRespawn = true;
        deathTimer = RESPAWN_TIME + unitstats.getLevel();
    }

    public bool isWaitingRespawn()
    {
        return waitingForRespawn;
    }

    public bool updateRespawnTimer()
    {
        //Debug.Log("Hero is waiting for respawn. " + respawnTimer + " seconds left.");
        deathTimer -= Time.deltaTime;
        return deathTimer <= 0;
    }

    public void respawn()
    {
        buffs.Clear();
        unitstats.resetVitals();
        unitstats.updateStats();
        coldness = 0;
        waitingForRespawn = false;
        command = null;
        warp(respawnPosition);

    }

    public float getRespawnTime()
    {
        return deathTimer;
    }

    public override int getFavour()
    {
        return 100;
    }

    public override void onEnterNewTile()
    {
        calculateBuildingWarmth();
    }

    public void calculateBuildingWarmth()
    {
        buildingWarmth = 0;

        for (int x = tile.x - Building.WARMTH_TILE_RANGE; x < tile.x + Building.WARMTH_TILE_RANGE + 1; x++)
        {
            for (int y = tile.y - Building.WARMTH_TILE_RANGE; y < tile.y + Building.WARMTH_TILE_RANGE + 1; y++)
            {
                if (!World.getMap().isValidTile(x, y)) continue;
                Tile checkTile = World.getMap().getTile(x, y);
                if (checkTile.containsActors() /*&& Pathfinder.unhinderedTilePath(World.getMap(), get2DPos(), new Vector2(x, y), id)*/)
                {
                    foreach (Actor actor in checkTile.getActors())
                    {
                        Building building = actor as Building;
                        if (building == null) continue;
                        float distance = Vector2i.getDistance(new Vector2i(x, y), tile);
                        if (distance <= Building.WARMTH_TILE_RANGE)
                            buildingWarmth += (1 - (distance + float.Epsilon - 1) / (float)(Building.WARMTH_TILE_RANGE)) * building.getWarmth();
                    }
                }
            }
        }
        Debug.Log("bwarmth " + buildingWarmth);
    }

    public override void onDeath()
    {
        command = null;
        startRespawn();

        if (isActive())
            setAnimationRestart("die_unarmed");
    }

    public override bool shouldBeRemoved()
    {
        return false;
    }

    protected override void setTag()
    {
        if (gameObject != null)
        {
            if (alive)
                gameObject.tag = colliderActive ? "Unit" : "Player";
            else
                gameObject.tag = "DeadUnit";
        }
    }
}
