using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkLobbyManager
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
    }

}
