using UnityEngine;
using System.Collections;
using Edit;

public class AbilityCommand : Command {

	private Unit target;
	private bool attacking;
	private bool hasAttacked;
	private float attackTime;
	private Vector2 attackPosition;
    private float attackHeight;
    private Vector2i targetTile;
    

    private Ability ability;
    private int lastUsedEffect;
    private int lastPlayedAnimation;
	
	public AbilityCommand(Unit unit, Unit target, Ability ability)
        : base(unit)
	{
		this.target = target;
		this.destination = target.get2DPos();
		this.attackPosition = this.destination;
		this.targetTile = target.getTile();

        this.ability = ability;
	}

    public AbilityCommand(Unit unit, Vector3 attackPosition, Ability ability)
        : base(unit)
    {
        this.target = null;
        this.destination = this.attackPosition = new Vector2(attackPosition.x, attackPosition.z);
        this.attackHeight = attackPosition.y;
        this.targetTile = new Vector2i(this.attackPosition);

        this.ability = ability;
    }
	
	public override void start ()
	{
		unit.setPath(destination);
	}
	
	public override void update()
	{
        

		if(attacking)
		{
			if(!hasAttacked)
			{
                attackTime += Time.deltaTime * unit.getAttackSpeed();

                if (lastPlayedAnimation < ability.data.animations.Length)
                {
                    AbilityAnimation animation = ability.getTimedAnimation(lastPlayedAnimation, attackTime);
                    if (animation != null)
                    {
                        if (animation.name.Equals("attack"))
                        {
                            float speed = unit.getAttackSpeed() * animation.speed;
                            unit.playWeaponAttackAnimation(speed);
                            unit.setAnimationRestart(unit.getAttackAnim(0), speed);
                        }
                        lastPlayedAnimation++;
                    }
                }

                //Get next effect from ability using attacktime
                AbilityEffect effect = getNextEffect();
                if (effect != null)
                {
                    //First effect
                    //TODO: Move this to network
                    if (lastUsedEffect == 0)
                    {
                        ability.setCooldown();
                        unit.getUnitStats().getHealth().addCurValue(-ability.getHealthCost());
                        unit.getUnitStats().getEnergy().addCurValue(-ability.getEnergyCost());

                        unit.setCommandEndTime(Time.time + (ability.data.totalTime / unit.getAttackSpeed() - attackTime));
                    }

                    while (effect != null)
                    {
                        //TODO: get effect sound
                        unit.playSound(unit.getAttackSound(0));

                        effect.action();
                        //unit.attack(target);

                        lastUsedEffect++;
                        //if it was the last effect set command as completed
                        if (lastUsedEffect >= ability.data.effects.Length)
                        {
                            hasAttacked = true;
                            
                            setCompleted();
                            break;
                        }

                        //get next effect from ability
                        effect = getNextEffect();
                    }
                }
			}
		}
		else if( Vector2.Distance(unit.get2DPos(), attackPosition) < 2 ) //TODO weapon range here
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
			unit.setMoving(false);

			attacking = true;
            attackTime = 0;
            lastUsedEffect = 0;
			hasAttacked = false;
			calculateRotation();
			
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

    private AbilityEffect getNextEffect()
    {
        AbilityEffectAndTime aeat = ability.getTimedEffect(lastUsedEffect, attackTime);
        if (aeat == null) return null;

        AbilityEffectData data = DataHolder.Instance.getEffectData(aeat.name);
        if (data == null) return null;

        return data.getAbilityEffect(unit, new Vector3(attackPosition.x, attackHeight, attackPosition.y));
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

    public override bool canStartOverride(Command command)
    {
        return ability.isCool() && unit.getUnitStats().getCurHealth() > ability.data.healthCost &&
            unit.getUnitStats().getCurEnergy() >= ability.data.energyCost && !this.Equals(command);
    }
}
