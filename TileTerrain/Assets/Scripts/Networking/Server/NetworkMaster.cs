using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkMaster{

    private static OnlinePlayer me;
    private static List<OnlinePlayer> players = new List<OnlinePlayer>();

	public static readonly string gameTypeName = "StoneAgeTowerDefense";

	private static NetworkConnection connection;

	public static bool initializeOfflineServer()
	{
		connection = new OfflineServer();
		//connection.connect();
		return true;
	}

	public static bool initializeOnlineServer(string gameName)
	{
		connection = new OnlineServer(gameName);
		//connection.connect();
		return true;
	}

	public static bool initializeOnlineClient(HostData hostData)
	{
		connection = new OnlineClient(hostData);
		//connection.connect();
		return true;
	}

	public static bool isConnected()
	{
		return connection != null && connection.isConnected();
	}

    public static bool isOnline()
    {
        return connection != null && connection.isOnline();
    }

    public static bool isServer()
    {
        return connection != null && connection.isServer();
    }

    public static int getAveragePing()
    {
        if (connection != null)
            return connection.getPing();

        return 0;
    }

	public static bool connect()
	{
		if(connection != null)
		{
			return connection.connect();
		}
        return false;
	}

    public static bool disconnect()
    {
        players = new List<OnlinePlayer>();
        me = null;
        if (connection != null)
        {
            if (connection.disconnect())
            {
                connection = null;
                return true;
            }
        }
        return false;
    }

	public static int getMyPlayerID()
	{
		return connection != null ? connection.getPlayerID() : 0;
	}

    public static NetworkPlayer getMyPlayer()
    {
        return Network.player;
    }

    public static void addPlayer(NetworkPlayer player, string name, int team, int hero)
    {
        OnlinePlayer p = new OnlinePlayer(player, name, team, hero);
        players.Add(p);

        if (player == getMyPlayer())
            me = p;
    }

    public static void addPlayer(OnlinePlayer player)
    {
        players.Add(player);

        if (player.getNetworkPlayer() == getMyPlayer())
            me = player; 
    }

    public static bool removePlayer(NetworkPlayer player)
    {
        OnlinePlayer op = findPlayer(player);
        if (op != null)
        {
            return players.Remove(op);
        }
        return false;
    }

    public static string getPlayerName(int id)
    {
        OnlinePlayer player = findPlayer(id);
        if (player == null) return null;
        return player.getName();
    }

    public static void changePlayerName(int id, string name)
    {
        OnlinePlayer player = findPlayer(id);
        if (player != null) player.setName(name);
    }

    public static OnlinePlayer findPlayer(int id)
    {
        foreach (OnlinePlayer player in players)
        {
            if (player.getID() == id) return player;
        }
        return null;
    }

    public static OnlinePlayer findPlayer(NetworkPlayer netPlayer)
    {
        foreach (OnlinePlayer player in players)
        {
            if (player.getNetworkPlayer() == netPlayer) return player;
        }
        return null;
    }

    public static List<OnlinePlayer> getAllPlayers()
    {
        return players;
    }

    public static List<OnlinePlayer> getTeamPlayers(int team)
    {
        List<OnlinePlayer> teamPlayers = new List<OnlinePlayer>();
        foreach (OnlinePlayer player in players)
        {
            if (player.getTeam() == team) teamPlayers.Add(player);
        }
        return teamPlayers;
    }

    public static OnlinePlayer getMe()
    {
        return me;
    }

}
