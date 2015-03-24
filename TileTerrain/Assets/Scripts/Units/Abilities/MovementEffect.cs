using UnityEngine;
using System.Collections;

public class MovementEffect : DurationEffect {

    MovementEffectData data;

    private Vector2 velocity;

    public MovementEffect(string name, Unit unit, Vector3 targetPosition, MovementEffectData data)
        : base(name, unit, targetPosition, data.travelTime)
    {
        this.data = data;

        Vector2 target = new Vector2(targetPosition.x, targetPosition.z);
        //float speed = Vector2.Distance(target, unit.get2DPos()) / data.travelTime;

        velocity = (target - unit.get2DPos()) / data.travelTime;
    }

    protected override void updateEffect(float speedIncrease)
    {
        unit.move(velocity * Time.deltaTime * speedIncrease);
    }

}
