﻿using UnityEngine;
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

		unit.setMoving(false);
		attackTime = TRIGGER_TIME / unit.getAttackSpeed();
		hasAttacked = false;
		calculateRotation();
		unit.playWeaponAttackAnimation(unit.getAttackSpeed());
		unit.setAnimationRestart(unit.getAttackAnim(0), unit.getAttackSpeed());
	
	}

	public override void update ()
	{

		if(!hasAttacked)
		{
			attackTime -= Time.deltaTime;
			if(attackTime <= 0) 
			{
				unit.playSound(unit.getAttackSound(0));
				unit.fireProjectile(target);
				hasAttacked = true;

                unit.setCommandEndTime(Time.time + (BASE_TIME - TRIGGER_TIME)/unit.getAttackSpeed());
                setCompleted();
			}
		}
	}

	private void calculateRotation()
	{
		Vector2 dir = (unit.get2DPos()-new Vector2(target.x, target.z)).normalized;
		unit.setRotation( new Vector3(0, Mathf.Rad2Deg*Mathf.Atan2(dir.x, dir.y), 0) );
		
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


}
