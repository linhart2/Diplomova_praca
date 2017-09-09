using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UIAddons;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class Level_3_1 : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
{

    public Button reset;
    public GameObject[] itemPrefab;
    public static List<int> table;
    string x = "Panel1_";
    string slot = "Slot_";
    GameObject[] slots;    
    public int pom_suc0;
    public int pom_suc1;
    [SerializeField]
    public Canvas gratulation;      //- object gratulacia
    public Canvas nespravne;        //- object nespravne
    public Canvas unlock_level;     //- object otvoreny novy level
    public CustomProgressBar progressBar; //- object progress bar
    bool isFillingProgressBar;            //- I dont know                  
    public int lvl;                       //- Level
	bool zobraz = true;                   //- I dont know  
    public Transform[] slots_control;     //- panel slotov ktore sa kontroluju ci nenastali zmeny
    Generator_uloh priklad;
    Kontrola skontroluj;
	SaveLoadProgress slp;

    void Start()
    {
		slp = new SaveLoadProgress ();
        reset = reset.GetComponent<Button>();
        skontroluj = new Kontrola(3);
        priklad = new Generator_uloh(lvl);
        table = priklad.get_array(6);       
        pom_suc0 = priklad.pom_suc0;
        pom_suc1 = priklad.pom_suc1;
        draw();       
		slp.Load (lvl);
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
		slp.Load (lvl);
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
        pom_suc0 = priklad.pom_suc0;
        pom_suc1 = priklad.pom_suc1;
        Destroy();
        draw();
    }

    public void ChangeCol(int col)
    {
        Image change = GameObject.Find(slot+col).GetComponent<Image>();
        change.color = new Color(0, 0, 255, 0.61f);
    }

    public void draw()
    {        
        //create a new item, name it, and set the parent
        slots = new GameObject[6];
        for (int i = 0; i < GameObject.Find(x + 4).gameObject.transform.childCount; i++)
        {
            slots[i] = GameObject.Find(x + 4).gameObject.transform.GetChild(i).gameObject;         
            GameObject newItem = Instantiate(itemPrefab[table[i]]) as GameObject;
            newItem.transform.parent = slots[i].transform;            
            newItem.transform.localScale = new Vector3(1, 1, 1);
        }

        ChangeCol(pom_suc0);
        ChangeCol(pom_suc1);
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
        foreach (Transform slotTransform in slots_control[2])
        {
            GameObject item = slotTransform.GetComponent<Slot>().item;
            if (item)
            {
                kontrola.Add(int.Parse(item.name.Substring(0, item.name.IndexOf("("))));               
            }
        }


        if (kontrola.Count == 6)
        {
            pom_suc0 = priklad.reverse(table)[priklad.pom_suc0];
            pom_suc1 = priklad.reverse(table)[priklad.pom_suc1];
            pom_suc0 = table.FindIndex(x => x == pom_suc0);
            pom_suc1 = table.FindIndex(x => x == pom_suc1);

            bool pom = skontroluj.Vyhodnot(kontrola, pom_suc0, pom_suc1, 25);
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
					slp.Save(lvl, zobraz, progressBar.slider.value);
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
        Application.LoadLevel(UnityEngine.Random.RandomRange(6, 8));
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);

        nespravne.enabled = false;
        Restart();
    }
}