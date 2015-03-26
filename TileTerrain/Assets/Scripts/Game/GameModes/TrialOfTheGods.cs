using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrialOfTheGods : GameMode {
    public const int TEAM_COUNT = 2;
    public Team[] teams;


    private bool spawning = false;
    private float spawnTimer = 0;
    private const float spawnTime = 3;

    public TrialOfTheGods(GameMaster gameMaster)
        : base(gameMaster)
    {
        teams = new Team[TEAM_COUNT];
        teams[0] = new Team("Thor's Thunder", 0);
        teams[1] = new Team("The cavemen of kolmården", 1);
    }

    public override void initWorld()
    {
        GameMaster.getWorld().initPvPWorld(this);
    }

    public override void update()
    {
        for (int i = 0; i < TEAM_COUNT; i++)
        {
            Team team = teams[i];
            if (team.baseHealth <= 0)
            {
                team.defeated = true;
                Debug.Log("Team " + i + " has lost!");
                //game over
            }
        }
        if(spawning)
        {
            if(spawnTimer <= 0)
            {
                for (int i = 0; i < teams.Length; i++)
                {
                    spawning = teams[i].spawnNextUnit();
                }
                spawnTimer = spawnTime;
            }
            spawnTimer -= Time.deltaTime;
            
        }
    }

    public override void initSpawning()
    {
        spawning = true;
    }

    public override void damageBase(int team, int damage)
    {
        teams[team].damageBase(damage);
    }
    public override void spawnHeroes()
    {
        int unitID = 0;
        for (int i = 0; i < TEAM_COUNT; i++)
        {
            List<OnlinePlayer> players = NetworkMaster.getTeamPlayers(i);
            for (int j = 0; j < players.Count; j++)
            {
                OnlinePlayer player = players[j];
                Hero hero = new Hero(GameMaster.heroNames[player.getHero()], new Vector3(teams[i].basePosition.x + j * 2, 0, teams[i].basePosition.y), new Vector3(0, 0, 0), unitID, teams[i].teamIndex);
                GameMaster.playerToUnitID.Add(player.getID(), unitID);
                GameMaster.addHero(hero);

                unitID++;
            }
        }
        return;
    }

    public override void grantFavour(int team, int favour)
    {
        teams[team].grantFavour(favour);
    }
    public int getSummonLevel(int team)
    {
        return teams[team].getSpawningLevel();
    }


    public class Team
    {
        public int favour;
        public string name;

        public readonly int teamIndex;

        //base 
        public Vector2i basePosition;
        public const int BASE_MAX_HEALTH = 500;
        public int baseHealth;

        //summons
        public Vector2i[] summonPositions;
        public Road[] roads;

        public bool defeated = false;

        private string[] spawns = new string[] { "goblin", "goblin", "goblin", "goblin", "wolf", "wolf", "troll" };
        private int nextUnit = 0;
        private int maxUnit;

        public Team(string name, int index)
        {
            this.name = name;
            favour = 0;

            teamIndex = index + 2;
            baseHealth = BASE_MAX_HEALTH;
            maxUnit = spawns.Length;
        }

        public bool spawnNextUnit()
        {
            for(int i = 0; i < summonPositions.Length; i++)
            {
                Vector2 spawnPos = summonPositions[i].toVector2();
                int unitID = GameMaster.getNextUnitID();
                AIUnit unit = new PathAIUnit(spawns[nextUnit], new Vector3(spawnPos.x + 0.5f, 0, spawnPos.y + 0.5f), Vector3.zero, unitID, roads[i].getWaypoints(), getSpawningLevel(), getEnemyTeam());
                GameMaster.addAwakeUnit(unit);
            }
            nextUnit++;
            if(nextUnit == maxUnit)
            {
                nextUnit = 0;
                return false;
            }
            return true;
            
        }

        public int getSpawningLevel()
        {
            return GameMaster.getAISpawnLevel() + (int)Mathf.Floor(favour / 100);
        }

        public void grantFavour(int favour)
        {
            this.favour += favour;
        }
        public void damageBase(int damage)
        {
            baseHealth -= damage;
           // Debug.Log("Base " + teamIndex + " took " + damage + " damage!");
        }

        public int getEnemyTeam()
        {
            return (teamIndex + 1) % 2;
        }
    }
}


