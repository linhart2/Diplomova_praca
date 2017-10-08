using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UIAddons;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using Firebase.Database;

public class Level_1_1Show : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
{
    public Button reset;
    public GameObject[] itemPrefab;
    public static List<int> table;
    string x = "Panel1_";
    GameObject[] slots;    
    [SerializeField]
    public Canvas gratulation;      //- object gratulacia
    public Canvas nespravne;        //- object nespravne
    public Canvas unlock_level;     //- object otvoreny novy level
    public CustomProgressBar progressBar; //- object progress bar
    bool isFillingProgressBar;            //- I dont know                  
    public int lvl;                       //- Level
	bool zobraz;                   //- Show progress bar if value < maxValue  
    public Transform[] slots_control;     //- panel slotov ktore sa kontroluju ci nenastali zmeny
    Generator_uloh priklad;
    Kontrola skontroluj;
	SaveLoadProgress slp;
	FirebaseConnect f;
	CreateJson c;

    void Start () {
		c = new CreateJson ();
		slp = new SaveLoadProgress ();
		f = new FirebaseConnect ();
		FirebaseDatabase.DefaultInstance
			.GetReference("/Class_ID/member/email/example/result/")
			.ValueChanged += HandleValueChanged;
		
		reset = reset.GetComponent<Button>();
		gratulation = gratulation.GetComponent<Canvas>();
		nespravne = nespravne.GetComponent<Canvas>();
		unlock_level = unlock_level.GetComponent<Canvas>();
        skontroluj = new Kontrola(2);
        priklad = new Generator_uloh(lvl);
        table = priklad.get_array(3); 
        draw();
		slp.Load (lvl);
		progressBar.slider.value = slp.progress;
		zobraz = slp.zobraz;
        gratulation.enabled = false;
        nespravne.enabled = false;
        unlock_level.enabled = false;        
        progressBar.slider.maxValue = 10f;
        progressBar.slider.minValue = 0f;
        progressBar.slider.value = 0f;
        slp.Load(lvl);
		progressBar.slider.value = slp.progress;
		zobraz = slp.zobraz;
        StartFillingUpProgressBar();
        HasChanged();


    }

	public void draw()
	{
		//create a new item, name it, and set the parent
		slots = new GameObject[3];
		for (int i = 0; i < GameObject.Find(x + 3).gameObject.transform.childCount; i++)
		{
			slots[i] = GameObject.Find(x + 3).gameObject.transform.GetChild(i).gameObject;
			GameObject newItem = Instantiate(itemPrefab[table[i]]) as GameObject;
			newItem.transform.parent = slots[i].transform;
			newItem.transform.localScale = new Vector3(1, 1, 1);
		}
	}

	public void Destroy()
	{        
		int y = 1;
		for (int i = 0; i < GameObject.Find("Panel1").gameObject.transform.childCount - 3; i++)
		{
			for (int j = 0; j < GameObject.Find(x + y).gameObject.transform.childCount; j++)
			{
				var pom = GameObject.Find(x + y).gameObject.transform.GetChild(j);            
				if (pom.gameObject.transform.childCount > 0)
					DestroyImmediate(pom.gameObject.transform.GetChild(0).gameObject);                                              
			}
			y++;
		}        
	}

	void HandleValueChanged(object sender, ValueChangedEventArgs args) {
		if (args.DatabaseError != null) {
			Debug.LogError(args.DatabaseError.Message);
			return;
		}
		Debug.Log (args.Snapshot.Value.ToString());
		ParseData (args.Snapshot.Value.ToString());
		Destroy ();
		draw();
			// Do something with the data in args.Snapshot
	}

	public void ParseData(string args){
		string[] arg = args.Split ('|');
		List<string[]> arg2 = new List<string[]>();
		for (int i=0; i<arg.Length;i++){
			arg2.Add (arg[i].Split (','));
		}
		List<int> p_t = new List<int>();
		for (int i = 0; i < arg2 [2].Length; i++) {
			if (arg2 [2] [i] != "Null") {
				p_t.Add (int.Parse (arg2 [2] [i]));
			}
		}
		table = p_t;

	}
		
    public void Restart()
    {
        Destroy();
        draw();
    }
		
    


    public void progressAdd()
    {
        progressBar.slider.value += 0.5f;
    }

    public void StartFillingUpProgressBar()
    {
        //progressBar.slider.value;
        if (progressBar.slider.value == progressBar.slider.maxValue)
        {
            progressBar.gameObject.SetActive(false);
        }               
        isFillingProgressBar = true;
    }

    public void HasChanged()
    {
        // metoda kontroluje ci nenastali zmeny v slotoch
		int pomIndex = 0;
		List<List<string>> pomSlot = new List<List<string>>();
        List<int> kontrola = new List<int> { };
		foreach (Transform panels in GameObject.Find("Panel1").gameObject.transform) {
			if (panels.name.Contains ("Panel1_")) {
				pomSlot.Add (new List<string> ());
				foreach (Transform slot in panels) {
					if (slot.GetComponent<Slot> ().item == null) {
						pomSlot [pomIndex].Add ("Null");
					} else {
						pomSlot[pomIndex].Add(slot.GetComponent<Slot> ().item.name.Substring(0,slot.GetComponent<Slot> ().item.name.IndexOf ("(")));
					}
					if (!slot.name.Contains ("SlotM")) {
						GameObject item = slot.GetComponent<Slot> ().item;
						if (item) {
							kontrola.Add (int.Parse (item.name.Substring (0, item.name.IndexOf ("("))));
						}
					}
				}
				pomIndex++;
			}
		}
		/*f.UpdateResult (c.GetJson(pomSlot));*/
        if (kontrola.Count == 3)
        {
            bool pom = skontroluj.Vyhodnot(kontrola);
            if (pom)
            {
                progressAdd();
                if (zobraz && progressBar.slider.value == progressBar.slider.maxValue)
                {
                    zobraz = false;
                    show_unlock();
                    slp.SaveLock(lvl);
					slp.Save(lvl, zobraz, progressBar.slider.value);
                }
                else
                {
                    congrats_show();
					slp.Save(lvl,zobraz, progressBar.slider.value);
                }
            }
            else { nespravne_show(); }
        }
    }

    public void congrats_show()
    {
        gratulation.enabled = true;
        StartCoroutine(congrats_hide());
    }

    public void nespravne_show()
    {
        nespravne.enabled = true;
        StartCoroutine(nespravne_hide());
    }

    void show_unlock()
    {
        unlock_level.enabled = true;
        StartCoroutine(hide_unlock());
    }

    IEnumerator hide_unlock()
    {
        yield return new WaitForSeconds(2.0f);

        unlock_level.enabled = false;
        Application.LoadLevel(0);
    }

    IEnumerator congrats_hide()
    {
        yield return new WaitForSeconds(2.0f);

        gratulation.enabled = false;
        Application.LoadLevel(16);
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);

            nespravne.enabled = false;
            Restart();
    }
}

[Serializable]
public class CreateJson {
	public CreateJson(){
	}
	public string GetJson (List<List<string>> input) {
		string json = "";
		foreach (List<string> pom in input) {
			json += "|";
			foreach (string s in pom) {
				json += s + ",";
			}
			json = json.Substring (0, json.Length -1);
		}
		json = json.Substring (1, json.Length-1);
		return json;
	}
}