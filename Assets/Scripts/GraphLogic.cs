using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphLogic : MonoBehaviour {

    public int nAnswers;
    public List<GameObject> answerBar = new List<GameObject>();
    public GameObject barPrefab;

    public List<Color32> answerColors = new List<Color32>();

    private ServerBehaviour refSB;

    //Lista di valori risposti
    public List<int> clientAnswersList = new List<int>();

    //Lista delle classi client contenenti anche le risposte
    public List<ClientsClass> clientList = new List<ClientsClass>();

    public Text noVoterText;

    // Use this for initialization
    void Start () {
        StartCoroutine(SearchSBCO());
        //Allo start deve prendere il numero di risposte dall'xml in base al contatore della domanda corrente 
        nAnswers = refSB.answerStringList.Count;

        //Passa al graph il valore estratto dal file. Questo metodo verrà chiamato dal server allo scadere del tempo.
        CreateGraph();
	}

    // Cerco il referimento al Server
    public IEnumerator SearchSBCO()
    {
        while (refSB == null)
        {
            refSB = FindObjectOfType<ServerBehaviour>();
            yield return null;
        }
        Debug.LogError("Ho trovato Server Behaviour");
        yield break;
    }


    //Crea il graph instanziando tante barre quante sono le risposte per quella specifica domanda
    public void CreateGraph()
    {
        for (int i = 0; i < nAnswers; i++)
        {
            GameObject newBar = Instantiate(barPrefab);
            newBar.gameObject.transform.SetParent(this.gameObject.transform);
            newBar.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
            newBar.gameObject.GetComponent<Image>().color = answerColors[i];
            answerBar.Add(newBar);
            //Qua inseriamo il testo e il colore della risposta e della percentuale
            answerBar[i].gameObject.transform.GetChild(1).GetComponent<Text>().text = refSB.answerStringList[i];
            answerBar[i].gameObject.transform.GetChild(1).GetComponent<Text>().color = answerColors[i];
            answerBar[i].gameObject.transform.GetChild(0).GetComponent<Text>().color = answerColors[i];
        }
        //ReadData();
        //FillGraph();
    }


    public IEnumerator CallGraphSetup()
    {
        yield return new WaitForSeconds(1f);
        ReadData();
    }

    public void ReadData()
    {
        clientList.AddRange(refSB.dictVoters[refSB.currentQuestion]);
        for (int i = 0; i < clientList.Count; i++)
        {
            clientAnswersList.Add(clientList[i].answerChoose);
        }
        FillGraph();
    }

    //Riempie ogni singola barra in base a quanti utenti hanno dato quella specifica risposta
    //Se la risposta corrisponde a i allora aggiungi altrimenti passa avanti
    public void FillGraph()
    { 
        float graphIncr = 1f / clientAnswersList.Count;
        //Serve a far comparire il count di quante volte è stata scelta quella domanda
        float sameAnswers = 0;
        Debug.Log("Graph Incr: " + graphIncr);
        float noVoterCount = 0;
        float percSingleClient = (100 / clientAnswersList.Count);
        Debug.Log("PercSingleClient: " + percSingleClient);

        for (int i = 0; i < answerBar.Count; i++)
        {
            sameAnswers = 0;
            for (int j = 0; j < clientAnswersList.Count; j++)
            {
                if(clientAnswersList[j] == i)
                {
                    Debug.Log("ClientAnswers " + j + ": " + clientAnswersList[j]);
                    answerBar[i].gameObject.GetComponent<Image>().fillAmount += graphIncr;
                    sameAnswers++;
                    
                    Debug.Log("nClient: " + clientAnswersList.Count);
                } 

                float finalPerc = sameAnswers * percSingleClient;
                answerBar[i].gameObject.transform.GetChild(0).GetComponent<Text>().text = (finalPerc.ToString() + "%");

            }         
        }


        for (int j = 0; j < clientAnswersList.Count; j++)
        {
            if (clientAnswersList[j] == -1)
            {
                noVoterCount++;
                Debug.Log("No Voter Count: " + noVoterCount);
            }
        }

        float percNoVoters = noVoterCount * percSingleClient;
        noVoterText.text = ("Astenuti: " + percNoVoters.ToString() + "%");
    }


}
