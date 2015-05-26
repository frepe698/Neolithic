using UnityEngine;
using System.Collections;

public class MovementEffect : DurationEffect {

    //MovementEffectData data;

    private Vector2 velocity;

    public MovementEffect(string name, Actor actor, Vector3 targetPosition, MovementEffectData data)
        : base(name, actor, targetPosition, data.travelTime)
    {
        //this.data = data;

        Vector2 target = new Vector2(targetPosition.x, targetPosition.z);
        //float speed = Vector2.Distance(target, unit.get2DPos()) / data.travelTime;

        velocity = (target - actor.get2DPos()) / data.travelTime;
    }

    protected override void updateEffect(float speedIncrease)
    {
        actor.move(velocity * Time.deltaTime * speedIncrease);
    }

}
