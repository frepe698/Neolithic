using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLobby : MonoBehaviour {

	NetworkView netView;
	private readonly int LEFT_MOUSE_BUTTON = 0;

	private int[] selectedHeroes = new int[] {-1, -1, -1, -1};
	private Text[] playerNameHeroes = new Text[4];

	private float activateTime;
	private bool buttonsActive = false;
	public GameObject startButton;
	public GameObject leaveButton;

	private bool singleplayer = true;

	// Use this for initialization
	void Start () {
		netView = GetComponent<NetworkView>();
//		for(int i = 0; i < 4; i++)
//		{
//			playerNameHeroes[i] = GameObject.Find ("Hero"+(i+1)).transform.FindChild("playername").GetComponent<Text>();
//			playerNameHeroes[i].text = "Available";
//		}
	}

	public void init(bool singleplayer)
	{
		this.singleplayer = singleplayer;
		Debug.Log ("Enable lobby");
		//NetworkMaster.connect();
		activateTime = Time.time + 1;

	}

	
	// Update is called once per frame
	void Update () {

		if(!buttonsActive && Time.time >= activateTime)
		{
			startButton.SetActive(true);
			leaveButton.SetActive(true);
			buttonsActive = true;
		}

		if(Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
		
		{
			RaycastHit hit;
			if( Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				string tag = hit.collider.tag;
				if(tag == "Unit")
				{
					MenuUnit unit = hit.transform.GetComponent<MenuUnit>();
					if(unit != null)
					{
						if(!singleplayer) netView.RPC ("requestSelectHero", RPCMode.Server, unit.id, int.Parse(Network.player.ToString()));
						else selectHeroSingleplayer(unit.id);
						unit.onSelect();
					}
				}
			}	
		}
	}

	public void buttonStart()
	{
		if(Network.isServer)
		{
			netView.RPC("startGameMultiplayer", RPCMode.All, Random.seed);
		}
		else
		{
			startGame();
		}
	}

	public void buttonLeave()
	{

	}

	public void buttonHero(int index)
	{
		netView.RPC ("requestSelectHero", RPCMode.Server, index, int.Parse(Network.player.ToString()));
	}

	void selectHeroSingleplayer(int index)
	{
		for(int i = 0; i < 4; i++)
		{
			if(i != index)
			{
				selectedHeroes[i] = -1;
			}
			else
			{
				selectedHeroes[i] = 0;
			}
		}
	}

	[RPC]
	void requestSelectHero(int index, int playerID)
	{
		if(selectedHeroes[index] == -1)
		{
			for(int i = 0; i < 4; i++)
			{
				if(selectedHeroes[i] == playerID) netView.RPC("selectHero", RPCMode.AllBuffered, i, -1);
			}
			netView.RPC("selectHero", RPCMode.AllBuffered, index, playerID);
		}
	}

	[RPC]
	void selectHero(int index, int playerID)
	{
		selectedHeroes[index] = playerID;
		/*if(playerID == -1)
		{
			playerNameHeroes[index].text = "Available";
		}
		else
		{
			playerNameHeroes[index].text = "player "+playerID.ToString();
		}*/
	}

	[RPC]
	void startGameMultiplayer(int worldSeed)
	{
		World.seed = worldSeed;
		for(int i = 0; i < 4; i++)
		{
			if(selectedHeroes[i] == -1) selectedHeroes[i] = 100000 + i;
			GameMaster.playerToUnitID.Add(selectedHeroes[i], i);
		}
		Application.LoadLevel("world");
	}

	void startGame()
	{
		World.seed = Random.seed;
		for(int i = 0; i < 4; i++)
		{
			if(selectedHeroes[i] != -1)
				GameMaster.playerToUnitID.Add(selectedHeroes[i], i);
		}
		Application.LoadLevel ("world");
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		if(Network.isServer)
		{
			for(int i = 0; i < 4; i++)
			{
				if(selectedHeroes[i] == int.Parse(player.ToString())) netView.RPC("selectHero", RPCMode.AllBuffered, i, -1);
			}
		}
	}
	
	void OnServerInitialized()
	{
		Debug.Log ("Server Initialized!");

	}
	
	void OnMasterServerEvent(MasterServerEvent e)
	{
		if(e == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log ("Server Registered");
		}
		if(e == MasterServerEvent.RegistrationFailedGameName)
		{
			Debug.LogWarning("Registration failed: Game name");
		}
		if(e == MasterServerEvent.RegistrationFailedGameType)
		{
			Debug.LogWarning("Registration failed: Game type name");
		}
		if(e == MasterServerEvent.RegistrationFailedNoServer)
		{
			Debug.LogWarning("Registration failed: No server");
		}
	}
}
