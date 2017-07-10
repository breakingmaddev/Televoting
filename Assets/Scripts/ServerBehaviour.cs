using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ServerBehaviour : NetworkBehaviour
{
    public List<ClientsClass> clientsList = new List<ClientsClass>();
    public Dictionary<int, List<ClientsClass>> dictVoters = new Dictionary<int, List<ClientsClass>>();

    // Contatore della domanda corrente per il dictVoters
    public int currentQuestion = 0;

    [SyncVar]
    public float timer = 0f;
    [SyncVar]
    public float totTimer = 5f;
    [SyncVar]
    public string questionString;

    public Text feedbackText;

    // Lista delle risposte da mandare ai Clients
    public List<string> answerStringList;

    // Lista di tutti i Clients
    public List<GameObject> playerList = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(AddPlayerCO());
        StartCoroutine(TimerCO());

        // Aggiungo al dizionario la lista di Domande e la lista di ClientsClass
        dictVoters.Add(currentQuestion, clientsList);
    }

    // Quando il timer finisce lo resetto e disattivo tutti i bottoni sui client e mi faccio mandare la risposta scelta
    public IEnumerator TimerCO()
    {
        while (timer < totTimer)
        {
            yield return new WaitForSecondsRealtime(1f);
            timer++;
            Debug.Log(timer);
        }
       
        timer = 0;

        foreach (var player in playerList)
        {
            player.GetComponent<PlayerBehaviour>().RpcDeactiveButton();
            player.GetComponent<PlayerBehaviour>().RpcSelectedAnswer();
        }

        yield break;
    }

    // Mette tutti i giocatori nella lista PlayerList
    public IEnumerator AddPlayerCO()
    {
        yield return new WaitForSeconds(0.5f);
        // lista di player (clients)
        playerList.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        
        Debug.LogError("Ho fatto la ricerca dei giocatori");
        Debug.LogError("I giocatori ora in gioco sono: " + playerList.Count);

        foreach (var player in playerList)
        {
            Debug.LogError(player.GetComponentInChildren<PlayerBehaviour>().name);
        }


        SetQuestionOnClient(questionString);

        CreateButtonOnClient(answerStringList.Count);

        SetAnswerOnClient();
    }

    // Crea i pulsanti premibili dai clients
    public void CreateButtonOnClient(int _numberOfString)
    {
        foreach (var player in playerList)
        {
            player.GetComponent<PlayerBehaviour>().RpcCreateUIButton(_numberOfString);
        }
    }

    // Setta la domanda a tutti i clients
    public void SetQuestionOnClient(string _question)
    {
        foreach (var player in playerList)
        {
            player.GetComponent<PlayerBehaviour>().RpcSetQuestion(_question);
        }
        Debug.LogError("Ho inviato la domanda a tutti i giocaotri");
    }

    // Mette il testo delle risposte dentro i pulsanti
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
    
    // Prendo la risposta scelta dai clients
    public void SetAnswerOnServer(string _nameSender, int _answerIndex)
    {
        feedbackText.text = "E' stata scelta la risposta " + _answerIndex + " da " + _nameSender;
        Debug.LogError("E' stata scelta la risposta " + _answerIndex + " da " + _nameSender);

        
        SetupDictionary(_nameSender, _answerIndex);

    }

    // Aggiunge le domande e le risposte date da ogni Clients al dizionario
    public void SetupDictionary(string _nameSender, int _answerIndex)
    {
        ClientsClass newClient = new ClientsClass(_nameSender, _answerIndex);
        Debug.Log(newClient);
        
        clientsList.Add(newClient);

        Debug.Log(dictVoters.Keys.Count + " - " + dictVoters.Values.Count);
        Debug.Log(clientsList.Count);
    }
}

