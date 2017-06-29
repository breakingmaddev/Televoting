using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerName : NetworkBehaviour
{
    [SyncVar]
    public string playerName = "Concorrente";

    //public TextMesh textName;

    private void Start()
    {
        if (isLocalPlayer)
        {
            this.gameObject.name = playerName;
        }
    }

    private void Update()
    { 
        //textName.text = playerName;
        //textName.transform.position = this.gameObject.transform.position;
    }
}
