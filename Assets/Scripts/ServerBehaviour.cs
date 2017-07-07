using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ServerBehaviour : NetworkBehaviour
{
    
    [SyncVar]
    public string questionString;

    public Text feedbackText;

    // Lista di stringhe delle Risposte da mandare ai Client
    public List<string> answerStringList;

    // Lista di tutti i Clients
    public List<GameObject> playerList = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(AddPlayerCO());       
    }

    public IEnumerator AddPlayerCO()
    {
        yield return new WaitForSeconds(0.5f);
        // lista di player (clients)
        playerList.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        //for (int i = 0; i < playerList.Count; i++)
        //{
        //    playerList[i].tag = "Untagged";
        //}
        Debug.LogError("Ho fatto la ricerca dei giocatori");
        Debug.LogError("I giocatori ora in gioco sono: " + playerList.Count);

        foreach (var player in playerList)
        {
            Debug.LogError(player.GetComponentInChildren<PlayerBehaviour>().name);
        }

        //SetOrderCanvasClient();
        SetQuestionOnClient(questionString);
        CreateButtonOnClient(answerStringList.Count);
        SetAnswerOnClient();
    }

    public void CreateButtonOnClient(int _numberOfString)
    {
        foreach (var player in playerList)
        {
            player.GetComponent<PlayerBehaviour>().RpcCreateUIButton(_numberOfString);
        }
    }

    public void SetQuestionOnClient(string _question)
    {
        foreach (var player in playerList)
        {
            player.GetComponent<PlayerBehaviour>().RpcSetQuestion(_question);
        }
        Debug.LogError("Ho inviato la domanda a tutti i giocaotri");
    }

    public void SetAnswerOnClient()
    {
        for (int i = 0; i < answerStringList.Count; i++)
        {
            foreach (var player in playerList)
            {
                player.GetComponent<PlayerBehaviour>().DoRpcSetAnswer(answerStringList[i].ToString(), i);
            }
        }
        Debug.LogError("Ho inviato le risposte in tutti i giocatori");
    }
    
    public void SetOrderCanvasClient()
    {
        foreach (var player in playerList)
        {
            //player.GetComponent<PlayerBehaviour>().RpcSetOrderCanvasClient();
        }
    }


    public void SetAnswerOnServer(string _nameSender, int _answerIndex)
    {
        feedbackText.text = "E' stata scelta la risposta " + _answerIndex + " da " + _nameSender;
        Debug.LogError("E' stata scelta la risposta " + _answerIndex + " da " + _nameSender);
    }
}

