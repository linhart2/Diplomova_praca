using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Firebase.Database;

public class GlobalData
{
    public static PlayerData playerData = new PlayerData();
}
[Serializable]
public class PlayerData
{
    public string UserId;
    public string Name;
    public string SelectedClass;
    public bool LoggedUser;
    public Dictionary<string, string> Classes;
    public string idSelectedExamOnBoard;
    public int selectedExamOnBoardCount;
    public List<DataSnapshot> selectedExamOnBoard;
    public DataSnapshot zdielaneDataAll;
    public string cestaKZdielanymDatam;
}