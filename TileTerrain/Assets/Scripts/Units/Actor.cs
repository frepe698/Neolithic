using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Actor {

    protected GameObject gameObject;
    protected UnitController unitController;
    protected string name;
    protected string modelName;

    protected Vector3 position;
    protected Vector3 rotation;
    protected Vector3 scale;
    protected Vector2i tile;

    protected int id;

    protected bool awake;
    protected bool alive = true;
    protected int lineOfSight;
    protected bool hostile;

    protected Command command;
    protected string lastCommand = "nothing";
    protected float commandEndTime;

    protected List<Buff> buffs;
    protected List<Ability> abilities;
    protected UnitStats unitstats;

    protected HealthbarController healthBar;

    public Actor(string name, Vector3 position, Vector3 rotation, int id)
    {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
        this.id = id;

        this.scale = new Vector3(1, 1, 1);

        this.buffs = new List<Buff>();
        abilities = new List<Ability>();
    }

    public Actor(string name, Vector3 position, Vector3 rotation, int id, Vector3 scale)
    {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
        this.id = id;

        this.scale = scale;

        this.buffs = new List<Buff>();
        abilities = new List<Ability>();
    }

    public void init()
    {
        tile = new Vector2i(get2DPos());
        World.tileMap.getTile(tile).addActor(this);
        onEnterNewTile();
    }

    public virtual void activate()
    {
        if (isActive()) return; // already active
        this.gameObject = ObjectPoolingManager.Instance.GetObject(modelName);
        if (gameObject == null) return; //Object pool was full and no more objects are available

        ground();
        gameObject.transform.position = position;
        gameObject.transform.eulerAngles = rotation;
        //unit.transform.localScale = scale;
        this.unitController = gameObject.GetComponent<UnitController>();
        unitController.setID(id);
        setTag();
    }

    public virtual bool inactivate()
    {
        if (!isActive()) return false; //Already inactive
        ObjectPoolingManager.Instance.ReturnObject(modelName, gameObject);
        gameObject = null;

        if (healthBar != null)
        {
            GameObject.Destroy(healthBar.gameObject);
            healthBar = null;
        }
        return true;
    }

    public virtual void updateHealthbar()
    {
        if (healthBar == null)
        {
            if(getHealth() >= getMaxHealth() - 1)
                return;

            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("GUI/ai_healthbar"));
            go.transform.SetParent(GameMaster.getGUIManager().getCanvas().FindChild("WorldUI"));
            healthBar = go.GetComponent<HealthbarController>();
            healthBar.setColor(Color.red);

        }

        if (!alive || getHealth() == getMaxHealth()) healthBar.setActive(false);
        else
        {
            healthBar.update(this);
            healthBar.setActive(true);
        }
    }

    public virtual void setAwake(bool awake)
    {
        this.awake = awake;
    }

    public bool isAwake()
    {
        return this.awake;
    }

    public virtual void updateAI()
    {

    }

    public abstract void update();

    public void move(Vector2 amount)
    {
        Vector3 newPos = position + new Vector3(amount.x, 0, amount.y);
        Vector2i newTile = new Vector2i(newPos);
        if (newTile != tile)
        {

            World.tileMap.getTile(tile).removeActor(this);
            World.tileMap.getTile(newTile).addActor(this);
        }
        tile = newTile;
        position = newPos;
        ground();
    }

    public void warp(WarpObject warpObject)
    {
        GameMaster.getGameController().requestWarp(id, new Vector2i(warpObject.get2DPos()));
    }

    public void warp(Vector2 position)
    {
        World.tileMap.getTile(tile).removeActor(this);
        this.position = new Vector3(position.x, World.getHeight(new Vector2(position.x, position.y)), position.y);

        Vector2i newTile = new Vector2i(this.position.x, this.position.z);
        World.tileMap.getTile(newTile).addActor(this);
        tile = newTile;
        //ground();
    }

    public void attack(Unit target)
    {
        GameMaster.getGameController().requestAttack(id, target.getID());
    }

    public void setCommandEndTime(float time)
    {
        commandEndTime = time;
    }

    public void giveCommand(Command command)
    {
        this.command = command;
        this.lastCommand = command.getName();
        this.command.start();
    }

    public virtual Ability getBasicAttack()
    {
        return null;
    }

    public void giveAttackCommand(Actor target)
    {
        if (target == null) return;
        command = new AbilityCommand(this, target, getBasicAttack());
        this.lastCommand = command.getName();
        command.start();
    }

    public void giveAttackCommand(Vector3 target)
    {
        command = new AbilityCommand(this, target, getBasicAttack());
        this.lastCommand = command.getName();
        command.start();
    }

    public void giveAbilityCommand(Actor target, int ability)
    {
        if (target == null) return;

        Ability ab = abilities[ability];
        AbilityCommand newCommand = new AbilityCommand(this, target, ab);
        if (ab.data.totalTime > float.Epsilon)
        {
            command = newCommand;
            this.lastCommand = command.getName();
        }
        newCommand.start();
    }

    public void giveAbilityCommand(Vector3 target, int ability)
    {
        Ability ab = abilities[ability];
        AbilityCommand newCommand = new AbilityCommand(this, target, ab);
        if (ab.data.totalTime > float.Epsilon)
        {
            command = newCommand;
            this.lastCommand = command.getName();
        }
        newCommand.start();
    }

    public bool canStartCommand(Command command)
    {
        //if (command.canAlwaysStart()) return true;
        //if (this.command != null && !this.command.canBeOverridden()) return false;
        //if (command.canAlmostAlwaysStart()) return true;
        if (!command.canStartOverride(this.command)) { return false; }
        if (this.lastCommand.Equals(command.getName()))
            return Time.time >= commandEndTime;

        return true;
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

    protected void setCommand(Command command)
    {
        this.command = command;
    }

    public Command getCommand()
    {
        return command;
    }

    protected bool commandEquals(Command c1, Command c2)
    {
        if (c1 == null || c2 == null) return false;
        return c1.Equals(c2);
    }

    public void setAnimation(string animation, float speed = 1)
    {
        if (isActive()) unitController.setAnimation(animation, speed);
    }

    public void setAnimationRestart(string animation, float speed = 1)
    {
        if (isActive()) unitController.setAnimationRestart(animation, speed);
    }

    public void playSound(string sound)
    {
        if (!isActive()) return;
        unitController.playSound(sound);
    }

    public virtual void playWeaponAttackAnimation(float speed = 1)
    {
        return;
    }

    public bool isActive()
    {
        return (gameObject != null && gameObject.activeSelf == true);
    }

    public int getID()
    {
        return id;
    }

    public void setID(int id)
    {
        this.id = id;
    }

    protected virtual void ground()
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 50, Vector3.down, out hit, Mathf.Infinity, 1 << 8))
        {
            position = new Vector3(position.x, hit.point.y, position.z);
        }
    }

    public Vector2 get2DPos()
    {
        return new Vector2(position.x, position.z);
    }

    public Vector2i getTile()
    {
        return tile;
    }

    public virtual void setPosition(Vector3 position)
    {
        this.position = position;
    }

    public Vector3 getPosition()
    {
        return position;
    }

    public Vector3 getRotation()
    {
        return rotation;
    }

    public void setRotation(Vector3 rotation)
    {
        this.rotation = rotation;
    }

    public virtual float getBaseAttackSpeed()
    {
        return 0;
    }

    public virtual float getAttackSpeed()
    {
        return 0;
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

    public virtual string getAttackAnim(int damageType)
    {
        return null;
    }

    public virtual string getAttackSound(int damageType)
    {
        return "punch01";
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
        Actor otherActor = other as Actor;
        if (otherActor == null) return false;
        return otherActor.getID() == id;
    }

    public override int GetHashCode()
    {
        return id;
    }

    public virtual bool setPath(Vector2 point)
    {
        return false;
    }

    public virtual void setMoving(bool moving)
    {
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

    public virtual void heal(int heal)
    {
        //this.health += heal;
    }

    public virtual float getHealth()
    {
        //return this.health;
        return unitstats.getCurHealth() ;
    }

    public virtual float getMaxHealth()
    {
        //return this.maxHealth;
        return unitstats.getMaxHealth();
    }

    public UnitStats getUnitStats()
    {
        return unitstats;
    }

    public virtual void onLevelUp()
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

    public void addToStat(Stat stat, float value)
    {
        //Debug.Log("add to stat " + stat.ToString());
        unitstats.addToStat(stat, value);
    }

    public void addMultiplierToStat(Stat stat, float value)
    {
        //Debug.Log("add to stat " + stat.ToString());
        unitstats.addMultiplierToStat(stat, value);
    }

    public void addBuff(Buff buff)
    {
        buffs.Add(buff);
        unitstats.updateStats();
    }

    public void removeBuff(Buff buff)
    {
        buffs.Remove(buff);
        unitstats.updateStats();

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

    public List<Buff> getBuffs()
    {
        return buffs;
    }

    public GameObject addEffectObject(GameObject prefab, Vector3 position)
    {
        if (isActive())
        {
            return unitController.addEffectObject(prefab, position);
        }
        return null;
    }
    public GameObject addEffectObject(GameObject prefab, Vector3 position, float time)
    {
        if (isActive())
        {
            return unitController.addEffectObject(prefab, position, time);
        }
        return null;
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

    protected void updateAbilities()
    {
        foreach (Ability a in abilities)
        {
            a.update();
        }
    }

    public void setAlive(bool alive)
    {
        this.alive = alive;
        if (!alive)
        {
            onDeath();
            World.tileMap.getTile(tile).removeActor(this);
        }
        setTag();
    }

    public bool isAlive()
    {
        return alive;
    }

    public virtual void onDeath()
    {
        command = null;
    }

    public string getName()
    {
        return name;
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

    public virtual float getSize()
    {
        return 1;
    }

    public virtual int getTeam()
    {
        return 0;
    }

    public virtual int getFavour()
    {
        return 0;
    }

    public virtual int getWeaponTags()
    {
        return int.MaxValue;
    }

    public virtual bool canMove()
    {
        return false;
    }


    public Actor findClosestEnemyActor(int radius)
    {
        float dist = 99999;
        Actor closestActor = null;
        for (int x = tile.x - lineOfSight; x < tile.x + lineOfSight + 1; x++)
        {
            for (int y = tile.y - lineOfSight; y < tile.y + lineOfSight + 1; y++)
            {
                if (!World.getMap().isValidTile(x, y)) continue;
                Tile checkTile = World.getMap().getTile(x, y);
                if (checkTile.containsActors() /*&& Pathfinder.unhinderedTilePath(World.getMap(), get2DPos(), new Vector2(x, y), id)*/)
                {

                    foreach (Actor actor in checkTile.getActors())
                    {
                        if (!actor.isAlive() || actor.getID() == id || actor.getTeam() == getTeam()) continue;

                        float newDist = Vector2.Distance(get2DPos(), actor.get2DPos());
                        if (newDist < dist)
                        {
                            closestActor = actor;
                            dist = newDist;
                        }
                    }
                }
            }
        }
        return closestActor;
    }

    public Unit findClosestEnemyUnit(int radius)
    {
        float dist = 99999;
        Unit closestUnit = null;
        for (int x = tile.x - lineOfSight; x < tile.x + lineOfSight + 1; x++)
        {
            for (int y = tile.y - lineOfSight; y < tile.y + lineOfSight + 1; y++)
            {
                if (!World.getMap().isValidTile(x, y)) continue;
                Tile checkTile = World.getMap().getTile(x, y);
                if (checkTile.containsActors() /*&& Pathfinder.unhinderedTilePath(World.getMap(), get2DPos(), new Vector2(x, y), id)*/)
                {
                    foreach (Actor actor in checkTile.getActors())
                    {
                        Unit unit = actor as Unit;
                        if (unit == null || !actor.isAlive() || actor.getID() == id || actor.getTeam() == getTeam()) continue;

                        float newDist = Vector2.Distance(get2DPos(), actor.get2DPos());
                        if (newDist < dist)
                        {
                            closestUnit = unit;
                            dist = newDist;
                        }
                    }
                }
            }
        }
        return closestUnit;
    }

    public float getHeight()
    {
        if (unitController == null)
            return 0;
        return unitController.getHeight();
    }

    public virtual bool shouldBeRemoved()
    {
        return !isAlive();
    }

    public virtual void onEnterNewTile()
    {
    }

    protected virtual void setTag()
    { 
    }
}
