using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class PlayerBehaviour : NetworkBehaviour
{
    public Text questionText, identifierText;
    public GameObject panelAnswer;

    private ServerBehaviour refSB;
    private GameObject lastClicked;

    private List<GameObject> listButton = new List<GameObject>();
    public List<GameObject> sortlistButton = new List<GameObject>();
    public GameObject answerButton;

    [SyncVar]
    public string playerIdentifier;

    private void Start()
    {
        StartCoroutine(SearcSBCO());
        StartCoroutine(DisableOtherClientCO());
        Debug.LogError("Sono nell'Awake di PlayerBehaviour");
    }

    public IEnumerator SearcSBCO()
    {
        while (refSB == null)
        {
            refSB = FindObjectOfType<ServerBehaviour>();
            yield return null;
        }
        Debug.LogError("Ho trovato Server Behaviour");
        yield break;
    }

    [ClientRpc]
    public void RpcSetQuestion(string _question)
    {
        questionText.text = _question;
        Debug.LogError("Io Client mi sono settato la domanda");
    }

    [ClientRpc]
    public void RpcCreateUIButton(int _answerIndex)
    {

        for (int i = 0; i < _answerIndex; i++)
        {
            GameObject newPlayerBase = Instantiate(answerButton);
            newPlayerBase.gameObject.transform.SetParent(panelAnswer.transform);
            newPlayerBase.name = "Answer " + i;
            newPlayerBase.GetComponent<Button>().onClick.AddListener(DoSelectAnswer);
            listButton.Add(newPlayerBase);
            Debug.LogError("Io Client mi sono creato il bottone " + newPlayerBase.name);
        }

        
    }

    public void DoRpcSetAnswer(string _answerText, int _answerIndex)
    {
    
        RpcSetAnswer(_answerText, _answerIndex);
   
    }

    [ClientRpc]
    public void RpcSetAnswer(string _answerText, int _answerIndex)
    {
        
        listButton[_answerIndex].GetComponentInChildren<Text>().text = _answerText;
        Debug.LogError("Io Client mi sono settato la risposta del bottone " + listButton[_answerIndex]);
       

        //for (int i = 0; i < _answerList.Count; i++)
        //{
        //    GameObject newPlayerBase = Instantiate(answerButton);
        //    newPlayerBase.gameObject.transform.SetParent(this.gameObject.transform);
        //    newPlayerBase.name = "Answer " + i;

        //    newPlayerBase.GetComponentInChildren<Text>().text = _answerList[i];
        //}
        //Debug.LogError("Ho CREATO UN BOTTONE E INSERITO IL TESTO DELLA RISPOSTA");
        //GameObject newPlayerBase = Instantiate(answerButton);
        //NetworkServer.Spawn(newPlayerBase);
        //newPlayerBase.gameObject.transform.SetParent(this.gameObject.transform);
        //newPlayerBase.name = "Answer " + _answerIndex;

        //sortlistButton[_answerIndex].GetComponentInChildren<Text>().text = _answerText;
    }

    public IEnumerator DisableOtherClientCO()
    {
        playerIdentifier = SystemInfo.deviceUniqueIdentifier;
        this.gameObject.name = playerIdentifier;
        yield return new WaitForSeconds(0.5f);
        List<GameObject> playerList = new List<GameObject>();
        playerList.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        foreach (var player in playerList)
        {
            if (!hasAuthority)
            {
                player.SetActive(false);
                Debug.LogError(player.GetComponentInChildren<PlayerBehaviour>().name + " è stato disattivato su questo client");
                break;
            }
            
        }

        //GetComponent<Canvas>().sortingOrder = 1;
        identifierText.text = this.gameObject.name + " - "; // + GetComponent<Canvas>().sortingOrder;
    }

    // Seleziona la risposta, la rende verde e poi rende tutte le altre non interattive
    public void DoSelectAnswer ()
    {
        lastClicked = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        lastClicked.GetComponent<Image>().color = Color.green;
        lastClicked.GetComponent<Button>().Select();
        
        for (int i = 0; i < listButton.Count; i++)
        {
            if (listButton[i].GetComponent<Button>() != lastClicked.GetComponent<Button>())
            {
                //listButton[i].GetComponent<Button>().interactable = false;
                Debug.LogError("Settati tutti gli altri bottoni non interagibili ");
            }

            else
            {
                // Inoltre invia al Server un resoconto su chi ha votato cosa
                CmdSelectAnswer(this.gameObject.name, i);
                Debug.LogError("Lancio il Command per passare i valori al Server");
            }
        }

        Debug.LogError("Premuto il bottone: " + lastClicked.name);
    }


    
    [Command]
    public void CmdSelectAnswer(string _playerName, int _index)
    {
        refSB.SetAnswerOnServer(_playerName, _index); 
    }

    public void FindAnswer()
    {

        // Per trovare ognuno le proprie risposte, ovvero i Children di ogni PanelAnswer
        for (int i = 0; i < this.transform.childCount; i++)
        {
            listButton.Add(this.transform.GetChild(i).gameObject);
        }

        // Riordina la lista di risposte per essere sincronizzata a come il Server gliele assegna
        sortlistButton = listButton.OrderBy(go => go.name).ToList();
        Debug.LogError("Ho riordinato la lista");
    }

}
