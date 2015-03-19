using UnityEngine;
using System.Collections;

public class ProjectileEffect : AbilityEffect {

    private ProjectileEffectData data;

    public ProjectileEffect(string name, Unit unit, Vector3 targetPosition) : base(name,unit,targetPosition)
    {
        //Fetch data with name
        this.data = (ProjectileEffectData)DataHolder.Instance.getEffectData(name);
    }

    public override void action()
    {
        int angle = (int)data.angle;
        string name = data.modelName;
        Vector2 target2D = new Vector2(targetPosition.x, targetPosition.z);
        Vector3 finalTarget = targetPosition;
        //If angle != calc new target position
        if(angle != 0)
        {
            float length = Vector2.Distance(targetPosition, unit.get2DPos());
            Vector2 direction = (target2D - unit.get2DPos()).normalized;
            int baseAngle =(int)( Mathf.Rad2Deg*Mathf.Atan2(direction.x, direction.y));
            int totalAngle = baseAngle + angle;

            Vector2 newDirection = new Vector2(Mathf.Cos(totalAngle) * length, Mathf.Sin(totalAngle) * length);
            finalTarget = new Vector3(unit.get2DPos().x + newDirection.x, finalTarget.y, unit.get2DPos().y + newDirection.y);
            
            
        }

        int selfDamage = 0;
        int damage = 0;
        foreach (HitDamage hit in data.hitDamage)
        {
            int tempDamage = 0;
            if (hit.yourStat)
            {
                tempDamage = (int)(unit.getUnitStats().getStatV(hit.stat) * hit.percent);
            }
            
            if (hit.damageSelf) selfDamage += tempDamage;
            else damage += tempDamage;
        }
        GameMaster.getGameController().requestFireProjectile(unit.getID(), finalTarget, damage, data.modelName); //TODO: använd projectile name
    }
}
