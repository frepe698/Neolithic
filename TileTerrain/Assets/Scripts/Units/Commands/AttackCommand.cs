using UnityEngine;
using System.Collections;

public class AttackCommand : Command {
	
	private readonly static float BASE_TIME = 0.8f;
	
	private Unit target;
	private bool attacking;
	private bool hasAttacked;
	private float attackTime;
	private float animationTime;
	private Vector2 attackPosition;
	private Vector2i targetTile;
	
	public AttackCommand(Unit unit, Unit target) : base(unit)
	{
		this.target = target;
		this.destination = target.get2DPos();
		this.attackPosition = this.destination;
		this.targetTile = target.getTile();
	}
	
	public override void start ()
	{
		unit.setPath(destination);
	}
	
	public override void update()
	{
		if(target == null) return;
		if(attacking)
		{
			if(!hasAttacked)
			{
				attackTime -= Time.deltaTime;
				if(attackTime <= 0) 
				{
					if(target != null)
					{
						unit.playSound(unit.getAttackSound(0));
					}
					unit.attack(target);
					hasAttacked = true;
				}
			}
			else
			{
				animationTime -= Time.deltaTime;
				if(animationTime <= 0)
				{
					setCompleted();
				}
			}
		}
		else if( Vector2.Distance(unit.get2DPos(), target.get2DPos()) < 2 ) //TODO weapon range here
		{
			
			unit.setMoving(false);
			if(target != null)
			{
				attackPosition = target.get2DPos();
				attacking = true;
				attackTime = BASE_TIME / unit.getAttackSpeed();
				hasAttacked = false;
				animationTime = attackTime * 0.1f;
				calculateRotation();
				unit.playWeaponAttackAnimation(unit.getAttackSpeed());
				unit.setAnimationRestart(unit.getAttackAnim(0), unit.getAttackSpeed());
			}
			else
			{
				setCompleted();
			}
			
			
			
		}
		else if(target.getTile() != targetTile)
		{
			if(unit.getLineOfSight() < Vector2.Distance(unit.get2DPos(), target.get2DPos()))
			{
				setCompleted();
			}
			else
			{
				destination = target.get2DPos();
				unit.setPath(destination);
			}
		}
	}
	
	private void calculateRotation()
	{
		Vector2 dir = (unit.get2DPos()-attackPosition).normalized;
		unit.setRotation( new Vector3(0, Mathf.Rad2Deg*Mathf.Atan2(dir.x, dir.y), 0) );
		
	}
	
	public Unit getTarget()
	{
		return target;
	}
	
	public override bool Equals(object o)
	{
		AttackCommand other = o as AttackCommand;
		if(other == null || other.getTarget() == null || getTarget() == null) return false;
		return other.getTarget().Equals(this.getTarget());
	}
	
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
	
	
}