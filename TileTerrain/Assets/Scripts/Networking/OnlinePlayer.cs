using UnityEngine;
using System.Collections;

public class OnlinePlayer {

	private NetworkPlayer networkPlayer;
	private string name;
    private int team;
    private int id;

    private int hero;

	public OnlinePlayer(NetworkPlayer player, string name, int team)
	{
		this.networkPlayer = player;
		this.name = name;
        this.team = team;
        id = Mathf.Max(int.Parse(player.ToString()), 0);

        hero = -1;
	}


	public string getName()
	{
		return name;
	}

    public void setName(string name)
    {
        this.name = name;
    }

    public int getTeam()
    {
        return team;
    }

    public void setTeam(int team)
    {
        this.team = team;
    }

    public NetworkPlayer getNetworkPlayer()
    {
        return networkPlayer;
    }

	public int getID()
	{
        return id;
	}

    public int getHero()
    {
        return hero;
    }

    public void setHero(int hero)
    {
        this.hero = hero;
    }


}
