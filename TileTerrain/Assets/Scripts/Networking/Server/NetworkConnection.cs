using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class NetworkConnection {

	protected bool connected = false;

    public abstract bool connect();
    public abstract bool disconnect();

	public virtual int getPlayerID()
	{
		return int.Parse(Network.player.ToString());
	}

	public bool isConnected()
	{
		return connected;
	}

    public abstract int getPing();
	
}
