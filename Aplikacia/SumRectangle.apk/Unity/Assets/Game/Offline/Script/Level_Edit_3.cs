using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Level_Edit_3 : MonoBehaviour
{
    public Canvas number;
    public Canvas ma_ries;
    public Canvas nema_ries;    
    public Button[] show;
    int lastchar;
    public Button[] butt;
    GameObject[] slots;
    public GameObject[] itemPrefab;
    public Button kontrola;
    public Canvas skontroluj;
    public static List<int> pole_cisel;
    public static bool ries;

    // Use this for initialization
    void Start () {
		
        pole_cisel = new List<int> { };

        for (int i = 0; i < show.Length; i++)
        {
            pole_cisel.Add(0);
            show[i] = show[i].GetComponent<Button>();
        }

        for (int i = 0; i < butt.Length; i++)
        {
            butt[i] = butt[i].GetComponent<Button>();
        }

        kontrola = kontrola.GetComponent<Button>();
        kontrola.enabled = false;
        skontroluj = skontroluj.GetComponent<Canvas>();
        skontroluj.enabled = false;
        number = number.GetComponent<Canvas>();
        number.enabled = false;
        ma_ries = ma_ries.GetComponent<Canvas>();
        ma_ries.enabled = false;
        nema_ries = nema_ries.GetComponent<Canvas>();
        nema_ries.enabled = false;             
    }

    public void Show_Number(int x)

    {
        number.enabled = true;
        string pom = show[x].name.Substring(0, show[x].name.Length);
        char lastcharacter = pom[pom.Length - 1];
        lastchar = x;// Convert.ToInt32(new string(lastcharacter,1));                        
    }

    public void Hide_Number(int x)
    {
        pole_cisel[lastchar] = x;
        number.enabled = false;
        Destroy();
        draw();
        if (!pole_cisel.Contains(0))
        {
            skontroluj.enabled = true;
            kontrola.enabled = true;
        }        
    }

    public void draw()
    {
        // metoda vykresli vygenerovane riesenie do prazdnych slotov
        slots = new GameObject[13];
        int y = 1;
        int poc = 0;
        for (int i = 0; i < GameObject.Find("Panel1").gameObject.transform.childCount - 3; i++)
        {
            for (int j = 0; j < GameObject.Find("Panel1_" + y).gameObject.transform.childCount; j++)
            {
                if (pole_cisel[poc] != 0)
                {             
                    slots[j] = GameObject.Find("Panel1_" + y).gameObject.transform.GetChild(j).gameObject;
                    GameObject newItem = Instantiate(itemPrefab[pole_cisel[poc]-1]) as GameObject;
                    newItem.transform.parent = slots[j].transform;
                    newItem.GetComponent<DragHandeler>().enabled = false;
                    newItem.transform.localScale = new Vector3(1, 1, 1);
                }
                poc++;
            }
            y++;
        }
    }

    public void Restart()
    {
        skontroluj.enabled = false;
        kontrola.enabled = false;
        for (int i = 0; i < pole_cisel.Count; i++)
        {
            pole_cisel[i] = 0;
        }
        Destroy();
    }

    public void Destroy()
    {
        int y = 1;
        for (int i = 0; i < GameObject.Find("Panel1").gameObject.transform.childCount - 3; i++)
        {
            for (int j = 0; j < GameObject.Find("Panel1_" + y).gameObject.transform.childCount; j++)
            {
                var pom = GameObject.Find("Panel1_" + y).gameObject.transform.GetChild(j);
                if (pom.gameObject.transform.childCount > 0)
                    DestroyImmediate(pom.gameObject.transform.GetChild(0).gameObject);
            }
            y++;
        }
    }

    public void ChangeCol(int col,bool f)
    {
        Image change = GameObject.Find("Slot_" + col).GetComponent<Image>();
        if (f)
            change.color = new Color(255, 0, 0, 0.61f);
        else
            change.color = new Color(255, 255, 255, 0.61f);
    }

    public void Skontroluj(int x)
    {        
        int y = x;
        bool r = true;
        int pom = 0;
        int[][] pole = new int[x][];
        List<int> pom_pole = new List<int> { };
        for (int i = 0; i < pole_cisel.Count; i++)
        {
            if (i < x) { pom_pole.Add(0); }
            else { pom_pole.Add(pole_cisel[i]); }                        
        }                   
        for (int i = 0; i < x; i++)
        {
            pole[i] = new int[y];
            for (int j = 0; j < y; j++)
            {                
                pole[i][j] = pole_cisel[pom];
                ChangeCol(pom,false);
                pom++;
            }
            y--;   
        }        
        for (int i = 0; i < pole.Length-1; i++)
        {
            for (int j = 0; j < pole[i].Length-1; j++)
            {
                if (pole[i][j] + pole[i][j + 1] != pole[i + 1][j])
                {
                    ChangeCol(pom_pole.FindIndex(g => g == pole[i + 1][j]), true);
                    r = false;                    
                }                                                           
            }            
        }
        if (r) {
            ma_ries.enabled = true;
            ries = true;
        }
        else {
            nema_ries.enabled = true;
            ries = false;
        }
    }
		
    public void close_info()
    {
        ma_ries.enabled = false;
        nema_ries.enabled = false;
    }
    public void play(int pom)
    {
        ma_ries.enabled = false;
        nema_ries.enabled = false;
        Application.LoadLevel(pom);
    }
}