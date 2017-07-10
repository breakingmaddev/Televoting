using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphLogic : MonoBehaviour {

    public int nAnswers;
    public List<GameObject> answerBar = new List<GameObject>();
    public GameObject barPrefab;

    //Lista finta di valori risposti
    public List<int> clientAnswers = new List<int>();

    // Use this for initialization
    void Start () {
        //Allo start deve prendere il numero di risposte dall'xml in base al contatore della domanda corrente 
        //nAnswers = PRENDERE IL VALORE TRAMITE PARSER!

        //Passa al graph il valore estratto dal file. Questo metodo verrà chiamato dal server allo scadere del tempo.
        CreateGraph();
	}

    //Crea il graph instanziando tante barre quante sono le risposte per quella specifica domanda
    public void CreateGraph()
    {
        for (int i = 0; i < nAnswers; i++)
        {
            GameObject newBar = Instantiate(barPrefab);
            newBar.gameObject.transform.SetParent(this.gameObject.transform);
            newBar.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
            answerBar.Add(newBar);
        }
        FillGraph();
    }

    //Riempie ogni singola barra in base a quanti utenti hanno dato quella specifica risposta
    //Se la risposta corrisponde a i allora aggiungi altrimenti passa avanti
    public void FillGraph()
    {
        float graphIncr = 1f / clientAnswers.Count;
        //Serve a far comparire il count di quante volte è stata scelta quella domanda
        int sameAnswers = 0;
        Debug.Log("Graph Incr: " + graphIncr);
        for (int i = 0; i < answerBar.Count; i++)
        {
            sameAnswers = 0;
            for (int j = 0; j < clientAnswers.Count; j++)
            {
                if(clientAnswers[j] == i)
                {
                    Debug.Log("ClientAnswers " + j + ": " + clientAnswers[j]);
                    answerBar[i].gameObject.GetComponent<Image>().fillAmount += graphIncr;
                    sameAnswers++;
                    answerBar[i].gameObject.transform.GetChild(0).GetComponent<Text>().text = sameAnswers.ToString();
                    //Qua dobbiamo inserire il testo della risposta 
                    //answerBar[i].gameObject.transform.GetChild(1).GetComponent<Text>().text = TESTO DELLA RISPOSTA;
                    Debug.Log("nClient: " + clientAnswers.Count);
                }
            }         
        }     
    }


}
