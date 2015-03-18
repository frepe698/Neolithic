using UnityEngine;
using System.Collections;

public class AbilityCommand : Command {

	private Unit target;
	private bool attacking;
	private bool hasAttacked;
	private float attackTime;
	private Vector2 attackPosition;
	private Vector2i targetTile;

    private Ability ability;
    private int lastUsedEffect;
	
	public AbilityCommand(Unit unit, Unit target, Ability ability)
        : base(unit)
	{
		this.target = target;
		this.destination = target.get2DPos();
		this.attackPosition = this.destination;
		this.targetTile = target.getTile();

        this.ability = ability;
	}

    public AbilityCommand(Unit unit, Vector2 attackPosition, Ability ability)
        : base(unit)
    {
        this.target = null;
        this.destination = this.attackPosition = attackPosition;
        this.targetTile = new Vector2i(this.attackPosition);

        this.ability = ability;
    }
	
	public override void start ()
	{
		unit.setPath(destination);
	}
	
	public override void update()
	{
        //Update target position to the targeted units position
        if (target != null)
        {
            attackPosition = target.get2DPos();
            //if it entered a new tile update path
            if (target.getTile() != targetTile)
            {
                destination = target.get2DPos();
                unit.setPath(destination);
            }
        }

		if(attacking)
		{
			if(!hasAttacked)
			{
				attackTime += Time.deltaTime;

                //Get next effect from ability using attacktime
                AbilityEffect effect = null;// = ability.getNextEffect(attacktime);
				while(effect != null) 
				{
                    //TODO: Play effect sound
					//unit.playSound(unit.getAttackSound(0));

                    effect.action();
					//unit.attack(target);

                    lastUsedEffect++;
                    if (lastUsedEffect >= ability.data.effects.Length)
                    {
                        hasAttacked = true;
                        unit.setCommandEndTime(Time.time + (ability.data.totalTime - attackTime) / unit.getAttackSpeed());
                        setCompleted();
                        break;
                    }

                    //get next effect from ability
                    //effect = ability.getNextEffect(attacktime, lastUsedEffect)
				}
               
			}
		}
		else if( Vector2.Distance(unit.get2DPos(), attackPosition) < 2 ) //TODO weapon range here
		{
			unit.setMoving(false);

			attacking = true;
            attackTime = 0;
            lastUsedEffect = 0;
			hasAttacked = false;
			calculateRotation();

            //TODO: animation
			//unit.playWeaponAttackAnimation(unit.getAttackSpeed());
			//unit.setAnimationRestart(unit.getAttackAnim(0), unit.getAttackSpeed());
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

    public Ability getAbility()
    {
        return ability;
    }

	public override bool Equals(object o)
	{
		AbilityCommand other = o as AbilityCommand;
		if(other == null) return false;
		return other.getAbility().data.name.Equals(getAbility().data.name);
	}
	
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
}
