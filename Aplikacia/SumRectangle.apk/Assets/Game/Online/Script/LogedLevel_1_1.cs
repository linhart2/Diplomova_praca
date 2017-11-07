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
using UnityEngine.SceneManagement;

public class LogedLevel_1_1 : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
{
    [SerializeField]
    public Canvas gratulation;      //- object gratulacia
    public Canvas nespravne;        //- object nespravne
    public Canvas unlock_level;     //- object otvoreny novy level
    public CustomProgressBar progressBar; //- object progress bar
    public GameObject[] itemPrefab;
    GameObject[] slots;
    bool zobrazSlider = true;
    string x = "Panel1_";
    public int lvl;
    public static List<int> table;
    public Dictionary<string, string> ExamArray;
    Generator_uloh priklad;
    Kontrola skontroluj;
    SaveLoadProgress saveloadprogress;
    FirebaseConnect firebase;
    public Dictionary<string, object> ListStudent = new Dictionary<string, object>();

    void Start()
    {
        saveloadprogress = new SaveLoadProgress();
        firebase = new FirebaseConnect();
        var controlChangeData = FirebaseDatabase.DefaultInstance
                                                .GetReference("/Class_ID/member/email/example/result/");
        controlChangeData.ChildChanged += HandleChildChanged;
        var controlAllStudentInClass = FirebaseDatabase.DefaultInstance
            .GetReference("/USERS");
        controlAllStudentInClass.ChildChanged += HandleShowAllUserInClassChange;
        controlAllStudentInClass.ChildAdded += HandleShowAllUserInClassAdd;
        controlAllStudentInClass.ChildRemoved += HandleShowAllUserInClassRemove;


        gratulation = gratulation.GetComponent<Canvas>();
        nespravne = nespravne.GetComponent<Canvas>();
        unlock_level = unlock_level.GetComponent<Canvas>();
        skontroluj = new Kontrola(2);
        priklad = new Generator_uloh(lvl);
        table = priklad.get_array(3);
        CreateArrayExam(table);
        draw();
        saveloadprogress.Load(lvl);
        progressBar.slider.value = saveloadprogress.progress;
        zobrazSlider = saveloadprogress.zobraz;
        gratulation.enabled = false;
        nespravne.enabled = false;
        unlock_level.enabled = false;
        progressBar.slider.maxValue = 10f;
        progressBar.slider.minValue = 0f;
        progressBar.slider.value = 0f;
        saveloadprogress.Load(lvl);
        progressBar.slider.value = saveloadprogress.progress;
        zobrazSlider = saveloadprogress.zobraz;
        StartFillingUpProgressBar();
        HasChanged();
    }

    public void ShareScreenWith()
    {

    }

    public void HandleShowAllUserInClassAdd(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string key = args.Snapshot.Key;
        object value = args.Snapshot.Value;
        ListStudent[key] = value;
    }
    public void HandleShowAllUserInClassChange(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string key = args.Snapshot.Key;
        object value = args.Snapshot.Value;
        ListStudent[key] = value;
    }
    public void HandleShowAllUserInClassRemove(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string key = args.Snapshot.Key;
        object value = args.Snapshot.Value;
        ListStudent.Remove(key);
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

        Debug.Log(key + "|" + value);
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
        if (progressBar.slider.value.Equals(progressBar.slider.maxValue))
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
        firebase.UpdateResult(ExamArray);
        solution_control(kontrola);

    }

    public void solution_control(List<int> kontrola)
    {
        if (kontrola.Count == 3)
        {
            bool pom = skontroluj.Vyhodnot(kontrola);
            if (pom)
            {
                progressAdd();
                if (zobrazSlider && progressBar.slider.value.Equals(progressBar.slider.maxValue))
                {
                    zobrazSlider = false;
                    show_unlock();
                    saveloadprogress.SaveLock(lvl);
                    saveloadprogress.Save(lvl, zobrazSlider, progressBar.slider.value);
                }
                else
                {
                    congrats_show();
                    saveloadprogress.Save(lvl, zobrazSlider, progressBar.slider.value);
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
        SceneManager.LoadScene(18);
    }

    IEnumerator congrats_hide()
    {
        yield return new WaitForSeconds(2.0f);

        gratulation.enabled = false;
        //SceneManager.LoadScene(UnityEngine.Random.Range(19, 20));
        SceneManager.LoadScene(21);
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);

        nespravne.enabled = false;
        Restart();
    }
}