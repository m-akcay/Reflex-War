using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject singlePlayerButton = null;
    [SerializeField] private GameObject multiPlayerButton = null;
    [SerializeField] private GameObject waitingStatusPanel = null;
    [SerializeField] private GameObject cancelConnectionButton = null;

    [SerializeField] private GameObject settingsPanel = null;
    [SerializeField] private GameObject settingsButton = null;

    [SerializeField] private TextMeshProUGUI waitingStatusText = null;

    private bool isConnecting = false;
    private const string GameVersion = "0.1";
    private const int MaxPlayersPerRoom = 2;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void singleplayerButton_onClick()
    {
        SceneManager.LoadScene("WarScene");
    }

    public void multiplayerButton_onClick()
    {
        singlePlayerButton.SetActive(false);
        multiPlayerButton.SetActive(false);

        isConnecting = true;

        waitingStatusPanel.SetActive(true);
        
        waitingStatusText.text = "Searching...";

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        waitingStatusPanel.SetActive(false);
        multiPlayerButton.SetActive(true);
        singlePlayerButton.SetActive(true);
        Debug.Log($"Disconnectted due to {cause}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No clients are waiting for opponent, creating a new room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Client successfully joined a room");

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount != MaxPlayersPerRoom)
        {
            waitingStatusText.text = "Waiting for opponent";
            cancelConnectionButton.SetActive(true);

            Debug.Log("Client is waiting for opponent");
        }
        else
        {
            waitingStatusText.text = "Opponent found";
            Debug.Log("Matching is ready to begin");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayersPerRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            Debug.Log("Match is ready to begin");
            waitingStatusText.text = "Opponent found";

            PhotonNetwork.LoadLevel("WarScene_multiplayer");
        }
    }

    public void settings_onClick()
    {
        settingsPanel.SetActive(true);
        GetComponent<PlayerNameInput>().enableNameInput();
        settingsButton.SetActive(false);
    }
    public void settings_back_onClick()
    {
        GetComponent<PlayerNameInput>().disableNameInput();
        settingsPanel.SetActive(false);
        settingsButton.SetActive(true);
    }
    public void quit_onClick()
    {
        PhotonNetwork.Disconnect();
        Application.Quit();
    }

    public void cancelConnection_onClick()
    {
        //PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
        singlePlayerButton.SetActive(true);
        multiPlayerButton.SetActive(true);
        cancelConnectionButton.SetActive(false);
        waitingStatusPanel.SetActive(false);
    }
}
