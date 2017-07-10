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
        //nAnswers = fare il parser!

        //Passa al graph il valore estratto dal file
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
        Debug.Log("Graph Incr: " + graphIncr);
        for (int i = 0; i < answerBar.Count; i++)
        {
            for (int j = 0; j < clientAnswers.Count; j++)
            {
                if(clientAnswers[j] == i)
                {
                    Debug.Log("ClientAnswers " + j + ": " + clientAnswers[j]);
                    answerBar[i].gameObject.GetComponent<Image>().fillAmount += graphIncr;
                    Debug.Log("nClient: " + clientAnswers.Count);
                }
            }         
        }     
    }


}
