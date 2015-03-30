using UnityEngine;
using System.Collections;

public class OnlineServer : NetworkConnection {

	string gameName;

	public OnlineServer(string gameName)
	{
		this.gameName = gameName;
	}


	public override bool connect()
	{
		Network.InitializeServer(7, 25000, true);
		MasterServer.RegisterHost(NetworkMaster.gameTypeName, gameName);
		Debug.Log ("starting server");
		connected = true;
        return true;
	}

    public override bool disconnect()
    {
        MasterServer.UnregisterHost();
        Network.Disconnect();
        Debug.Log("disconnecting server");
        connected = false;
        return true;
    }

    public override int getPing()
    {
        return Network.GetAveragePing(Network.player);
    }
}
