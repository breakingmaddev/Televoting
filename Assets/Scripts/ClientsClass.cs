﻿using System.Collections;
using System.Collections.Generic;
using System;

public class ClientsClass : IComparable<ClientsClass>
{
    public string nameClient;
    public int answerChoose;

    public ClientsClass(string _name, int _answerIndex)
    {
        nameClient = _name;
        answerChoose = _answerIndex;
    }

    public int CompareTo(ClientsClass other)
    {
        if (other == null)
        {
            return 1;
        }

        return answerChoose - other.answerChoose;
    }
}