using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkLobbyPlayer
{
    [SyncVar]
    public string playerName = "New Player";

    [SyncVar]
    public Color colorPlayer = Color.white;
    

}
