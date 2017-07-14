using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class ServerBehaviour : NetworkBehaviour
{
    public List<ClientsClass> clientsList = new List<ClientsClass>();
    
    //public Dictionary<int, List<ClientsClass>> dictVoters = new Dictionary<int, List<ClientsClass>>();

    public List<GameSessionClass> gameSession = new List<GameSessionClass>();

    // Contatore della domanda corrente per il dictVoters
    public int currentQuestion = 0;

    [SyncVar]
    public float timerCounter = 0f;
    [SyncVar]
    public float finalTimer = 0f;
    [SyncVar]
    public string questionString;

    public Text feedbackText, questionText, timerText;
    private GraphLogic refGL;

#region ParserArea
    public TextAsset csvFile; // file CSV da leggere

    private char lineSeparater = '\n';
    private char fieldSeparator = ',';

    public int startCSVIndex = 0;
    private int endCSVIndex = 0;

    List<string> readedData = new List<string>();

#endregion

    // Lista delle risposte da mandare ai Clients
    public List<string> answerStringList = new List<string>();

    // Lista di tutti i Clients
    public List<GameObject> playerList = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(AddPlayerCO());
       
        refGL = FindObjectOfType<GraphLogic>();
        
        //Legge i dati da file
        readedData.AddRange(csvFile.text.Split(lineSeparater));
        
        //Metodo che passa il testo della domanda da file
        SetDataFromCSV();

        Invoke("FillDictionary", 1f);
    }

    public void SetDataFromCSV()
    {
        string endLine = "--";
        Debug.Log("ReadCSV");
        //legge e imposta la domanda
        questionString = readedData[startCSVIndex];
        questionText.text = questionString;

        //DAVID DEVE SISTEMARE QUESTA COSA DEL 1000
        for (int i = endCSVIndex; i < 1000; i++)
        {
            if (!readedData[i].Contains(endLine))
            {
                endCSVIndex++;
            }
            else
            {
                break;
            }
            Debug.LogWarning(endCSVIndex);
        }

        //legge la cella in ordine di posizione lungo la prima colonna
        int j = -1;
        startCSVIndex += 1;
        for (int i = startCSVIndex; i < endCSVIndex; i++)
        {
            j++;
            answerStringList.Insert(j, readedData[i]);
            Debug.Log(answerStringList[j]);
        }
    }

    //Esegue le funzionalità del bottone Next
    public void NextQuestion()
    {
        StartCoroutine(CallResetOnClient());
        answerStringList.Clear();
        currentQuestion++;
        FillDictionary();
        endCSVIndex ++;
        startCSVIndex = endCSVIndex;
        timerText.text = ("Timer");
        SetDataFromCSV();
        refGL.RestartGraph();
    }

    private void FillDictionary()
    {
        // Aggiungo al dizionario la lista di Domande e la lista di ClientsClass
        gameSession.Add(new GameSessionClass());
        gameSession[currentQuestion].questionIndex = currentQuestion;
    }

    // Quando il timer finisce lo resetto e disattivo tutti i bottoni sui client e mi faccio mandare la risposta scelta
    public IEnumerator TimerCO()
    {
        timerCounter = 10f;
        Debug.LogError("Il timer è partito e parte da: " + timerCounter);
        while (timerCounter > finalTimer)
        {
            yield return new WaitForSecondsRealtime(1f);
            timerCounter--;
            timerText.text = (timerCounter / 60).ToString("00") + ":" + (timerCounter % 60).ToString("00");
        }

        //timerCounter = 0;
        timerText.text = ("Time's Up!");

        foreach (var player in playerList)
        {
            player.GetComponent<PlayerBehaviour>().RpcDeactiveButton();
            player.GetComponent<PlayerBehaviour>().RpcSelectedAnswer();
        }

        StartCoroutine(refGL.CallGraphSetup());
        
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
    }

    //Setta tutto il necessario sui client
    public void SetupClient()
    {
        SetQuestionOnClient(questionString);

        CreateButtonOnClient(answerStringList.Count);

        SetAnswerOnClient();

        StartCoroutine(TimerCO());
    }


    //Chiama il reset del client
    public IEnumerator CallResetOnClient()
    {
        yield return null;
        foreach (var player in playerList)
        {
            player.GetComponent<PlayerBehaviour>().RpcResetClient();
        }
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

        
        SetupGameSession(_nameSender, _answerIndex);

    }

    // Aggiunge le domande e le risposte date da ogni Clients al dizionario
    public void SetupGameSession(string _nameSender, int _answerIndex)
    {
        ClientsClass newClient = new ClientsClass(_nameSender, _answerIndex);
        Debug.Log(newClient);
        gameSession[currentQuestion].clientClassArch.Add(newClient);

        //Debug.Log(dictVoters.Keys.Count + " - " + dictVoters.Values.Count);
        Debug.Log(clientsList.Count);
    }

    // Add data to CSV file
    public void SaveGameSessionData()
    {

        string filePath = getPath();

        StreamWriter writer = new StreamWriter(filePath);

        writer.WriteLine("Index Domanda, Utente, Risposta");

        for (int i = 0; i < gameSession.Count; i++)
        {
            for (int j = 0; j < gameSession[i].clientClassArch.Count; j++)
            {
                writer.WriteLine(gameSession[i].questionIndex.ToString() +
                "," + gameSession[i].clientClassArch[j].nameClient.ToString() +
                "," + gameSession[i].clientClassArch[j].answerChoose.ToString());
            }
            
        }

        writer.Flush();

        writer.Close();

        /*
        // Following line adds data to CSV file
        File.AppendAllText(getPath() + "/Resources/Log_Televoting.csv", lineSeparater + rollNoInputField.text + fieldSeparator + nameInputField.text);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
*/
    }

    // Get path for given CSV file
    private static string getPath()
    {
#if UNITY_EDITOR
        Directory.CreateDirectory(Application.persistentDataPath + "/LogData");
        return Application.persistentDataPath + "/LogData/" + "Log_Televoting.csv";
#elif UNITY_STANDALONE_WIN
        Directory.CreateDirectory(Application.persistentDataPath + "/LogData");
        return Application.persistentDataPath + "/LogData/" + "Log_Televoting.csv";
#elif UNITY_ANDROID
		return Application.persistentDataPath;// +fileName;
#elif UNITY_IPHONE
		return GetiPhoneDocumentsPath();// +"/"+fileName;
#else
		return Application.dataPath;// +"/"+ fileName;
#endif
    }

    // Get the path in iOS device
    private static string GetiPhoneDocumentsPath()
    {
        string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
        path = path.Substring(0, path.LastIndexOf('/'));
        return path + "/Documents";
    }
}

