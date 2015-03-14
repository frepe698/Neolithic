using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkMaster{

    private static Dictionary<int, string> playerName = new Dictionary<int,string>();

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
        if (connection != null)
        {
            if (connection.disconnect())
            {
                connection = null;
            }
        }
        return false;
    }

	public static int getPlayerID()
	{
		return connection != null ? connection.getPlayerID() : 0;
	}

    public static void addPlayer(int id, string name)
    {
        playerName[id] = name;
    }

    public static string getPlayerName(int id)
    {
        return playerName[id];
    }

    public static void changePlayerName(int id, string name)
    {
        playerName[id] = name;
    }

}
