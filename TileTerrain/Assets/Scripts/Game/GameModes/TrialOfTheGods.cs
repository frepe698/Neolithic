using UnityEngine;
using System.Collections;

public class TrialOfTheGods : GameMode {
    public const int TEAM_COUNT = 2;
    private Team[] teams;

    public TrialOfTheGods(GameMaster gameMaster)
        : base(gameMaster)
    {
        teams = new Team[TEAM_COUNT];
        teams[0] = new Team("Thor's Thunder", 0);
        teams[1] = new Team("The cavemen of kolmården", 1);
    }

    public override void initWorld()
    {
        throw new System.NotImplementedException();
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

    public int getSummonLevel(int team)
    {
        return teams[team].favour / 100;
    }


    private class Team
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


