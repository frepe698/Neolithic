using UnityEngine;
using System.Collections;

public class AIUnit : Unit {

	private float commandTimer;
	private int damage;
	private float attackspeed;
    private string attackSound;

	public AIUnit(string unit, Vector3 position, Vector3 rotation, int id) 
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
        unitstats.updateStats();
	}

	public override void updateAI()
	{

		if(command == null)
		{
			for(int x = tile.x - lineOfSight; x < tile.x + lineOfSight + 1; x++)
			{
				for(int y = tile.y - lineOfSight; y < tile.y + lineOfSight + 1; y++)
				{
					if(!World.getMap().isValidTile(x,y)) continue;
					Tile checkTile = World.getMap().getTile(x,y);
					if(checkTile.containsUnits() /*&& Pathfinder.unhinderedTilePath(World.getMap(), get2DPos(), new Vector2(x, y), id)*/)
					{
						foreach(Unit unit in checkTile.getUnits())
						{
                            if (unit.getID() == id || unit.getTeam() == getTeam()) continue;

							if(hostile) 
							{
								GameMaster.getGameController().requestAttackCommand(id, unit.getID());
							}
							else if(unit.isHostile())
							{
								int dx = tile.x - x;
								if(dx != 0) dx = dx / Mathf.Abs(dx);
								int dy = tile.y - y;
								if(dy != 0) dy = dy / Mathf.Abs(dy);
								if(!hostile)
								{

									GameMaster.getGameController().requestMoveCommand(id,tile.x + dx*5, tile.y + dy*5);
									return;
								}
							}
						}
					}
				}
			}
			commandTimer-=Time.deltaTime;
			if(commandTimer < 0)
			{
				GameMaster.getGameController().requestMoveCommand(id,tile.x+Random.Range(-10.0f, 10.0f), tile.y+Random.Range(-10.0f, 10.0f));
				commandTimer = Random.Range(0, 8);
			}
		}
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
        return damage;
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
				GameMaster.getGameController().requestAttackCommand(id, dealerID);
			}
		}
	}

	public override bool isMelee()
	{
		return false;
	}

    public override int getTeam()
    {
        return 1;
    }

}
