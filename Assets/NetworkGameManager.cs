using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkGameManager : NetworkBehaviour
{
    NetworkPlayer[] allPlayers;

    public RectTransform canvasServer;

    public Text nPlayer1;

    public override void OnStartServer()
    {
        canvasServer.gameObject.SetActive(true);      
    }

   
    IEnumerator StartCO()
    {
        yield return new WaitForSeconds(0.5f);

        if (isServer)
        {
            allPlayers = FindObjectsOfType(typeof(NetworkPlayer)) as NetworkPlayer[];
        }

        while (true)
        {
            foreach (var p in allPlayers)
            {
                nPlayer1.text = p.m_health.ToString();
            }
            yield return null;

            Debug.Log("Sono entrato nel While per settare la vita ad ogni player della lista");
        }

        yield break;
    }



    private void Update()
    {
        //foreach (var p in allPlayers)
        //{
        //    nPlayer1.text = p.m_health.ToString();
        //}       
    }


    public void DoStuff()
    {
        foreach (var player in allPlayers)
        {
            player.DoDoSfuff();           
        }
       
    }
}
