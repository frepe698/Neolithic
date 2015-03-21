using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameLobby : MonoBehaviour {

	NetworkView netView;
    private GlobalMenu globalMenu;
	private readonly int LEFT_MOUSE_BUTTON = 0;

	private int[] selectedHeroes = new int[] {-1, -1, -1, -1};
    private List<int>[] playerTeam = new List<int>[] {new List<int>(), new List<int>()};
	private Text[] playerNameHeroes = new Text[4];

    private Transform[] teamHolders;
    private List<Text>[] teamPlayerNames = new List<Text>[] {new List<Text>(), new List<Text>()}; 

	private float activateTime;
	private bool buttonsActive = false;
    private GameObject uiObject;

	private bool singleplayer = true;

    private bool initialized = false;

	// Use this for initialization
	void Awake () {
        globalMenu = GetComponent<GlobalMenu>();
		netView = GetComponent<NetworkView>();

        uiObject = GameObject.Find("Game Lobby");
		for(int i = 0; i < 4; i++)
		{
			playerNameHeroes[i] = uiObject.transform.FindChild ("Hero"+(i)).GetComponent<Text>();
			playerNameHeroes[i].text = "Available";
		}

        Transform teams = uiObject.transform.FindChild("Teams");
        teamHolders = new Transform[2];
        teamHolders[0] = teams.FindChild("Team0");
        teamHolders[1] = teams.FindChild("Team1");


        uiObject.SetActive(false);
	}

	public void init(bool singleplayer)
	{
        if (initialized) return;

        if (GlobalMenu.playerName == null || GlobalMenu.playerName.Trim().Equals(""))
        {
            GlobalMenu.randomizePlayerName();
        }

		this.singleplayer = singleplayer;
        if (NetworkMaster.isOnline())
        {
            //NetworkMaster.addPlayer(NetworkMaster.getPlayerID(), GlobalMenu.playerName);
            netView.RPC("onPlayerConnected", RPCMode.All, Network.player, NetworkMaster.getPlayerID(), GlobalMenu.playerName);
        }
		//NetworkMaster.connect();
		activateTime = Time.time + 1;
        initialized = true;
	}

    public void onLeave()
    {
        uiObject.SetActive(false);
        buttonsActive = false;
        initialized = false;
    }

	
	// Update is called once per frame
	void Update () {

		if(!buttonsActive && Time.time >= activateTime)
		{
            uiObject.SetActive(true);
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
		else if(singleplayer)
		{
			startGame();
		}
	}

	public void buttonLeave()
	{
        NetworkMaster.disconnect();
        globalMenu.goToMenu();
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
                playerNameHeroes[i].text = "<color=white>Available</color>";
			}
			else
			{
				selectedHeroes[i] = 0;
                playerNameHeroes[i].text = "<color=#fcfc11ff>Selected</color>";
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
		if(playerID == -1)
		{
			playerNameHeroes[index].text = "<color=white>Available</color>";
		}
		else
		{
            string colorcode = "<color=#1111d6ff>";
            if (playerID == NetworkMaster.getPlayerID())
            {
                colorcode = "<color=#f6f622ff>";
            }
            playerNameHeroes[index].text = colorcode + NetworkMaster.getPlayerName(int.Parse(playerID.ToString())) + "</color>";
		}
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
        globalMenu.addChatMessage(NetworkMaster.getPlayerName(int.Parse(player.ToString())) + " disconnected."); 
	}

    [RPC]
    void onPlayerConnected(NetworkPlayer player, int playerID, string name)
    {
        globalMenu.addChatMessage(name + " has connected.");
        addPlayer(playerID, name);
        if (Network.isServer && player != Network.player)
        {
            {
                int id = NetworkMaster.getPlayerID();
                netView.RPC("addPlayer", player, id, NetworkMaster.getPlayerName(id));
            }
            foreach (NetworkPlayer np in Network.connections)
            {

                if(np != player)
                {
                    int id = int.Parse(np.ToString());
                    netView.RPC("addPlayer", player, id, NetworkMaster.getPlayerName(id));
                }
            }
        }
    }

    [RPC]
    void addPlayer(int playerID, string name)
    {
        NetworkMaster.addPlayer(playerID, name);
        playerJoinTeam(playerID, 0);
    }

    void playerJoinTeam(int playerID, int team)
    {
        for (int i = 0; i < playerTeam.Length; i++)
        {
            foreach (int pid in playerTeam[0])
            {
                if (pid == playerID)
                {
                    playerTeam[i].Remove(pid);
                }
            }
        }
        playerTeam[team].Add(playerID);

        updateTeamDisplay();
    }

    void updateTeamDisplay()
    {
        for (int i = 0; i < playerTeam.Length; i++)
        {
            List<Text> teamNames = teamPlayerNames[i];
            while (teamNames.Count < playerTeam[i].Count)
            {
                GameObject prefab = Resources.Load<GameObject>("GUI/LobbyPlayerText");
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(teamHolders[i]);

                Text text = go.GetComponent<Text>();
                text.rectTransform.anchoredPosition = new Vector2(0, (teamNames.Count + 1) * -25);
                text.rectTransform.localScale = new Vector3(1, 1, 1);

                text.text = NetworkMaster.getPlayerName(playerTeam[i][teamNames.Count]);

                teamNames.Add(text);
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
