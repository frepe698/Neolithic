using UnityEngine;
using System.Collections;

public class Tower : Building {

    private Transform aimPart;

    private float justGotCommandTimer;

    private int damage;
    private float attackSpeed;
    private string attackSound;

    private Ability basicAttack;

    public Tower(string name, Vector2i position, float yRotation, int id, int team)
        : base(name, position, yRotation, id, team)
    {
        
        TowerData data = DataHolder.Instance.getTowerData(name);
        if (data == null)
        {
            Debug.LogError("The Tower " + name + " has no data");
            return;
        }
        modelName = data.modelName;
        damage = data.damage;
        attackSpeed = data.attackspeed;
        attackSound = data.attackSound;
        lineOfSight = data.lineofsight;
        warmth = data.warmth;
        unitstats = new UnitStats(this, 0, data);
        init();
        tile = new Vector2i(get2DPos());
        World.tileMap.getTile(tile).addActor(this);
        unitstats.updateStats();
        basicAttack = new Ability(data.basicattack, this);
    }

    public Tower(TowerData data, Vector2i position, float yRotation, int id, int team)
        : base(data.name, position, yRotation, id, team)
    {
        damage = data.damage;
        attackSpeed = data.attackspeed;
        attackSound = data.attackSound;
        lineOfSight = data.lineofsight;
        warmth = data.warmth;
        modelName = data.modelName;
        unitstats = new UnitStats(this, 0, data);
        init();
        tile = new Vector2i(get2DPos());
        World.tileMap.getTile(tile).addActor(this);
        unitstats.updateStats();
        
        basicAttack = new Ability(data.basicattack, this);
    }

    public override void activate()
    {
        if (isActive()) return; // already active
        this.gameObject = ObjectPoolingManager.Instance.GetObject(modelName);
        if (gameObject == null) return; //Object pool was full and no more objects are available

        ground();
        gameObject.transform.position = position;
        gameObject.transform.eulerAngles = rotation;
        //unit.transform.localScale = scale;
        this.unitController = gameObject.GetComponent<UnitController>();
        unitController.setID(id);
        aimPart = gameObject.transform.FindChild("aim");

    }

    public override bool inactivate()
    {
        if (!isActive()) return false; //Already inactive
        ObjectPoolingManager.Instance.ReturnObject(modelName, gameObject);
        gameObject = null;
        aimPart = null;
        return true;
    }

    public override void update()
    {
        if (!awake) return;
        if (command != null)
        {
            if (command.isCompleted()) command = null;
            else command.update();
        }
        else if(Time.time >= commandEndTime)
        {
            setAnimation(getIdleAnim());
        }

        //update rotation
        if (aimPart != null)
        {
            Quaternion target = Quaternion.Euler(this.rotation);
            Quaternion current = aimPart.rotation;
            Quaternion newAngle = Quaternion.RotateTowards(current, target, 1080 * Time.deltaTime);
            aimPart.rotation = newAngle;
        }

        updateAbilities();
        updateBuffs();
    }

    public override void updateAI()
    {
        justGotCommandTimer -= Time.deltaTime;
        if (command == null && justGotCommandTimer <= 0)
        {
            Unit closest = findClosestEnemyUnit(10);
            if (closest != null)
            {
                GameMaster.getGameController().requestAttackCommandUnit(id, closest.getID());
                justGotCommandTimer = 0.2f;
            }
        }
    }

    public override Ability getBasicAttack()
    {
        return basicAttack;
    }

    public override string getIdleAnim()
    {
        return "idle_unarmed";
    }

    public override string getAttackAnim(int type)
    {
        return "chop_unarmed";
    }

    public override int getDamage(int damageType)
    {
        return damage;
    }

    public override int getBaseDamage(int damageType)
    {
        return damage;
    }

    public override float getBaseAttackSpeed()
    {
        return attackSpeed;
    }

    public override float getAttackSpeed()
    {
        return attackSpeed;
    }

    public override string getAttackSound(int damageType)
    {
        return attackSound;
    }

    public override bool isMelee()
    {
        return true;
    }
}
