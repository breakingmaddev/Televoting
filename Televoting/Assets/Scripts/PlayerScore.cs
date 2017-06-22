using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerScore : NetworkBehaviour
{
    [SyncVar]
    public short playerPersonalScore = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }


}
