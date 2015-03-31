using UnityEngine;
using System.Collections;

public class StunnedCommand : Command {

    private float duration;
    private GameObject particle;

    public StunnedCommand(Unit unit, float duration)
        : base(unit)
    {
        this.duration = duration;
    }

    public override void start()
    {
        unit.setMoving(false);
        unit.setAnimationRestart(unit.getIdleAnim(), 20);
        GameObject prefab = Resources.Load<GameObject>("Particles/stunned");
        if (prefab != null) particle = unit.addEffectObject(prefab, new Vector3(0, 0.0f, 0), duration);
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


    public override bool Equals(object obj)
    {
        StunnedCommand other = obj as StunnedCommand;
        if (other == null) return false;
        return unit == other.unit;
    }

    public override bool canAlwaysStart()
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
