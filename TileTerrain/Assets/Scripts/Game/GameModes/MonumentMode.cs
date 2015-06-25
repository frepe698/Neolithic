using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonumentMode : GameMode {
    private List<string>[] spawns = new List<string>[] { new List<string>(){"goblin", "goblin", "goblin", "goblin", "shaman","goblin", "goblin", "goblin", "goblin", "shaman"},
            new List<string>(){"goblin", "goblin", "shaman", "goblin", "goblin", "shaman"},
            new List<string>(){"goblin", "goblin", "shaman", "goblin", "goblin", "shaman", "troll"},};
    public Vector2i[] holyplaces;

	public const int TEAM_COUNT = 2;
    public Team[] teams;

    private bool gameOver = false;

    private bool spawning = false;
    private float spawnTimer = 0;
    private const float spawnTime = 2;

    private int day = -1;
    private const int DAYS = 2;

    public MonumentMode(GameMaster gameMaster)
        : base(gameMaster)
    {
        teams = new Team[TEAM_COUNT];
        teams[0] = new Team("Thor's Thunder", 0);
        teams[1] = new Team("The cavemen of kolmården", 1);
    }

    public override void initWorld()
    {
        GameMaster.getWorld().initPvPWorld(this);

        for(int i = 0; i < holyplaces.Length; i++)
        {
            GameMaster.getGUIManager().addMinimapBase(holyplaces[i].toVector2(), Color.grey);
        }
        foreach (Vector2i caveOpening in World.tileMap.cavePos)
        {
            GameMaster.getGUIManager().addMinimapCave(caveOpening.toVector2());
        }
        foreach (Vector2 caveOpening in World.tileMap.theHallEntrance)
        {
            GameMaster.getGUIManager().addMinimapCave(caveOpening);
        }
    }

    public override void spawnUnits()
    {
        GameMaster.getWorld().addPvPSpawners(this);
    }
    public override void update()
    {
        if (!gameOver)
        {
            for (int i = 0; i < TEAM_COUNT; i++)
            {
                Team team = teams[i];
                //team.favour += (-2.0f * Time.deltaTime);
                //GameMaster.getGUIManager().setTeamFavour(i, (int)team.favour);
                if (team.favour <= -100)
                {
                    team.defeated = true;
                    GameMaster.getGUIManager().setGameOver(NetworkMaster.getMe().getTeam() != i);
                    gameOver = true;
                }
            }
        }
        if(spawning)
        {
            if(spawnTimer <= 0)
            {
            }
            spawnTimer -= Time.deltaTime;
            
        }
    }

    public override void initSpawning()
    {
        day = Mathf.Clamp(day + 1, 0, DAYS);
        Debug.Log(day);
        spawning = true;
    }

    public override void damageBase(int team, int damage)
    {
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
                Hero hero = new Hero(GameMaster.heroNames[player.getHero()], new Vector3(teams[i].spawnPosition.x + j * 2, 0, teams[i].spawnPosition.y), new Vector3(0, 0, 0), unitID, teams[i].teamIndex);
                GameMaster.playerToUnitID.Add(player.getID(), unitID);
                GameMaster.addHero(hero);
                hero.inactivate();

                unitID++;
            }
        }        
    }

    public override void grantFavour(int team, int favour)
    {
        getTeam(team).grantFavour(favour);
        GameMaster.getGUIManager().setTeamFavour(team - 2, (int)getTeam(team).favour);
    }
    public int getSummonLevel(int team)
    {
        return teams[team].getSpawningLevel();
    }

    private Team getTeam(int team)
    {
        return teams[team - 2];
    }

    public override int getFavour(int team)
    {
        return (int)teams[team].favour;
    }

    public override int getBaseCurHealth(int team)
    {
        return 1;
    }

    public override int getBaseMaxHealth()
    {
        return 1;
    }

    public class Team
    {
        public float favour;
        public string name;

        public readonly int teamIndex;

        public Vector2 spawnPosition = new Vector2(100, 100);

        //summons
        public Vector2i[] summonPositions;
        public Road[] roads;

        public bool defeated = false;
                                                   
        private int nextUnit = 0;
        private int maxUnit;

        public Team(string name, int index)
        {
            this.name = name;
            favour = 0;

            teamIndex = index + 2;
        }

        public int getSpawningLevel()
        {
            return GameMaster.getAISpawnLevel() + (int)Mathf.Floor(favour / 100);
        }

        public void grantFavour(float favour)
        {
            this.favour += favour;
            Debug.Log("Favour " + this.favour);
        }

        public int getThisTeam()
        {
            return teamIndex - 2;
        }

        public int getEnemyTeam()
        {
            return (teamIndex + 1) % 2;
        }
    }
}
