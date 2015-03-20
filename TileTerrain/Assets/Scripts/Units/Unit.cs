using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit {

	protected GameObject unit;
	protected string unitName;
    protected string modelName;
	protected UnitController unitController;

	protected Vector3 position;
	protected Vector3 rotation;
	protected Vector3 scale;

	protected bool awake;
	protected int lineOfSight;
	protected bool hostile;

	//protected float maxHealth;
	//protected float health;
	protected float size;

	protected int id;
	protected Vector2i destinationTile = new Vector2i();
	protected Vector2 destination;
	protected bool moving;
	protected Path path;
	protected Vector2i tile;

    protected const float BASE_MOVESPEED = 4;
	//protected float movespeed = 4;
	protected float speedMult = 1;
	protected float moveSensitivity = 0.1f;
	
	protected Command command;
    protected float commandEndTime;
	protected bool alive = true;

	protected float gatherTime = 0.8f;
	protected float lootTime = 0.25f;


    protected UnitStats unitstats;
    protected List<Buff> buffs;

    protected List<Ability> abilities;
	
	private AudioSource audio;
	
	public Unit(string unit, Vector3 position, Vector3 rotation, int id) 
	{
		this.unitName = unit;
		this.position = position;
		this.rotation = rotation;
		this.scale = new Vector3(1,1,1);
		this.id = id;

        this.buffs = new List<Buff>();

        abilities = new List<Ability>();
	}

	public Unit(string unit, Vector3 position, Vector3 rotation, int id, Vector3 scale) 
	{
		this.unitName = unit;
		this.position = position;
		this.rotation = rotation;
		this.scale = scale;
		this.id = id;

        this.buffs = new List<Buff>();



        abilities = new List<Ability>();
	}

    public void init()
    {
        tile = getTile();
        World.tileMap.getTile(tile).addUnit(this);
    }
	public virtual void activate()
	{
		if(isActive()) return; // already active
		this.unit = ObjectPoolingManager.Instance.GetObject(modelName);
		if(unit == null) return; //Object pool was full and no more objects are available

		ground();
		unit.transform.position = position;
		unit.transform.eulerAngles = rotation;
		unit.transform.localScale = scale;
		this.unitController = unit.GetComponent<UnitController>();
		unitController.setID(id);
		//audio = unit.AddComponent<AudioSource>();
		//audio.minDistance = 4;
		//audio.maxDistance = 15;
		//audio.dopplerLevel = 0;
	}

	public void setAwake(bool awake)
	{
		this.awake = awake;
	}

	public bool isAwake()
	{
		return this.awake;
	}



	public void inactivate()
	{
		if(!isActive()) return; //Already inactive
		ObjectPoolingManager.Instance.ReturnObject(modelName, unit);		
		unit = null;
		unitController = null;
	}

	public virtual void updateAI()
	{

	}
	
	public void update ()
	{
		if(awake == false) 
		{
			return;
		}
		if(command != null)
		{
			if(command.isCompleted()) command = null;
			else command.update ();
		}
		if(moving && command != null)
		{
            float deltaMove = Time.deltaTime * getAdjustedMovespeed();
            float distanceToCheckPoint = Vector2.Distance(destination, get2DPos());

            if (distanceToCheckPoint < deltaMove)
            {

                position = new Vector3(destination.x, position.y, destination.y);
                deltaMove -= distanceToCheckPoint;
                if (path.getCheckPointCount() > 0) destination = path.popCheckPoint();
                else moving = false;
            }
            /*
            int counter = 0;
            while (distanceToCheckPoint < deltaMove && path.getCheckPointCount() > 0)
            {
                counter++;
                position = new Vector3(destination.x, position.y, destination.y);
                deltaMove -= distanceToCheckPoint;
                destination = path.popCheckPoint();
                if (path.getCheckPointCount() == 0) moving = false;
                distanceToCheckPoint = Vector2.Distance(destination, get2DPos());
            }
            Debug.Log(counter);
                
            */
            //position = new Vector3(destination.x,position.y, destination.y);
            //if(path.getCheckPointCount() > 0) destination = path.popCheckPoint();
            //else moving = false;


            /*
            while (distanceToCheckPoint < deltaMove && path.getCheckPointCount() > 0)
                {
                    position = new Vector3(destination.x, position.y, destination.y);
                    deltaMove -= distanceToCheckPoint;
                    destination = path.popCheckPoint();
                    if (path.getCheckPointCount() == 0) moving = false;
                    distanceToCheckPoint = Vector2.Distance(destination, get2DPos());
                }*/
            if (moving)
			{
				Vector2 dir = (destination-get2DPos()).normalized;
				//Here is some new stuff for unit collision
				
				Vector3 newPos = position + new Vector3(dir.x, 0, dir.y)*(deltaMove);
				Vector2i newTile = new Vector2i(newPos);
				if(newTile != tile)
				{

                    //ERROR: This checks tiles outside of the tilemap
					if(!World.tileMap.getTile(newTile).isWalkable(id))
					{
						Vector2 start = new Vector2(getTile().x + 0.5f, getTile().y + 0.5f);
						path = Pathfinder.findPath(World.tileMap,start,command.getDestination(),id); //start was get2DPos earlier
						if(path.getCheckPointCount() > 0) 
						{
							destination = path.popCheckPoint();
						}
						else moving = false;		
						
						return;
					}
					else
					{
						World.tileMap.getTile(tile).removeUnit(this);
						World.tileMap.getTile(newTile).addUnit(this);
						
						
					}
				}
				tile = newTile;
				position =  newPos;
                setRotation( new Vector3(0, Mathf.Rad2Deg*Mathf.Atan2(dir.x, dir.y) + 180, 0) );
				ground();
                setAnimation(getRunAnim(), getAdjustedMovespeed() / BASE_MOVESPEED);

			}
		}
		else
		{
			if(command == null && Time.time >= commandEndTime)
			{
				setAnimation( getIdleAnim(), 1);
			}
		}
		updateTransform();
        updateAbilities();
        updateBuffs();
	}

	protected virtual void updateTransform()
	{
		if(!isActive()) return;

		unit.transform.position = this.position;
        
        Quaternion target = Quaternion.Euler(this.rotation);
        Quaternion current = unit.transform.rotation;
        Quaternion newAngle = Quaternion.RotateTowards(current, target, 1080*Time.deltaTime);
        unit.transform.rotation = newAngle;

		unit.transform.localScale = this.scale;

	}
	
	public int getID()
	{
		return id;
	}

	public void setID(int id)
	{
		this.id = id;
	}

    public void setCommandEndTime(float time)
    {
        commandEndTime = time;
    }

	public void giveCommand(Command command)
	{
		this.command = command;
		this.command.start ();
	}
	
	public void giveMoveCommand(Vector2 point)
	{
		if(!World.tileMap.isValidTile(new Vector2i(point))) return;
		Command newCommand = new MoveCommand(this, point);
		if(commandEquals(command, newCommand)) return;
		command = newCommand;
		command.start();
	}
	
	public void giveGatherCommand(Vector2 target)
	{
		ResourceObject rObject = World.tileMap.getTile(new Vector2i(target)).getResourceObject();
		if(rObject == null) return;
		Command newCommand = new GatherCommand(this, rObject);
		//		if(commandEquals(command, newCommand))
		//		{
		//			Debug.Log ("Already chopping that tree");
		//			return;
		//		}
		command = newCommand;
		command.start();
	}

   

    public virtual Ability getBasicAttack()
    {
        return null;
    }

	public void giveAttackCommand(Unit target)
	{
		if(target == null) return;
		command = new AbilityCommand(this, target, getBasicAttack());
		command.start ();
	}

    public void giveAttackCommand(Vector3 target)
    {
        if (target == null) return;
        command = new AbilityCommand(this, target, getBasicAttack());
        command.start();
    }


    public void giveAbilityCommand(Unit target, int ability)
    {
        if (target == null) return;
        command = new AbilityCommand(this, target, abilities[ability]);
        command.start();
    }

    public void giveAbilityCommand(Vector3 target, int ability)
    {
        command = new AbilityCommand(this, target, abilities[ability]);
        command.start();
    }

	public void giveLootCommand(Vector2i targetTile, int lootID)
	{
		LootableObject lObject = World.tileMap.getTile(targetTile).getLootableObject(lootID);
		if(lObject == null) return;
		Command newCommand = new LootCommand(this, lObject);
		if(commandEquals(command, newCommand))
		{
			Debug.Log ("Already looting that loot");
			return;
		}
		command = newCommand;
		command.start();
	}

    public bool canStartCommand(Command command)
    {
        if (this.command != null && !this.command.canBeOverridden()) return false;
        if (command.canAlwaysStart()) return true;
        if (!command.canStartOverride(this.command)) { return false; Debug.Log("cant override"); }
        return Time.time >= commandEndTime;
    }

    public bool canOverrideCurrentCommand()
    {
        if (this.command == null) return true;
        else return this.command.canBeOverridden();
    }
	
	public bool currentCommandEquals(Command command)
	{
		return commandEquals(command, this.command);
	}
	
	private void setCommand(Command command)
	{
		this.command = command;
	}
	
	protected void ground()
	{
		RaycastHit hit;
		if(Physics.Raycast(position + Vector3.up*50, Vector3.down, out hit, Mathf.Infinity , 1 << 8))
		{
			position = new Vector3(position.x, hit.point.y, position.z);
			speedMult = hit.normal.y;
		}
	}
	
	public void setPath(Vector2 point)
	{
        float deltaMove = Time.deltaTime * getAdjustedMovespeed();
		
		if(Vector2.Distance(point, get2DPos()) < deltaMove)
		{
			Vector2 dir = (point-get2DPos()).normalized;
            setRotation( new Vector3(0, Mathf.Rad2Deg*Mathf.Atan2(dir.x, dir.y) + 180, 0) );
			return;
		}

        if (Pathfinder.getClosestWalkablePoint(World.tileMap, point, get2DPos(), out point, id))
        {
            Vector2i clickedTile = new Vector2i(point);
            command.setDestination(point);
            if (clickedTile == destinationTile)
            {
                if (path.getCheckPointCount() < 1)
                {
                    //path.addCheckPoint(point);
                    destination = point;
                }
                else
                {
                    path.setPoint(path.getCheckPointCount() - 1, point);
                }

                moving = true;
            }
            else
            {
                path = Pathfinder.findPath(World.tileMap, get2DPos(), point, id);
                if (path.getCheckPointCount() > 0)
                {
#if false
                    for (int i = 0; i < path.getCheckPointCount() - 1; i++)
                    {
                        Debug.DrawLine(new Vector3(path.getPoint(i).x, 4, path.getPoint(i).y), new Vector3(path.getPoint(i + 1).x, 4, path.getPoint(i + 1).y), Color.white, 3);
                    } 
#endif
                    moving = true;
                    destination = path.popCheckPoint();
                }
                else
                {
                    moving = false;
                    command = null;
                }
            }
            destinationTile = clickedTile;

        }
        else
        {
            moving = false;
            command = null;
        }
	}
	
	public Vector2 get2DPos()
	{
		return new Vector2(position.x, position.z);
	}
	
	public Vector2i getTile()
	{
		return new Vector2i(position.x, position.z);
	}

	public void setPosition(Vector3 position)
	{
		this.position = position;
	}
	
	public void setMoving(bool moving)
	{
		this.moving = moving;
	}
	
	public void gather(ResourceObject resourceObject)
	{
		GameMaster.getGameController().requestGather(id, new Vector2i(resourceObject.getPosition()));
	}

	public void attack(Unit target)
	{
		GameMaster.getGameController().requestAttack(id, target.getID());
	}

	/*public void fireProjectile(Vector3 target)
	{
		GameMaster.getGameController().requestFireProjectile(id, target);
	}*/
	
	public void loot(LootableObject lootableObject)
	{
		GameMaster.getGameController().requestLoot(id, new Vector2i(lootableObject.get2DPos().x, lootableObject.get2DPos().y), lootableObject.getID());
	}

    public void warp(WarpObject warpObject)
    {
        GameMaster.getGameController().requestWarp(id, new Vector2i(warpObject.get2DPos()));
    }

	public Vector3 getPosition()
	{
		return position;
	}

	public void warp(Vector2 position)
	{
        World.tileMap.getTile(tile).removeUnit(this);
		this.position = new Vector3(position.x, World.getHeight(new Vector2(position.x, position.y)), position.y);
        
        Vector2i newTile = new Vector2i(this.position.x, this.position.z);
        World.tileMap.getTile(newTile).addUnit(this);
        tile = newTile;
        //ground();
	}

	public Vector3 getRotation()
	{
		return rotation;
	}

	public void setRotation(Vector3 rotation)
	{
		this.rotation = rotation;
	}

	public virtual void playWeaponAttackAnimation(float speed = 1)
	{
		return;
	}

	public void setAnimation(string animation, float speed = 1)
	{
		if(isActive()) unitController.setAnimation(animation, speed);
	}
	
	public void setAnimationRestart(string animation, float speed = 1)
	{
		if(isActive()) unitController.setAnimationRestart(animation, speed);
	}
	
	public void playSound(string sound)
	{
		if(!isActive()) return;
		unitController.playSound(sound);
	}
	
	private bool commandEquals(Command c1, Command c2)
	{
		if(c1 == null || c2 == null) return false;
		return c1.Equals(c2);
	}
	
	
	public virtual float getAttackSpeed()
	{
		return gatherTime;
	}
	
	public virtual float getLootTime()
	{
		return lootTime;
	}
	
	public virtual int getDamage(int damageType)
	{
		return 0;
	}

    public virtual int getBaseDamage(int damageType)
    {
        return 0;
    }
	
	public virtual string getIdleAnim()
	{
		return null;
	}
	
	public virtual string getRunAnim()
	{
		return null;
	}
	
	public virtual string getLootAnim()
	{
		return null;
	}
	
	public virtual string getAttackAnim(int damageType)
	{
		return null;
	}
	
	public virtual string getAttackSound(int damageType)
	{
		return "punch01";
	}

	public bool isActive()
	{
		return (unit != null && unit.activeSelf == true);
		
	}

	public bool isHostile()
	{
		return hostile;
	}

	public void setHostile(bool hostile)
	{
		this.hostile = hostile;
	}

	public override bool Equals(object other)
	{
		Unit otherUnit = other as Unit;
		if(otherUnit == null) return false;
		return otherUnit.getID() == id;
	}

	public override int GetHashCode()
	{
		return id;
	}

	public virtual void takeDamage(int damage, int dealerID)
	{
        //TODO:(kanske) move damage reduction to server side
        float reducedDamage = Mathf.Max(1, damage - unitstats.getStatV(Stat.Armor));
        unitstats.getHealth().addCurValue(-reducedDamage);

        if (isActive())
        {
            ParticleSystem particles = ParticlePoolingManager.Instance.GetObject("bloodParticles");
            if (particles != null)
            {
                particles.transform.position = new Vector3(position.x, position.y + 1, position.z);
                particles.Play();
            }
        }
	}

	public void heal(int heal)
	{
		//this.health += heal;
        unitstats.getHealth().addCurValue(heal);
	}

	public virtual float getMovespeed()
	{
		//return movespeed;
        return unitstats.getMovespeed().getValue();
	}

    public float getAdjustedMovespeed()
    {
        return getMovespeed() * speedMult;
    }

	public float getHealth()
	{
		//return this.health;
        return unitstats.getHealth().getCurValue();
	}

	public float getMaxHealth()
	{
		//return this.maxHealth;
        return unitstats.getHealth().getValue();
	}

	public void setAlive(bool alive)
	{
		this.alive = alive;
	}

	public bool isAlive()
	{
		return alive;
	}




	public string getName()
	{
		return unitName;
	}

	public int getLineOfSight()
	{
		return lineOfSight;
	}

	public virtual string getProjectileName()
	{
		return "woodenArrow"; // TODO ingen läser de här men fixa skiten
	}

	public virtual bool isMelee()
	{
		return false;
	}

	public float getSize()
	{
		return size;
	}

    public virtual int getTeam()
    {
        return 0;
    }

    public UnitStats getUnitStats()
    {
        return unitstats;
    }

    public virtual ArmorData[] getEquipedArmor()
    {
        return new ArmorData[0];
    }

    public virtual void grantExperience(int skill, int exp)
    {

    }

    public virtual void increaseSkillLevel()
    {

    }

    public void onLevelUp()
    {
        if (isActive())
        {
            ParticleSystem particles = ParticlePoolingManager.Instance.GetObject("levelupParticles");
            if (particles != null)
            {
                particles.transform.position = new Vector3(position.x, position.y, position.z);
                particles.Play();
            }

            unitController.playSound("levelup");
        }
    }

    public virtual void grantAbilityPoint()
    {
        //do nada
    }

    public void learnAbility(string ability)
    {
        GameMaster.getGameController().requestLearnAbility(ability, getID());
    }
    public virtual void addAbility(string ability)
    {
        abilities.Add(new Ability(ability, this));
    }

    public Ability getAbility(int ability)
    {
        return abilities[ability];
    }

    public bool hasAbility(int ability)
    {
        return ability < abilities.Count;
    }

    private void updateAbilities()
    {
        foreach(Ability a in abilities)
        {
            a.update();
        }
    }

    public virtual int getWeaponTags()
    {
        return int.MaxValue;
    }

    public void addToStat(Stat stat, float value)
    {
        unitstats.addToStat(stat, value);
        unitstats.updateStats();
    }

    public void addMultiplierToStat(Stat stat, float value)
    {
        unitstats.addMultiplierToStat(stat, value);
        unitstats.updateStats();
    }

    public void addBuff(Buff buff)
    {
        buffs.Add(buff);
    }

    public void removeBuff(Buff buff)
    {
        buffs.Remove(buff);
    }

    public void updateBuffs()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            Buff buff = buffs[i];
            buff.update();
            if (buff.isFinished())
            {
                buff.remove();
                i--;
            }
        }
    }

    public GameObject addEffectObject(GameObject prefab, Vector3 position)
    {
        if (isActive())
        {
            return unitController.addEffectObject(prefab, position);
        }
        return null;
    }

}
