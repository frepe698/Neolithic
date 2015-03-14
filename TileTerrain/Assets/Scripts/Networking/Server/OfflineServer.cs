using UnityEngine;
using System.Collections;

public class OfflineServer : NetworkConnection {

	public override bool connect()
	{
		connected = true;
        return true;
	}

    public override bool disconnect()
    {
        connected = false;
        return true;
    }

	public override int getPlayerID()
	{
		return 0;
	}

    public override int getPing()
    {
        return 0;
    }

    public override bool isOnline()
    {
        return false;
    }
}
