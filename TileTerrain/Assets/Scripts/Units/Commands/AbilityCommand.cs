using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Edit;

public class AbilityCommand : Command {

	private Unit target;
	private bool attacking;
	private bool doneAllEffects;
	private float attackTime;
    private float speedIncrease;
	private Vector2 attackPosition;
    private float attackHeight;
    private Vector2i targetTile;
    

    private Ability ability;
    private int lastUsedEffect;
    private int lastPlayedAnimation;

    private List<DurationEffect> activeDurationEffects;
	
	public AbilityCommand(Unit unit, Unit target, Ability ability)
        : base(unit)
	{
		this.target = target;
        init(target.getPosition(), ability);
	}

    public AbilityCommand(Unit unit, Vector3 attackPosition, Ability ability)
        : base(unit)
    {
        this.target = null;
        init(attackPosition, ability);
    }

    private void init(Vector3 attackPosition, Ability ability)
    {
        this.destination = this.attackPosition = new Vector2(attackPosition.x, attackPosition.z);
        this.attackHeight = attackPosition.y;
        this.targetTile = new Vector2i(this.attackPosition);
        this.ability = ability;

        activeDurationEffects = new List<DurationEffect>();

        speedIncrease = 1;
        foreach (SpeedIncrease s in ability.data.speedIncreases)
        {
            speedIncrease *= 1 + s.percent * (unit.getUnitStats().getStatV(s.stat)-1);
        }
    }
	
	public override void start ()
	{
		unit.setPath(destination);
	}
	
	public override void update()
	{
		if(attacking)
		{
            for (int i = 0; i < activeDurationEffects.Count; i++)
            {
                DurationEffect de = activeDurationEffects[i];
                if (de.done())
                {
                    activeDurationEffects.RemoveAt(i);
                    i--;
                    continue;
                }

                de.update(speedIncrease);
            }

			if(!doneAllEffects)
			{
                attackTime += Time.deltaTime * speedIncrease;

                if (lastPlayedAnimation < ability.data.animations.Length)
                {
                    AbilityAnimation animation = ability.getTimedAnimation(lastPlayedAnimation, attackTime);
                    if (animation != null)
                    {
                        if (animation.weaponAttackAnimation)
                        {
                            float speed = speedIncrease * animation.speed;
                            unit.playWeaponAttackAnimation(speed);
                            unit.setAnimationRestart(unit.getAttackAnim(0), speed);
                        }
                        else
                        {
                            float speed = speedIncrease * animation.speed;
                            unit.setAnimationRestart(animation.name, speed);
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
                        //unit.getUnitStats().getHealth().addCurValue(-ability.getHealthCost());
                        //unit.getUnitStats().getEnergy().addCurValue(-ability.getEnergyCost());
                        GameMaster.getGameController().requestChangeEnergy(unit.getID(), -ability.getEnergyCost());
                        GameMaster.getGameController().requestChangeHealth(unit.getID(), -ability.getHealthCost());
                        unit.setCommandEndTime(Time.time + (ability.data.totalTime / unit.getAttackSpeed() - attackTime));
                    }

                    while (effect != null)
                    {
                        //TODO: get effect sound
                        unit.playSound(unit.getAttackSound(0));
                        effect.action(this);
                        //unit.attack(target);

                        lastUsedEffect++;
                        //if it was the last effect set command as completed
                        if (lastUsedEffect >= ability.data.effects.Length)
                        {
                            doneAllEffects = true;
                            
                            break;
                        }

                        //get next effect from ability
                        effect = getNextEffect();
                    }
                }
			}

            if (doneAllEffects && activeDurationEffects.Count < 1)
            {
                setCompleted();
            }
		}
		else if( Vector2.Distance(unit.get2DPos(), attackPosition) < ability.data.range) //TODO weapon range here
		{
			unit.setMoving(false);

			attacking = true;
            attackTime = 0;
            lastUsedEffect = 0;
			doneAllEffects = false;
			calculateRotation();
		}
        else if(target != null)
        {
            //Update target position to the targeted units position
            attackPosition = target.get2DPos();
            //if it entered a new tile update path
            if (target.getTile() != targetTile)
            {
                if(Vector2i.getDistance(target.getTile(), unit.getTile()) > unit.getLineOfSight())
                {
                    setCompleted();
                    return;
                }
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

    private AbilityEffect getNextEffect()
    {
        AbilityEffectAndTime aeat = ability.getTimedEffect(lastUsedEffect, attackTime);
        if (aeat == null) return null;
        AbilityEffectData data = DataHolder.Instance.getEffectData(aeat.name);
        if (data == null) return null;

        if (target != null) attackHeight = target.getPosition().y + 1;
        return data.getAbilityEffect(unit, new Vector3(attackPosition.x, attackHeight, attackPosition.y));
    }

    public void addDurationEffect(DurationEffect effect)
    {
        activeDurationEffects.Add(effect);
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

    public override bool canBeOverridden()
    {
        return activeDurationEffects.Count < 1;
    }

    public override bool canStartOverride(Command command)
    {
        //Debug.Log("HP " + unit.getUnitStats().getCurHealth() + "EN " + unit.getUnitStats().getCurEnergy());
        return ability.isCool() 
            && unit.getUnitStats().getCurHealth() > ability.data.healthCost 
            && unit.getUnitStats().getCurEnergy() >= ability.data.energyCost
            && (unit.getWeaponTags() & ability.data.tags) > 0
            && !this.Equals(command);
    }
}
