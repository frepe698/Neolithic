using UnityEngine;
using System.Collections;

public class OnlinePlayer {

	private NetworkPlayer player;
	private string name;

	public OnlinePlayer(NetworkPlayer player, string name)
	{
		this.player = player;
		this.name = name;
	}

	public string getName()
	{
		return name;
	}

	public int getID()
	{
		return int.Parse(player.ToString());
	}


}
