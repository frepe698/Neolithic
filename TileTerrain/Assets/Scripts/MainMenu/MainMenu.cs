using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour {

	private GameLobby gameLobby;

	private HostData[] serverList;

	private Transform serverListHolder;
	public Text serverListText;
	// Use this for initialization
	void Awake () {

		//serverListHolder = GameObject.Find ("Server list").transform;
		serverList = new HostData[0];

		gameLobby = GetComponent<GameLobby>();
		gameLobby.enabled = false;
		//if(gameLobby == null) Debug.Log("NUll");

	}




	public void buttonSinglePlayer()
	{
		NetworkMaster.initializeOfflineServer();
		NetworkMaster.connect();
		goToLobby(true);
		//Application.LoadLevel("world");

	}

	public void buttonHostServer()
	{
		Debug.Log ("host server");
		//gameLobby.SetActive(true);
		NetworkMaster.initializeOnlineServer("my game");
		NetworkMaster.connect();
		//gameObject.SetActive(false);
	}

	public void buttonExitGame()
	{
		Application.Quit();
	}

	public void buttonJoin()
	{
		if(serverList.Length > 0)
		{
			NetworkMaster.initializeOnlineClient(serverList[0]);
			NetworkMaster.connect();
		}
	}

	public void buttonFindServers()
	{
		MasterServer.RequestHostList(NetworkMaster.gameTypeName);
		updateServerList();
	}

	private void updateServerList()
	{

//		while(serverListHolder.childCount > serverList.Length)
//		{
//			Destroy(serverListHolder.GetChild(0).gameObject);
//		}
//		while(serverListHolder.childCount < serverList.Length)
//		{
//			GameObject go = new GameObject("serverInfo");
//			go.transform.SetParent(serverListHolder);
//			Text t = go.AddComponent<Text>();
//			t.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
//			go.transform.localPosition = new Vector3(-50, -80 + -20 * serverListHolder.childCount, 0);
//
//		}
//		for(int i = 0; i < serverListHolder.childCount; i++)
//		{
//			serverListHolder.GetChild(i).GetComponent<Text>().text = serverList[i].gameName + " - " + serverList[i].connectedPlayers;
//		}


		string newServerListText = "";

		foreach(HostData data in serverList)
		{
			newServerListText+=data.gameName + "\n";
		}

		serverListText.text = newServerListText;

	}

	private void goToLobby(bool singleplayer)
	{
		gameLobby.enabled = true;
		gameLobby.init (singleplayer);
		GetComponent<Animator>().SetTrigger("gotoLobby");
		this.enabled = false;
	}

	void OnConnectedToServer()
	{
		goToLobby(false);
	}

	void OnMasterServerEvent(MasterServerEvent e)
	{
		if(e == MasterServerEvent.HostListReceived)
		{
			serverList = MasterServer.PollHostList();
			updateServerList();
			Debug.Log (serverList.Length);
		}
		if(e == MasterServerEvent.RegistrationSucceeded)
		{
			goToLobby(false);
		}
	}
}
