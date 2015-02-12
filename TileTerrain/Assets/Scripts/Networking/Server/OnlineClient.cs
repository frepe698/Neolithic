using UnityEngine;
using System.Collections;

public class OnlineClient : NetworkConnection {

	private HostData hostData;

	public OnlineClient(HostData hostData)
	{
		this.hostData = hostData;
	}

	public override void connect ()
	{
		Network.Connect(hostData); 
		connected = true;
	}
}
