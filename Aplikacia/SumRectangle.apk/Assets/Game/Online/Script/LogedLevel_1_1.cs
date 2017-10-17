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

public class LogedLevel_1_1 : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
{
    [SerializeField]
    public Canvas gratulation;      //- object gratulacia
    public Canvas nespravne;        //- object nespravne
    public Canvas unlock_level;     //- object otvoreny novy level
    public CustomProgressBar progressBar; //- object progress bar
    public GameObject[] itemPrefab;
    GameObject[] slots;
    //bool isFillingProgressBar;  
    bool zobrazSlider = true;
    string x = "Panel1_";
    public int lvl;
    public static List<int> table;
    public Dictionary<string, string> ExamArray;
    Generator_uloh priklad;
    Kontrola skontroluj;
    SaveLoadProgress slp;
    FirebaseConnect f;
    CreateJson c;

    void Start()
    {
        c = new CreateJson();
        slp = new SaveLoadProgress();
        f = new FirebaseConnect();
        var refer = FirebaseDatabase.DefaultInstance
            .GetReference("/Class_ID/member/email/example/result");

        //ref.ChildAdded += HandleChildAdded;
        refer.ChildChanged += HandleChildChanged;
        //ref.ChildRemoved += HandleChildRemoved;
        //ref.ChildMoved += HandleChildMoved;
        gratulation = gratulation.GetComponent<Canvas>();
        nespravne = nespravne.GetComponent<Canvas>();
        unlock_level = unlock_level.GetComponent<Canvas>();
        skontroluj = new Kontrola(2);
        priklad = new Generator_uloh(lvl);
        table = priklad.get_array(3);
        CreateArrayExam(table);
        draw();
        slp.Load(lvl);
        progressBar.slider.value = slp.progress;
        zobrazSlider = slp.zobraz;
        gratulation.enabled = false;
        nespravne.enabled = false;
        unlock_level.enabled = false;
        progressBar.slider.maxValue = 10f;
        progressBar.slider.minValue = 0f;
        progressBar.slider.value = 0f;
        slp.Load(lvl);
        progressBar.slider.value = slp.progress;
        zobrazSlider = slp.zobraz;
        StartFillingUpProgressBar();
        HasChanged();
    }

    void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string key = args.Snapshot.Key;
        object value = args.Snapshot.Value;
        ExamArray[key] = value.ToString();
        Destroy();
        draw();
        // Do something with the data in args.Snapshot
    }
    public void CreateArrayExam(List<int> pr)
    {
        ExamArray = new Dictionary<string, string>();
        foreach (Transform objekt in GameObject.Find("Panel1").gameObject.transform)
        {
            foreach (Transform slot in objekt)
            {
                if (slot.name.Contains("SlotM"))
                {
                    int index = Int32.Parse(slot.name.Substring(slot.name.IndexOf("_") + 1));
                    ExamArray.Add(slot.name, pr[index].ToString());
                }
                else
                {
                    ExamArray.Add(slot.name, "null");
                }
            }
        }
    }

    public void Destroy()
    {
        foreach (Transform objekt in GameObject.Find("Panel1").gameObject.transform)
        {
            foreach (Transform slot in objekt)
            {
                if (slot.childCount > 0)
                {
                    DestroyImmediate(slot.gameObject.transform.GetChild(0).gameObject);
                }
            }
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
        foreach (Transform objekt in GameObject.Find("Panel1").gameObject.transform)
        {
            foreach (Transform slot in objekt)
            {
                if (ExamArray[slot.name] != "null")
                {
                    GameObject newItem = Instantiate(itemPrefab[int.Parse(ExamArray[slot.name])]) as GameObject;
                    newItem.transform.parent = slot.transform;
                    newItem.transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
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
        //isFillingProgressBar = true;
    }
    public void HasChanged()
    {
        // metoda kontroluje ci nenastali zmeny v slotoch
        List<int> kontrola = new List<int> { };
        foreach (Transform objekt in GameObject.Find("Panel1").gameObject.transform)
        {
            foreach (Transform slot in objekt)
            {
                //Debug.Log (slot);
                if (slot.GetComponent<Slot>().item == null)
                {
                    ExamArray[slot.name] = "null";
                }
                else
                {
                    ExamArray[slot.name] = slot.GetComponent<Slot>().item.name.Substring(0, slot.GetComponent<Slot>().item.name.IndexOf("("));
                }
                if (!slot.name.Contains("SlotM"))
                {
                    GameObject item = slot.GetComponent<Slot>().item;
                    if (item)
                    {
                        kontrola.Add(int.Parse(item.name.Substring(0, item.name.IndexOf("("))));
                    }
                }
            }
        }
        f.UpdateResult(ExamArray);
        if (kontrola.Count == 3)
        {
            bool pom = skontroluj.Vyhodnot(kontrola);
            if (pom)
            {
                progressAdd();
                if (zobrazSlider && progressBar.slider.value == progressBar.slider.maxValue)
                {
                    zobrazSlider = false;
                    show_unlock();
                    slp.SaveLock(lvl);
                    slp.Save(lvl, zobrazSlider, progressBar.slider.value);
                }
                else
                {
                    congrats_show();
                    slp.Save(lvl, zobrazSlider, progressBar.slider.value);
                }
            }
            else
            {
                nespravne_show();
            }
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
        Application.LoadLevel(18);
    }

    IEnumerator congrats_hide()
    {
        yield return new WaitForSeconds(2.0f);

        gratulation.enabled = false;
        Application.LoadLevel(UnityEngine.Random.RandomRange(19, 20));
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);

        nespravne.enabled = false;
        Restart();
    }
}