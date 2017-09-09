using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIAddons;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System.Linq;
using System;

public class Level_4_2 : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
{

    public Button reset;
    public GameObject[] itemPrefab; //- prefabsy cisle
    public  List<int> table;    //- zadany priklad
    public  List<int> table_M;  // - zadany priklad + moznosti
    string x = "Panel1_";           //- Nnazov panelov v ktorych su umiestnene sloty 
    string slot = "Slot_";
    GameObject[] slots;             //- pole slotov
    List<int> typ;                        //- ktore policko sa vynecha a nevykresli
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
    int pom_suc0;
    int pom_suc1;
    Kontrola skontroluj;
	SaveLoadProgress slp;


    // Use this for initialization
    void Start()
    {
		slp = new SaveLoadProgress ();
        reset = reset.GetComponent<Button>();
        skontroluj = new Kontrola(4);
        generator_uloh();        
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

    void generator_uloh()
    {
        priklad = new Generator_uloh(lvl);         
        table_M = new List<int> { };        
        typ = new List<int> { };
        List<int> pozicia = new List<int> { };                        
        table = priklad.reverse(priklad.get_array(10));
        pom_suc0 = priklad.reverse(table)[priklad.pom_suc0];
        pom_suc1 = priklad.reverse(table)[priklad.pom_suc1];        
        pom_suc0 = table.FindIndex(x => x == pom_suc0);
        pom_suc1 = table.FindIndex(x => x == pom_suc1);       
        typ.Add(pom_suc0);
        typ.Add(pom_suc1);

        for (int i = 0; i < table.Count; i++)
        {
            table_M.Add(table[i]);            
        }       
                
        while (typ.Count < 6 || pozicia.Count < 6)
        {
            int x = UnityEngine.Random.RandomRange(0, 10);
            if (typ.Contains(x) == false && typ.Count < 6)
                typ.Add(x);
            int y = UnityEngine.Random.RandomRange(10, 24);
            if (pozicia.Contains(y) == false && pozicia.Count < 6)
                pozicia.Add(y);
        }
       
        List<int> moznosti = new List<int> { };
        while (moznosti.Count < 14)
        {
            int x = UnityEngine.Random.RandomRange(1, 101);
            if (moznosti.Contains(x) == false && x != table_M[typ[0]] && x != table_M[typ[1]] && x != table_M[typ[2]] && x != table_M[typ[3]] && x != table_M[typ[4]])
            {
                moznosti.Add(x);
            }
        }
     
        for (int i = 0; i < moznosti.Count; i++)
        {
             table_M.Add(moznosti[i]);            
        }
        table_M[pozicia[0]] = table_M[typ[0]];
        table_M[pozicia[1]] = table_M[typ[1]];
        table_M[pozicia[2]] = table_M[typ[2]];
        table_M[pozicia[3]] = table_M[typ[3]];
        table_M[pozicia[4]] = table_M[typ[4]];   
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

    public void ChangeCol(int col,bool f)
    {
        if (f)
        {
            Image change = GameObject.Find(slot + col).GetComponent<Image>();
            change.color = new Color(0, 0, 255, 0.61f);
        }
        else {
            Image change = GameObject.Find(slot + col).GetComponent<Image>();
            change.color = new Color(255, 255, 255, 0.61f);
        }
    }


    public void draw()
    {
        // metoda vykresli vygenerovane riesenie do prazdnych slotov
        slots = new GameObject[24];
        int y = 1;
        int poc = 0;
        for (int i = 0; i < GameObject.Find("Panel1").gameObject.transform.childCount - 3; i++)
        {
            for (int j = 0; j < GameObject.Find(x + y).gameObject.transform.childCount; j++)
            {                
                if (poc != typ[0] && poc != typ[1] && poc != typ[2] && poc != typ[3] && poc != typ[4])
                {
                    slots[j] = GameObject.Find(x + y).gameObject.transform.GetChild(j).gameObject;
                    GameObject newItem = Instantiate(itemPrefab[table_M[poc]]) as GameObject;
                    newItem.transform.parent = slots[j].transform;
                    newItem.transform.localScale = new Vector3(1, 1, 1);
                    if (poc < 10)
                    {   // nastavy aby sa hodnoty ktore su nazaciatku umiestnene nedali presuvat
                        newItem.GetComponent<DragHandeler>().enabled = false;
                    }
                    ChangeCol(priklad.pom_suc0,true);
                    ChangeCol(priklad.pom_suc1,true);                    
                }
                poc++;
            }
            y++;
        }
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

        foreach (Transform slotTransform in slots_control[2])
        {
            GameObject item = slotTransform.GetComponent<Slot>().item;
            if (item)
            {
                kontrola.Add(int.Parse(item.name.Substring(0, item.name.IndexOf("("))));
            }
        }
        foreach (Transform slotTransform in slots_control[3])
        {
            GameObject item = slotTransform.GetComponent<Slot>().item;
            if (item)
            {
                kontrola.Add(int.Parse(item.name.Substring(0, item.name.IndexOf("("))));
            }
        }

        if (kontrola.Count == 10)
        {
            pom_suc0 = priklad.reverse(table)[priklad.pom_suc0];
            pom_suc1 = priklad.reverse(table)[priklad.pom_suc1];
            pom_suc0 = table.FindIndex(x => x == pom_suc0);
            pom_suc1 = table.FindIndex(x => x == pom_suc1);

            bool pom = skontroluj.Vyhodnot(kontrola, pom_suc0, pom_suc1, 45);
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
    

    public void progressAdd()
    {
        //- pripocita do progress baru 
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

    public void congrats_show()
    {
        // zobrazi gratulaciu a zavola metodu na skrytie gratulacie
        gratulation.enabled = true;
        StartCoroutine(congrats_hide());
    }

    public void nespravne_show()
    {
        //zobrazi oznam a zavola metodu na skrytie
        nespravne.enabled = true;
        StartCoroutine(nespravne_hide());
    }

    void show_unlock()
    {
        //zobrazi oznam a zavola metodu na skrytie
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
        Application.LoadLevel(UnityEngine.Random.RandomRange(8, 10));
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);

        nespravne.enabled = false;
        ChangeCol(priklad.pom_suc0, false);
        ChangeCol(priklad.pom_suc1, false);
        generator_uloh();
        Restart();

    }
}
