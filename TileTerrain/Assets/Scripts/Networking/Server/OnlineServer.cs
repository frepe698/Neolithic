using UnityEngine;
using System.Collections;

public class OnlineServer : NetworkConnection {

	string gameName;

	public OnlineServer(string gameName)
	{
		this.gameName = gameName;
	}


	public override void connect()
	{
		Network.InitializeServer(4, 25000, true);
		MasterServer.RegisterHost(NetworkMaster.gameTypeName, gameName);
		Debug.Log ("starting server");
		connected = true;
	}


}
