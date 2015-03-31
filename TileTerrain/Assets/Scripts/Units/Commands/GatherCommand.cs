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

	public GatherCommand(Unit unit, ResourceObject resourceObject) : base(unit)
	{
		this.resourceObject = resourceObject;
		this.destination = resourceObject.get2DPos();
		this.resourcePosition = this.destination;
	}

	public override void start ()
	{
		unit.setPath(destination);
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
						unit.playSound(unit.getAttackSound(resource.getDamageType()));
					}
					unit.gather(resourceObject);
					hasGathered = true;

                    unit.setCommandEndTime(Time.time + (BASE_TIME - TRIGGER_TIME) / unit.getAttackSpeed());
                    setCompleted();
				}
			}
		}
		else if( Vector2.Distance(unit.get2DPos(), destination) < resourceObject.getActionRadius() )
		{

			unit.setMoving(false);
			ResourceObject resObject = World.tileMap.getTile(new Vector2i(resourceObject.get2DPos())).getResourceObject();
			if(resObject != null)
			{
				gathering = true;
				gatherTime = TRIGGER_TIME / unit.getAttackSpeed();
				hasGathered = false;
				calculateRotation();
				unit.setAnimationRestart(unit.getAttackAnim(resObject.getDamageType()), unit.getAttackSpeed());
			}
			else
			{
				setCompleted();
			}



		}
	}

	private void calculateRotation()
	{
		Vector2 dir = (unit.get2DPos()-resourcePosition).normalized;
		unit.setRotation( new Vector3(0, Mathf.Rad2Deg*Mathf.Atan2(dir.x, dir.y), 0) );

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