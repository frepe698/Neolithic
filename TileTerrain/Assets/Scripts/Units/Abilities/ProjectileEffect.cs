using UnityEngine;
using System.Collections;

public class ProjectileEffect : AbilityEffect {

    private ProjectileEffectData data;

    public ProjectileEffect(string name, Unit unit, Vector3 targetPosition, ProjectileEffectData data) : base(name,unit,targetPosition)
    {
        //Fetch data with name
        this.data = data;
    }

    public override void action()
    {
        
        int angle = (int)data.angle;
        string name = data.modelName;
        
        Vector3 finalTarget = targetPosition;
        //If angle != calc new target position
        if(true)//angle != 0)
        {
            Vector2 target2D = new Vector2(targetPosition.x, targetPosition.z);
            float length = Vector2.Distance(target2D, unit.get2DPos());
            Vector2 direction = (target2D - unit.get2DPos()).normalized;
            int baseAngle =(int)( Mathf.Rad2Deg*Mathf.Atan2(direction.x, -direction.y));
            //if (direction.y > 0) baseAngle += 180;
            int totalAngle = baseAngle + angle - 90;// + 360) % 360;

            Vector2 newDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad*totalAngle) * length, Mathf.Sin(Mathf.Deg2Rad*totalAngle) * length);
            finalTarget = new Vector3(unit.get2DPos().x + newDirection.x, finalTarget.y, unit.get2DPos().y + newDirection.y);
            
            
        }

        /*int selfDamage = 0;
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
        */
        string projectileName;
        if (data.weaponProjectile)
            projectileName = unit.getProjectileName();
        else
            projectileName = data.projectileName;

        Debug.Log("fire projectile " + data.name);
        GameMaster.getGameController().requestFireProjectile(unit.getID(), finalTarget, data.name, projectileName); //TODO: använd projectile name
    }
}
