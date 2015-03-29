using UnityEngine;
using System.Collections;

public class AIUnit : Unit {

	protected float commandTimer;
    protected float justGotCommandTimer;
	private int damage;
	private float attackspeed;
    private string attackSound;
    private int favour;

    private Ability basicAttack;
    
	public AIUnit(string unit, Vector3 position, Vector3 rotation, int id, int level = 0) 
		: base(unit, position, rotation, id)
	{
		AIUnitData data = DataHolder.Instance.getAIUnitData(unit);
		if(data == null) 
		{
			hostile = false;
			//health = 100.0f;
			damage = 0;
			//movespeed = 4;
			lineOfSight = 8;
			size = 0.5f;
            favour = 0;
			Debug.LogError ("The unit data you are looking for does not exist: " + unit);
		}
		else
		{
			hostile = data.hostile;
			//health = (float)data.health;
			damage = data.damage;
			attackspeed = data.attackSpeed;
            attackSound = data.attackSound;
			//movespeed = data.movespeed;
			lineOfSight = data.lineofsight;
			size = data.size;
            modelName = data.modelName;
            favour = data.favouronkill;
		}
        init();
        tile = new Vector2i(get2DPos());
        World.tileMap.getTile(tile).addUnit(this);
        this.unitstats = new UnitStats(this, level, data);
        unitstats.updateStats();
        basicAttack = new Ability(data.basicattack, this);
        if (data.abilities != null)
        {
            foreach (string ability in data.abilities)
            {
                if(!ability.Trim().Equals(""))
                    addAbility(ability);
            }
        }
        
	}

	public override void updateAI()
	{
        justGotCommandTimer -= Time.deltaTime;
        if ((command == null || command is MoveCommand) && justGotCommandTimer < 0)
		{
            Unit closestUnit = findClosestEnemy(lineOfSight);
            if (fleeOrFight(closestUnit)) return;

			commandTimer-=Time.deltaTime;
			if(commandTimer < 0)
			{
				GameMaster.getGameController().requestMoveCommand(id,tile.x+Random.Range(-10.0f, 10.0f), tile.y+Random.Range(-10.0f, 10.0f));
				commandTimer = Random.Range(8, 30);
			}
		}
	}

    //Returns true if it did something
    public bool fleeOrFight(Unit other)
    {
        if (other != null)
        {
            if (hostile)
            {
                int randAb = Random.Range(0, abilities.Count);
                if(abilities.Count > 0 && abilities[randAb].isCool())
                {
                    GameMaster.getGameController().requestAbilityCommandID(getID(), other.getID(), randAb);
                }
                else
                {
                    GameMaster.getGameController().requestAttackCommandUnit(id, other.getID());
                }
                justGotCommandTimer = 0.2f;
                return true;
            }
            else if (other.isHostile() && !hostile)
            {

                Vector2i path = findEscapePath(other.getTile());
                GameMaster.getGameController().requestMoveCommand(id, path.x, path.y);
                justGotCommandTimer = 0.2f;
                return true;
                
            }
        }
        return false;
    }

    protected Vector2i findEscapePath(Vector2i other)
    {
        int dx = tile.x - other.x;
        if (dx != 0) dx = dx / Mathf.Abs(dx);
        int dy = tile.y - other.y;
        if (dy != 0) dy = dy / Mathf.Abs(dy);

        Vector2i path = new Vector2i(tile.x + dx * 5, tile.y + dy * 5);
        if(World.tileMap.isValidTile(path))
        {
            return path;
        }
        path = new Vector2i(tile.x, tile.y + dy * 5);
        if (World.tileMap.isValidTile(path))
        {
            return path;
        }
        path = new Vector2i(tile.x + dx * 5, tile.y);
        if (World.tileMap.isValidTile(path))
        {
            return path;
        }
        path = new Vector2i(tile.x - dx * 5, tile.y - dy * 5);
        if (World.tileMap.isValidTile(path))
        {
            return path;
        }
        return new Vector2i(tile.x + dx * 5, tile.y + dy * 5);
        
    }

    public override Ability getBasicAttack()
    {
        return basicAttack;
    }

	public override string getRunAnim ()
	{
		return "run_unarmed";
	}

	public override string getIdleAnim ()
	{
		return "idle_unarmed";
	}

	public override string getAttackAnim (int type)
	{
		return "chop_unarmed";
	}

	public override int getDamage (int damageType)
	{
		return damage;
	}

    public override int getBaseDamage(int damageType)
    {
        int level = unitstats.getLevel();
        return (int)((level + 1 + level * (level * 0.353f)) * damage);
        //return damage + (int)( damage * (float)(unitstats.getLevel()/ 2) ) ;
    }

    public override float getBaseAttackSpeed()
    {
        return attackspeed;
    }

	public override float getAttackSpeed ()
	{
		return attackspeed;
	}

    public override string getAttackSound(int damageType)
    {
        return attackSound;
    }

	public override void takeDamage(int damage, int dealerID)
	{
        base.takeDamage(damage, dealerID);
		Unit dealer = GameMaster.getUnit(dealerID);
		if(command == null || command is MoveCommand)
		{
            if (dealer == null) return;
			if(hostile) 
			{
				GameMaster.getGameController().requestAttackCommandUnit(id, dealerID);
			}
            else
            {
                Vector2i path = findEscapePath(dealer.getTile());
                GameMaster.getGameController().requestMoveCommand(id, path.x, path.y);
                justGotCommandTimer = 0.2f;
                return;
                
            }
		}
	}

	public override bool isMelee()
	{
		return true;
	}

    public override int getTeam()
    {
        return 1;
    }

    public override int getFavour()
    {
        return favour;
    }

}
