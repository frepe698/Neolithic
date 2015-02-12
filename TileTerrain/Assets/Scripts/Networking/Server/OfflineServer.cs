using UnityEngine;
using System.Collections;

public class OfflineServer : NetworkConnection {

	public override void connect()
	{
		connected = true;
	}

	public override int getPlayerID()
	{
		return 0;
	}
}
