using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyLobbyNetworkManager : NetworkLobbyManager
{
    public override void OnLobbyServerConnect(NetworkConnection conn)
    {
        Debug.Log("Lobby Server Ready");
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
    }

    private void OnGUI()
    {
        bool allReady = true;
        for (int i = 0; i < lobbySlots.Length; i++)
        {
            if (!lobbySlots[i]) continue;
            if(!lobbySlots[i].readyToBegin)
            {
                allReady = false;
                break;
            }
        }

        if (allReady)
        {
            if (GUILayout.Button("Start"))
            {
                ServerChangeScene(playScene);
            }
        }
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        var lobbyScript = lobbyPlayer.GetComponent<LobbyPlayer>();
        var gamePlayerName = gamePlayer.GetComponent<PlayerName>();
        var gamePlayerScore = gamePlayer.GetComponent<PlayerScore>();

        gamePlayerName.playerName = lobbyScript.playerName;

        Debug.Log("Set from lobby: " + lobbyScript.playerName);
        return true;
    }

}
