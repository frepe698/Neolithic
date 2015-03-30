using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {

    private GlobalMenu globalMenu;

	private HostData[] serverList;
    private List<Button> serverListButtons = new List<Button>();

	public RectTransform serverListHolder;
	private Text serverListStatus;

    private int selectedServer = -1;

    private int lastClickedServer = -1;
    private float clickTime = 0;
    private readonly static float DOUBLECLICK_TIME = 0.5f;

    private bool searching = false;
	// Use this for initialization
	void Awake () 
    {

		//serverListHolder = GameObject.Find ("Server list").transform;
        globalMenu = GetComponent<GlobalMenu>();
		serverList = new HostData[0];


		//if(gameLobby == null) Debug.Log("NUll");
	}

    void Start()
    {
        serverListStatus = serverListHolder.FindChild("Status").GetComponent<Text>();
        serverListStatus.gameObject.SetActive(false);
    }

    void Update()
    {
        if (searching)
        {
            int dots = (int)Time.time % 3;
            if (dots == 0) serverListStatus.text = "Searching.";
            else if (dots == 1) serverListStatus.text = "Searching..";
            else if (dots == 2) serverListStatus.text = "Searching...";
        }
        else
        {
            clickTime -= Time.deltaTime;
        }
    }




	public void buttonSinglePlayer()
	{
		NetworkMaster.initializeOfflineServer();
		NetworkMaster.connect();
        globalMenu.goToLobby(true);
		//Application.LoadLevel("world");

	}

	public void buttonHostServer()
	{
        if (GlobalMenu.playerName == null || GlobalMenu.playerName.Trim().Equals(""))
        {
            GlobalMenu.randomizePlayerName();
        }
		Debug.Log ("host server");
		//gameLobby.SetActive(true);
		NetworkMaster.initializeOnlineServer(GlobalMenu.playerName + "'s game");
		NetworkMaster.connect();
		//gameObject.SetActive(false);
	}

	public void buttonExitGame()
	{
		Application.Quit();
	}

	public void buttonJoin()
	{
        joinServer(selectedServer);
	}

	public void buttonFindServers()
	{
		MasterServer.RequestHostList(NetworkMaster.gameTypeName);
        hideServerListButtons(true);
        serverListStatus.gameObject.SetActive(true);
        searching = true;
        clickTime = 0;
	}

    public void buttonServerList(int index)
    {
        selectedServer = index;

        if (clickTime > 0 && lastClickedServer == index)
        {
            joinServer(index);
            lastClickedServer = -1;
        }
        else
        {
            lastClickedServer = index;
            clickTime = DOUBLECLICK_TIME;
        }
        
    }

    private void joinServer(int index)
    {
        if (index < 0 || index >= serverList.Length) return;

        NetworkMaster.initializeOnlineClient(serverList[index]);
        NetworkMaster.connect();
    }

    private void hideServerListButtons(bool hide)
    {
        foreach (Button b in serverListButtons)
        {
            b.gameObject.SetActive(!hide);
        }
    }

	private void updateServerList()
	{
        if (serverList.Length == 0)
        {
            serverListStatus.gameObject.SetActive(true);
            serverListStatus.text = "No servers online";
        }
        else
        {
            serverListStatus.gameObject.SetActive(false);
        }

        for (int i = 0; i < serverListButtons.Count; i++)
        {
            Button button = serverListButtons[i];
            if (i > serverList.Length)
            {
                Destroy(button.gameObject);
                serverListButtons.RemoveAt(i);
                i--;
                continue;
            }

            button.transform.FindChild("Text").GetComponent<Text>().text = getServerString(serverList[i]);
            button.gameObject.SetActive(true);
        }

        while (serverListButtons.Count < serverList.Length)
        {
            int buttonIndex = serverListButtons.Count;
            GameObject go = Instantiate(Resources.Load<GameObject>("GUI/ServerListButton"));
            go.transform.SetParent(serverListHolder);

            Button button = go.GetComponent<Button>();
            button.onClick.AddListener(() => buttonServerList(buttonIndex));
            button.transform.FindChild("Text").GetComponent<Text>().text = getServerString(serverList[buttonIndex]);
            RectTransform rect = button.image.rectTransform;
            rect.anchoredPosition3D = new Vector3(0,-5 + buttonIndex * -25, 0);
            rect.localScale = new Vector3(1, 1, 1);
            rect.localRotation = Quaternion.identity;

            serverListButtons.Add(button);
        }
	}

    private string getServerString(HostData data)
    {
        return data.gameName.Substring(0, 8) + " | " + data.connectedPlayers + "/" + data.playerLimit;
    }



	void OnConnectedToServer()
	{
        globalMenu.goToLobby(false);
	}

	void OnMasterServerEvent(MasterServerEvent e)
	{
		if(e == MasterServerEvent.HostListReceived)
		{
			serverList = MasterServer.PollHostList();
            searching = false;
			updateServerList();
		}
		if(e == MasterServerEvent.RegistrationSucceeded)
		{
            globalMenu.goToLobby(false);
		}
	}
}
