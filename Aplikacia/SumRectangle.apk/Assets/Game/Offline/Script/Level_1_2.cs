using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIAddons;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System.Linq;

public class Level_1_2 : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
{
    public Button reset;
    public GameObject[] itemPrefab; //- prefabsy cisle
    public static List<int> table;     //- zadany priklad
    public static List<int> table_M;
    string x = "Panel1_";           //- Nnazov panelov v ktorych su umiestnene sloty 
    GameObject[] slots;             //- pole slotov
    List<int> typ;                      //- ktore policko sa vynecha a nevykresli
    [SerializeField]
    public Canvas gratulation;      //- object gratulacia
    public Canvas nespravne;        //- object nespravne
    public Canvas unlock_level;     //- object otvoreny novy level
    public CustomProgressBar progressBar; //- object progress bar
    bool isFillingProgressBar;            
    public int lvl;                       
    bool zobraz = true;                   
    public Transform[] slots_control;     //- panel slotov ktore sa kontroluju ci nenastali zmeny
    Generator_uloh priklad;
    Kontrola skontroluj;
	SaveLoadProgress slp;

    void Start () {
		slp = new SaveLoadProgress ();
        reset = reset.GetComponent<Button>();
        skontroluj = new Kontrola(2);
        generator_uloh();
        draw();
        slp.Load(lvl);
		progressBar.slider.value = slp.LoadSliderValue ();
		zobraz = slp.LoadShowSlider();
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
		progressBar.slider.value = slp.LoadSliderValue ();
		zobraz = slp.LoadShowSlider();
        StartFillingUpProgressBar();
        HasChanged();
    }

    void generator_uloh()
    {
        priklad = new Generator_uloh(lvl);
        table = priklad.reverse(priklad.get_array(3));
        table_M = new List<int> { };
        typ = new List<int> { };
        List<int> pozicia = new List<int> { };

        for (int i = 0; i < table.Count; i++)
        {
            table_M.Add(table[i]);
        }

        while (typ.Count < 2 || pozicia.Count < 2)
        {
            int x = UnityEngine.Random.RandomRange(0, 3);
            if (typ.Contains(x) == false && typ.Count < 2)
                typ.Add(x);
            int y = UnityEngine.Random.RandomRange(3, 7);
            if (pozicia.Contains(y) == false && pozicia.Count < 2)
                pozicia.Add(y);
        }

        List<int> moznosti = new List<int> { };
        while (moznosti.Count < 4)
        {
            int x = UnityEngine.Random.RandomRange(1, 10);
            if (moznosti.Contains(x) == false && x != table_M[typ[0]])
            {
                moznosti.Add(x);
            }
        }

        for (int i = 0; i < moznosti.Count; i++)
        {
            table_M.Add(moznosti[i]);
        }
        table_M[pozicia[0]] = table_M[typ[0]];        
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
        // metoda vykresli vygenerovane riesenie do prazdnych slotov
        slots = new GameObject[7];
        int y = 1;
        int poc = 0;
        for (int i = 0; i < GameObject.Find("Panel1").gameObject.transform.childCount - 3; i++)
        {
            for (int j = 0; j < GameObject.Find(x + y).gameObject.transform.childCount; j++)
            {
                if (poc != typ[0])
                {
                    slots[j] = GameObject.Find(x + y).gameObject.transform.GetChild(j).gameObject;
                    GameObject newItem = Instantiate(itemPrefab[table_M[poc]]) as GameObject;
                    newItem.transform.parent = slots[j].transform;
                    newItem.transform.localScale = new Vector3(1, 1, 1);
                    if (poc < 3)
                    {   // nastavy aby sa hodnoty ktore su nazaciatku umiestnene nedali presuvat
                        newItem.GetComponent<DragHandeler>().enabled = false;
                    }
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
        Application.LoadLevel(Random.RandomRange(2, 4));
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);

        nespravne.enabled = false;
        generator_uloh();
        Restart();
        
    }
}
