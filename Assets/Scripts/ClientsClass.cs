using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class ClientsClass
{

    public string nameClient;
    public int answerChoose;

    public ClientsClass(string _name, int _answerIndex)
    {
        nameClient = _name;
        answerChoose = _answerIndex;
    }
}
