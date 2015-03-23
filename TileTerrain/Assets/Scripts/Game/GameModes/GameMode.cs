﻿using UnityEngine;
using System.Collections;

public abstract class GameMode {

    private GameMaster gameMaster;

    public GameMode(GameMaster gameMaster)
    {
        this.gameMaster = gameMaster;
    }
    public abstract void initWorld();
    public abstract void update();
    public abstract void spawnHeroes();
    public abstract void initSpawning();
    public abstract void grantFavour(int team, int favour);
}
