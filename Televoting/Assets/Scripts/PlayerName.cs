using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerName : NetworkBehaviour
{
    [SyncVar]
    public string playerName = "Votante";

    public TextMesh textName;

    private void Start()
    {
        textName.text = playerName;
    }


    
}
