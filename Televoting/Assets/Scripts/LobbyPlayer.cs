using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkLobbyPlayer
{
    [SyncVar]
    public string playerName = "New Player";

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (this.readyToBegin)
                {
                    SendNotReadyToBeginMessage();
                }

                else
                {
                    SendReadyToBeginMessage();
                }
            }
        }
    }

}
