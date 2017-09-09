using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UIAddons;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class Level_1_1 : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
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
    bool zobraz = true;                   //- zobrazi progressBar v pripade ze nieje splneny pocet uloh  
    public Transform[] slots_control;     //- panel slotov ktore sa kontroluju ci nenastali zmeny

    Generator_uloh priklad;
    Kontrola skontroluj;

	SaveLoadProgress slp;

    void Start () {
		slp = new SaveLoadProgress ();
		reset = reset.GetComponent<Button>();
        skontroluj = new Kontrola(2);
        priklad = new Generator_uloh(lvl);
        table = priklad.get_array(3);        
        draw();
		slp.Load(lvl);
		progressBar.slider.value = slp.progress;
		zobraz = slp.zobraz;
        gratulation = gratulation.GetComponent<Canvas>();
        gratulation.enabled = false;
        nespravne = nespravne.GetComponent<Canvas>();
        nespravne.enabled = false;
        unlock_level = unlock_level.GetComponent<Canvas>();
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
    

    public void Restart()
    {
        Destroy();
        draw();
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

    public void progressAdd()
    {
        progressBar.slider.value += 0.5f;
    }

    void StartFillingUpProgressBar()
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
        List<int> kontrola = new List<int> { };
        foreach (Transform slotTransform in slots_control[0])
        {
            GameObject item = slotTransform.GetComponent<Slot>().item;
            if (item)
            {
                kontrola.Add(int.Parse(item.name.Substring(0, item.name.IndexOf("("))));
            }
        }
        foreach (Transform slotTransform in slots_control[1])
        {
            GameObject item = slotTransform.GetComponent<Slot>().item;
            if (item)
            {
                kontrola.Add(int.Parse(item.name.Substring(0, item.name.IndexOf("("))));
            }
        }        

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
					slp.Save(lvl,zobraz,progressBar.slider.value);
                }
                else
                {
                    congrats_show();
					slp.Save(lvl,zobraz,progressBar.slider.value);
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
        Application.LoadLevel(1);
    }

    IEnumerator congrats_hide()
    {
        yield return new WaitForSeconds(2.0f);

        gratulation.enabled = false;
        Application.LoadLevel(UnityEngine.Random.RandomRange(2, 4));
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);

            nespravne.enabled = false;
            Restart();
    }

    
}

namespace UnityEngine.EventSystems
{
    public interface IHasChanged : IEventSystemHandler
    {
        void HasChanged();
    }
}
