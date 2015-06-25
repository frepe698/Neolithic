using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Edit;

public class AbilityCommand : Command {

	private Actor target;
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
	
	public AbilityCommand(Actor actor, Actor target, Ability ability)
        : base(actor)
	{
		this.target = target;
        init(target.getPosition(), ability);
	}

    public AbilityCommand(Actor actor, Vector3 attackPosition, Ability ability)
        : base(actor)
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
            //Debug.Log(s.stat + " " + s.percent + ", " + actor.getUnitStats().getStatV(s.stat));
            speedIncrease *= 1 + s.percent * (actor.getUnitStats().getStatV(s.stat)-1);
        }
    }
	
	public override void start ()
	{
        if (ability.data.totalTime <= float.Epsilon) //if total time is 0 do all effects and dont override command
        {
            foreach (AbilityEffectAndTime effect in ability.data.effects)
            {
                DataHolder.Instance.getEffectData(effect.name).getAbilityEffect(actor, attackPosition).action(this);
            }
            ability.setCooldown();
            GameMaster.getGameController().requestChangeEnergy(actor.getID(), -ability.getEnergyCost());
            GameMaster.getGameController().requestChangeHealth(actor.getID(), -ability.getHealthCost());
            actor.setCommandEndTime(Time.time + (ability.data.totalTime / actor.getAttackSpeed() - attackTime));
        }
        else
        {
            actor.setPath(destination);
        }
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
                //Aimbot if you have a target == op
                if (target != null && ability.data.canRetarget)
                {
                    //Update target position to the targeted units position
                    Vector2 heroPos = actor.get2DPos();
                    Vector2 groundPos2D = target.get2DPos();

                    if (Vector2.Distance(heroPos, groundPos2D) > ability.data.range)
                    {
                        Vector2 dir = (groundPos2D - heroPos).normalized;
                        attackPosition = new Vector2(heroPos.x + dir.x * ability.data.range, heroPos.y + dir.y * ability.data.range);
                    }
                    else
                    {
                        attackPosition = target.get2DPos();
                    }
                    calculateRotation();
                }


                attackTime += Time.deltaTime * speedIncrease;

                if (lastPlayedAnimation < ability.data.animations.Length)
                {
                    AbilityAnimation animation = ability.getTimedAnimation(lastPlayedAnimation, attackTime);
                    if (animation != null)
                    {
                        if (animation.weaponAttackAnimation)
                        {
                            float speed = speedIncrease * animation.speed;
                            actor.playWeaponAttackAnimation(speed);
                            actor.setAnimationRestart(actor.getAttackAnim(0), speed);
                        }
                        else
                        {
                            float speed = speedIncrease * animation.speed;
                            actor.setAnimationRestart(animation.name, speed);
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
                        GameMaster.getGameController().requestChangeEnergy(actor.getID(), -ability.getEnergyCost());
                        GameMaster.getGameController().requestChangeHealth(actor.getID(), -ability.getHealthCost());
                        actor.setCommandEndTime(Time.time + (ability.data.totalTime / actor.getAttackSpeed() - attackTime));
                    }

                    while (effect != null)
                    {
                        //TODO: get effect sound
                        //unit.playSound(unit.getAttackSound(0));
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
		else if( Vector2.Distance(actor.get2DPos(), attackPosition) < ability.data.range)
		{
			actor.setMoving(false);

			attacking = true;
            attackTime = 0;
            lastUsedEffect = 0;
			doneAllEffects = false;
			calculateRotation();
		}
        else if (!actor.canMove())
        {
            setCompleted();
        }
        else if (target != null)
        {
            //Update target position to the targeted units position
            attackPosition = target.get2DPos();
            //if it entered a new tile update path
            if (target.getTile() != targetTile)
            {
                if (Vector2i.getDistance(target.getTile(), actor.getTile()) > actor.getLineOfSight())
                {
                    setCompleted();
                    return;
                }
                destination = target.get2DPos();
                actor.setPath(destination);
            }
        }
	}
	
	private void calculateRotation()
	{
        Vector2 dir = (attackPosition - actor.get2DPos()).normalized;
		actor.setRotation( new Vector3(0, Mathf.Rad2Deg*Mathf.Atan2(dir.x, dir.y), 0) );
	}
	
	public Actor getTarget()
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
        return data.getAbilityEffect(actor, new Vector3(attackPosition.x, attackHeight, attackPosition.y));
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
            && actor.getUnitStats().getCurHealth() > ability.data.healthCost 
            && actor.getUnitStats().getCurEnergy() >= ability.data.energyCost
            && (actor.getWeaponTags() & ability.data.tags) > 0
            && !this.Equals(command)
            && ((command == null || command.canBeOverridden()) || canAlwaysStart());
    }

    public override bool canAlwaysStart()
    {
        return ability.data.canAlwaysStart;
    }

    public override string getName()
    {
        return ability.data.name;
    }
}
