using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class SaveLoad
{
	public float progressBar {get;set;}
	public bool zobraz{ get; set;}
	public void Save(float progressBarValue, bool zobraz, int lvl)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/progress" + lvl + ".dat");
		Progress data = new Progress();
		data.progr = progressBarValue;
		data.zobrazenie = zobraz;

		bf.Serialize(file, data);
		file.Close();
		Debug.Log("Save");
	}

	public void SaveLock(int lvl)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/lock.dat");

		Locks data = new Locks();
		data.locked = lvl + 1;

		bf.Serialize(file, data);
		file.Close();
		Debug.Log("Save_lock");
	}

	public void Load(int lvl)
	{
		if (File.Exists(Application.persistentDataPath + "/progress" + lvl + ".dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/progress" + lvl + ".dat", FileMode.Open);
			Progress data = (Progress)bf.Deserialize(file);
			file.Close();
			progressBar = data.progr;
			zobraz = data.zobrazenie;
			Debug.Log(progressBar);
			//zobraz = data.zobrazenie;
		}
	}
}

