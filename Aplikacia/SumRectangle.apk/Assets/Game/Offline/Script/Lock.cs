using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Lock : MonoBehaviour {

    public Canvas lock2;
    public Canvas lock3;
    public Canvas lock4;
	SaveLoadProgress slp;

    // Use this for initialization
    void Start () {
		slp = new SaveLoadProgress ();
		slp.LoadLock();
        lock2 = lock2.GetComponent<Canvas>();
        lock2.enabled = false;
        lock2.enabled = true;
        lock3 = lock3.GetComponent<Canvas>();
        lock3.enabled = false;
        lock3.enabled = true;
        lock4 = lock4.GetComponent<Canvas>();
        lock4.enabled = false;
        lock4.enabled = true;
		locked(slp.locks);
    }

    public void locked(int pokrok)
    {
        if (pokrok == 2)
        {
            lock2.enabled = false;
            
        }
        else if (pokrok == 3)
        {
            lock2.enabled = false;
            lock3.enabled = false;
        }
        else if (pokrok >= 4)
        {
            lock2.enabled = false;
            lock3.enabled = false;
            lock4.enabled = false;
        }
        
    }
}

class SaveLoadProgress{

	public int locks{ get; set;}
	public float progress{ get; set;}
	public bool zobraz{ get; set;}

	public SaveLoadProgress(){
		zobraz = true;
	}

	public void Save(int lvl, bool zobraz, float progressBarvalue)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/progress" + lvl + ".dat");
		Progress data = new Progress();
		data.progr = progressBarvalue;
		data.zobrazenie = zobraz;
		bf.Serialize(file, data);
		file.Close();
	}

	public void SaveLock(int lvl)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/lock.dat");
		Locks data = new Locks();
		data.locked = lvl + 1;
		bf.Serialize(file, data);
		file.Close();
	}

	public void Load(int lvl)
	{
		if (File.Exists(Application.persistentDataPath + "/progress" + lvl + ".dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/progress" + lvl + ".dat", FileMode.Open);
			Progress data = (Progress)bf.Deserialize(file);
			file.Close();
			progress = data.progr;
			zobraz = data.zobrazenie;
		}
	}

	public void LoadLock()
	{
		if (File.Exists(Application.persistentDataPath + "/lock.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/lock.dat", FileMode.Open);
			Locks data = (Locks)bf.Deserialize(file);
			file.Close();
			locks = data.locked;
		}
	}
}


[Serializable]
class Progress
{
	public float progr;
	public bool zobrazenie;
}
[Serializable]
class Locks
{
	public int locked;
}
