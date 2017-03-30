using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Lock_Edit : MonoBehaviour {

    public Canvas lock2;
    public Canvas lock3;    
	SaveLoadProgress slp;

    // Use this for initialization
    void Start()
    {
		slp = new SaveLoadProgress ();
		slp.LoadLock();
        lock2 = lock2.GetComponent<Canvas>();
        lock2.enabled = false;
        lock2.enabled = true;
        lock3 = lock3.GetComponent<Canvas>();
        lock3.enabled = false;
        lock3.enabled = true;        
		locked(slp.LoadLockValue());


    }



    public void locked(int pokrok)
    {
        if (pokrok >= 2)
        {
            lock2.enabled = false;

        }
        if (pokrok >= 4)
        {
            lock2.enabled = false;
            lock3.enabled = false;
        }        

    }
}
