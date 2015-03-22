using UnityEngine;
using System.Collections;

public class TrialOfTheGods : GameMode {
    public const int TEAM_COUNT = 2;
    public Team[] teams;

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
                //game over
            }
        }
    }
    public override void spawnHeroes()
    {

        string[] heroNames = new string[]
		{
			"vrodl",
			"halftroll",
			"halftroll",
			"halftroll"
		};
        //spawn heroes
        for (int i = 0; i < 4; i++)
        {
            //GameObject vrodlGo = (GameObject)Instantiate(vrodl, new Vector3(World.tileMap.basePos.x+i*2, 0, World.tileMap.basePos.y), Quaternion.identity);
            //Hero vrodlU = vrodlGo.GetComponent<Hero>();
            //vrodlU.init(i);
            //Hero hero = new Hero(heroNames[i], new Vector3(World.tileMap.getCaveEntrance(i).x, 0, World.tileMap.getCaveEntrance(i).y), new Vector3(0, 0, 0), i);
            Hero hero = new Hero(heroNames[i], new Vector3(teams[i % 2].basePosition.x + i * 2, 0, teams[i % 2].basePosition.y), new Vector3(0, 0, 0), i, teams[i % 2].teamIndex);
            GameMaster.addHero(hero);
        }


    }

    public int getSummonLevel(int team)
    {
        return teams[team].favour / 100;
    }


    public class Team
    {
        public int favour;
        public string name;

        public readonly int teamIndex;

        //base 
        public Vector2i basePosition;
        public const int BASE_MAX_HEALTH = 30;
        public int baseHealth;

        //summons
        public Vector2i[] summonPositions;
        public Road[] roads;

        public bool defeated = false;

        public Team(string name, int index)
        {
            this.name = name;
            favour = 0;

            teamIndex = index + 2;
            baseHealth = BASE_MAX_HEALTH;
        }
    }
}


