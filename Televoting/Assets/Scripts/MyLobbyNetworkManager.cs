using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyLobbyNetworkManager : NetworkLobbyManager
{
    public Button startButton;
    private bool allReady;
    public override void OnLobbyServerPlayersReady()
    {
        Debug.Log("ALL READY");
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);    
    }
   
    private void Update()
    {
        for (int i = 0; i < lobbySlots.Length; i++)
        {
            if (!lobbySlots[i]) continue;
            if (!lobbySlots[i].readyToBegin)
            {
                allReady = false;
                Debug.Log(allReady);
                break;
            }
            else
            {
                allReady = true;
                Debug.Log(allReady);
                break;
            }
        }

        if (allReady)
        {
            startButton.gameObject.SetActive(true);
        }
    }

    public void StartScene()
    {
        ServerChangeScene(playScene);
        this.enabled = false;
    }
}
