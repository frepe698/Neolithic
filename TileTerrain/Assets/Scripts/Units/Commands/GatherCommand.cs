using UnityEngine;
using System.Collections;

public class GatherCommand : Command {

	private readonly static float BASE_TIME = 1.0f;
    private readonly static float TRIGGER_TIME = 0.583f;

	private ResourceObject resourceObject;
	private bool gathering;
	private bool hasGathered;
	private float gatherTime;
	private Vector2 resourcePosition;

    private Unit unit;

	public GatherCommand(Unit unit, ResourceObject resourceObject) : base(unit)
	{
        this.unit = unit;
		this.resourceObject = resourceObject;
		this.destination = resourceObject.get2DPos();
		this.resourcePosition = this.destination;
	}

	public override void start ()
	{
		actor.setPath(destination);
	}

	public override void update()
	{
		if(gathering)
		{
			if(!hasGathered)
			{
				gatherTime -= Time.deltaTime;
				if(gatherTime <= 0) 
				{
					ResourceObject resource = World.tileMap.getTile(new Vector2i(resourceObject.get2DPos())).getResourceObject();
					if(resource != null)
					{
						actor.playSound(actor.getAttackSound(resource.getDamageType()));
					}
                    unit.gather(resourceObject);
					hasGathered = true;

                    actor.setCommandEndTime(Time.time + (BASE_TIME - TRIGGER_TIME) / actor.getAttackSpeed());
                    setCompleted();
				}
			}
		}
		else if( Vector2.Distance(actor.get2DPos(), destination) < resourceObject.getActionRadius() )
		{
			actor.setMoving(false);
			ResourceObject resObject = World.tileMap.getTile(new Vector2i(resourceObject.get2DPos())).getResourceObject();
			if(resObject != null)
			{
				gathering = true;
				
				hasGathered = false;
				calculateRotation();
                if (resObject.canBeHarvested())
                {
                    gatherTime = unit.getLootTime()*2;
                    actor.setAnimationRestart(unit.getLootAnim(), 0.5f);
                }
                else
                {
                    gatherTime = TRIGGER_TIME / actor.getAttackSpeed();
                    actor.setAnimationRestart(actor.getAttackAnim(resObject.getDamageType()), actor.getAttackSpeed());
                }
			}
			else
			{
				setCompleted();
			}
		}
	}

	private void calculateRotation()
	{
        Vector2 dir = (resourcePosition - actor.get2DPos()).normalized;
		actor.setRotation( new Vector3(0, Mathf.Rad2Deg*Mathf.Atan2(dir.x, dir.y), 0) );

	}

	public ResourceObject getResourceObject()
	{
		return resourceObject;
	}

	public override bool Equals(object o)
	{
		GatherCommand other = o as GatherCommand;
		if(other == null || other.getResourceObject() == null || getResourceObject() == null) return false;
		return other.getResourceObject().Equals(this.getResourceObject());
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

    public override string getName()
    {
        return "gather";
    }
}