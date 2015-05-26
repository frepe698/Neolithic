using UnityEngine;
using System.Collections;

public class RangedAttackCommand : Command {

	private readonly static float BASE_TIME = 1.0f;
    private readonly static float TRIGGER_TIME = 0.375f;

	private bool hasAttacked;
	private float attackTime;
	private Vector3 target;


	public RangedAttackCommand(Unit unit, Vector3 target) :base(unit)
	{
		this.target = target;
	}

	public override void start ()
	{

		actor.setMoving(false);
		attackTime = TRIGGER_TIME / actor.getAttackSpeed();
		hasAttacked = false;
		calculateRotation();
		actor.playWeaponAttackAnimation(actor.getAttackSpeed());
		actor.setAnimationRestart(actor.getAttackAnim(0), actor.getAttackSpeed());
	
	}

	public override void update ()
	{

		if(!hasAttacked)
		{
			attackTime -= Time.deltaTime;
			if(attackTime <= 0) 
			{
				actor.playSound(actor.getAttackSound(0));
				//unit.fireProjectile(target);
				hasAttacked = true;

                actor.setCommandEndTime(Time.time + (BASE_TIME - TRIGGER_TIME)/actor.getAttackSpeed());
                setCompleted();
			}
		}
	}

	private void calculateRotation()
	{
		Vector2 dir = (actor.get2DPos()-new Vector2(target.x, target.z)).normalized;
		actor.setRotation( new Vector3(0, Mathf.Rad2Deg*Mathf.Atan2(dir.x, dir.y), 0) );
		
	}

	public override bool Equals(object o)
	{
		RangedAttackCommand other = o as RangedAttackCommand;
		return (other != null);

	}
	
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

    public override string getName()
    {
        return "ranged";
    }
}
