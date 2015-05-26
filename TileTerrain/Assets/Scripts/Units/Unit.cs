using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : Actor{
	
	//protected float maxHealth;
	//protected float health;
	protected float size;

	
	protected Vector2i destinationTile = new Vector2i();
	protected Vector2 destination;
	protected bool moving;
	protected Path path;

    protected const float BASE_MOVESPEED = 4;
	//protected float movespeed = 4;
	protected float speedMult = 1;
	protected float moveSensitivity = 0.1f;
	
	protected const float LOOT_TIME = 0.25f;

	public Unit(string unit, Vector3 position, Vector3 rotation, int id) 
        : base(unit, position, rotation, id)
	{
	}

    public Unit(string unit, Vector3 position, Vector3 rotation, int id, Vector3 scale)
        : base(unit, position, rotation, id, scale)
    {
    }

	public override void update ()
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
					if(!World.tileMap.isValidTile(newTile) || !World.tileMap.getTile(newTile).isWalkable(id))
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
						World.tileMap.getTile(tile).removeActor(this);
						World.tileMap.getTile(newTile).addActor(this);
						
						
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

        gameObject.transform.position = this.position;
        
        Quaternion target = Quaternion.Euler(this.rotation);
        Quaternion current = gameObject.transform.rotation;
        Quaternion newAngle = Quaternion.RotateTowards(current, target, 1080*Time.deltaTime);
        gameObject.transform.rotation = newAngle;
	}
	
    
	
	public void giveMoveCommand(Vector2 point)
	{
		if(!World.tileMap.isValidTile(new Vector2i(point))) return;
		Command newCommand = new MoveCommand(this, point);
		if(commandEquals(command, newCommand)) return;
		command = newCommand;
        this.lastCommand = command.getName();
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
        this.lastCommand = newCommand.getName();
        command = newCommand;
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
        this.lastCommand = newCommand.getName();
        command.start();
	}


	
	protected override void ground()
	{
		RaycastHit hit;
		if(Physics.Raycast(position + Vector3.up*50, Vector3.down, out hit, Mathf.Infinity , 1 << 8))
		{
			position = new Vector3(position.x, hit.point.y, position.z);
			speedMult = hit.normal.y;
		}
	}
	
	public override bool setPath(Vector2 point)
	{
        float deltaMove = Time.deltaTime * getAdjustedMovespeed();
		
		if(Vector2.Distance(point, get2DPos()) < deltaMove) //distance to target pos is to small
		{
			Vector2 dir = (point-get2DPos()).normalized;
            setRotation( new Vector3(0, Mathf.Rad2Deg*Mathf.Atan2(dir.x, dir.y) + 180, 0) );
			return false;
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
                    command.setDestination(path.getDestination());
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
        return true;
	}
	
	
	
	public override void setMoving(bool moving)
	{
		this.moving = moving;
	}
	
	public void gather(ResourceObject resourceObject)
	{
		GameMaster.getGameController().requestGather(id, new Vector2i(resourceObject.getPosition()));
	}


	/*public void fireProjectile(Vector3 target)
	{
		GameMaster.getGameController().requestFireProjectile(id, target);
	}*/
	
	public void loot(LootableObject lootableObject)
	{
		GameMaster.getGameController().requestLoot(id, new Vector2i(lootableObject.get2DPos().x, lootableObject.get2DPos().y), lootableObject.getID());
	}

   

	public virtual float getLootTime()
	{
		return LOOT_TIME;
	}
	
	public virtual string getRunAnim()
	{
		return null;
	}
	
	public virtual string getLootAnim()
	{
		return null;
	}

	public override void heal(int heal)
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

	public override float getSize()
	{
		return size;
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


    
    public override bool canMove()
    {
        return true;
    }
}
