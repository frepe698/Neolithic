using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class NetworkConnection {

	protected bool connected = false;

	public abstract void connect();

	public virtual int getPlayerID()
	{
		return int.Parse(Network.player.ToString());
	}

	public bool isConnected()
	{
		return connected;
	}
	
}
