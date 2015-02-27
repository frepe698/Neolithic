using UnityEngine;
using System.Collections;

public class OnlineClient : NetworkConnection {

	private HostData hostData;

	public OnlineClient(HostData hostData)
	{
		this.hostData = hostData;
	}

	public override bool connect ()
	{
		Network.Connect(hostData); 
		connected = true;
        return true;
	}

    public override bool disconnect()
    {
        Network.Disconnect();
        connected = false;
        return true;
    }

    public override int getPing()
    {
        return Network.GetAveragePing(Network.player);
    }
}
