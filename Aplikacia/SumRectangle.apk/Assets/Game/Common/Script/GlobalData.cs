using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GlobalData
{
    public static PlayerData playerData = new PlayerData();
}
[Serializable]
public class PlayerData{
	public string Name;
	public string SelectedClasses;
	public bool LoggedUser;
	public List<string> Classes;
}