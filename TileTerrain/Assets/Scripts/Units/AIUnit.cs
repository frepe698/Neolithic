using UnityEngine;
using System.Collections;

public class AIUnit : Unit {

	protected float commandTimer;
    protected float justGotCommandTimer;
	private int damage;
	private float attackspeed;
    private string attackSound;

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
			Debug.Log ("The unit data you are looking for does not exist: " + unit);
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
		}
        init();

        this.unitstats = new UnitStats(this, level, data);
        unitstats.updateStats();
        basicAttack = new Ability("aimeleebasicattack", this);
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
                GameMaster.getGameController().requestAttackCommandUnit(id, other.getID());
                justGotCommandTimer = 0.2f;
                return true;
            }
            else if (other.isHostile())
            {
                int dx = tile.x - other.getTile().x;
                if (dx != 0) dx = dx / Mathf.Abs(dx);
                int dy = tile.y - other.getTile().y;
                if (dy != 0) dy = dy / Mathf.Abs(dy);
                if (!hostile)
                {
                    GameMaster.getGameController().requestMoveCommand(id, tile.x + dx * 5, tile.y + dy * 5);
                    justGotCommandTimer = 0.2f;
                    return true;
                }
            }
        }
        return false;
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
			if(hostile && dealer != null) 
			{
				GameMaster.getGameController().requestAttackCommandUnit(id, dealerID);
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

}
