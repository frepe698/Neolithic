using UnityEngine;
using System.Collections;

public class NetworkMaster{

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

	public static void connect()
	{
		if(connection != null)
		{
			connection.connect();
		}
	}

	public static int getPlayerID()
	{
		return connection.getPlayerID();
	}
}
