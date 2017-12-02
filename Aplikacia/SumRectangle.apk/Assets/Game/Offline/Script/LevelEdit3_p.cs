using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UIAddons;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;

public class LevelEdit3_p : MonoBehaviour, IHasChanged
{
    public Button reset;
    public GameObject[] itemPrefab;
    public static List<int> table;
    string x = "Panel1_";
    string slot = "Slot_";
    GameObject[] slots;
    public int cislo_panelu;
    public int cislo_sceny;
    public int slots_count;
    [SerializeField]
    public Canvas gratulation;      //- object gratulacia
    public Canvas nespravne;        //- object nespravne    
    public int velkost;
    public Transform[] slots_control;     //- panel slotov ktore sa kontroluju ci nenastali zmeny
    Kontrola skontroluj;
    bool ries;

    void Start()
    {
        skontroluj = new Kontrola(velkost);
        table = Level_Edit_3.pole_cisel;
        ries = Level_Edit_3.ries;
        reset = reset.GetComponent<Button>();
        draw();
        gratulation = gratulation.GetComponent<Canvas>();
        gratulation.enabled = false;
        nespravne = nespravne.GetComponent<Canvas>();
        nespravne.enabled = false;
        HasChanged();
    }

    public void Destroy()
    {
        int y = 1;
        for (int i = 0; i < GameObject.Find("Panel1").gameObject.transform.childCount - 4; i++)
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
        ries = Level_Edit_3.ries;
        Destroy();
        draw();
    }

    public void draw()
    {
        //create a new item, name it, and set the parent
        slots = new GameObject[slots_count];
        for (int i = 0; i < GameObject.Find(x + cislo_panelu).gameObject.transform.childCount; i++)
        {
            slots[i] = GameObject.Find(x + cislo_panelu).gameObject.transform.GetChild(i).gameObject;
            GameObject newItem = Instantiate(itemPrefab[table[i]]) as GameObject;
            newItem.transform.parent = slots[i].transform;
            newItem.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void HasChanged(bool zaznamenajDoDB = true)
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
        if (slots_control.Length > 2)
        {
            foreach (Transform slotTransform in slots_control[2])
            {
                GameObject item = slotTransform.GetComponent<Slot>().item;
                if (item)
                {
                    kontrola.Add(int.Parse(item.name.Substring(0, item.name.IndexOf("("))));
                }
            }
        }
        if (slots_control.Length > 3)
        {
            foreach (Transform slotTransform in slots_control[3])
            {
                GameObject item = slotTransform.GetComponent<Slot>().item;
                if (item)
                {
                    kontrola.Add(int.Parse(item.name.Substring(0, item.name.IndexOf("("))));
                }
            }
        }
        if (kontrola.Count == slots_count)
        {
            bool pom = skontroluj.Vyhodnot(kontrola);
            if (pom) { congrats_show(); }
            else { nespravne_show(); }
        }
    }

    public void nema_ries()
    {
        if (!ries) { congrats_show(); }
        else { nespravne_show(); }
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

    IEnumerator congrats_hide()
    {
        yield return new WaitForSeconds(2.0f);

        gratulation.enabled = false;
        Application.LoadLevel(cislo_sceny);
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);

        nespravne.enabled = false;
        Restart();
    }
}