using UnityEngine;
using System.Collections;

public abstract class GameMode {

    public static readonly string[] teamColorsHex = new string[]
    {
        "858FFFFF",
        "FF763BFF",
    };

    public static readonly Color[] teamColors = new Color[]
    {
       new Color(0.4f, 0.4f, 1),
       new Color(1, 0, 0),
    };

    

    protected GameMaster gameMaster;

    public GameMode(GameMaster gameMaster)
    {
        this.gameMaster = gameMaster;
    }
    public abstract void initWorld();
    public abstract void spawnUnits();
    public abstract void update();
    public abstract void spawnHeroes();
    public abstract void initSpawning();
    public abstract void grantFavour(int team, int favour);
    public abstract void damageBase(int team, int damage);

    public abstract int getFavour(int team);
    public abstract int getBaseCurHealth(int team);
    public abstract int getBaseMaxHealth();

}
