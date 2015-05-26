using UnityEngine;
using System.Collections;

public class StunnedCommand : Command {

    private float duration;
    private GameObject particle;

    public StunnedCommand(Actor actor, float duration)
        : base(actor)
    {
        this.duration = duration;
    }

    public override void start()
    {
        actor.setMoving(false);
        actor.setAnimationRestart(actor.getIdleAnim(), 20);
        GameObject prefab = Resources.Load<GameObject>("Particles/stunned");
        if (prefab != null)
        {
            particle = actor.addEffectObject(prefab, new Vector3(0, 0.0f, 0), duration);
        }
    }

    public override void update()
    {
        duration -= Time.deltaTime;

        if (duration <= 0)
        {
            if (particle != null) GameObject.Destroy(particle);
            setCompleted();
        }
    }

    public override void cancel()
    {
        duration = 0;
    }

    public override bool Equals(object obj)
    {
        StunnedCommand other = obj as StunnedCommand;
        if (other == null) return false;
        return actor == other.actor;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool canAlmostAlwaysStart()
    {
        return true;
    }

    public override bool canBeOverridden()
    {
        return false;
    }

    public override bool canStartOverride(Command command)
    {
        StunnedCommand other = command as StunnedCommand;
        if (other != null) return other.duration >= this.duration;

        return false;
    }

    public override string getName()
    {
        return "stunned";
    }
}
