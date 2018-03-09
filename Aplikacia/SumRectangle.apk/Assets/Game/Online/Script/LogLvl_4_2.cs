using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIAddons;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using Firebase.Database;


public class LogLvl_4_2 : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
{
    private const string NAZOV_PANELOV = "Panel1_";
    private const string SLOT = "Slot_";
    private const string SLOTM = "SlotM_";
    private const string FLAG_PODMIENKA = "-PODMIENKA";
    private const string FLAG_DISABLED = "-DISABLED";

    [SerializeField]
    public Canvas gratulation;      //- object gratulacia
    public Canvas nespravne;        //- object nespravne
    public Canvas showSharedWith;      //- object zdielat s....
    public Canvas infoAboutShare;

    public GameObject[] itemPrefab; //- prefabsy cisla
    private GameObject[] slots;             //- pole slotov
    public GameObject togglePrefab;

    public Button reset;
    public List<int> table;    //- zadany priklad
    public Dictionary<string, int?> table_M;  // - zadany priklad + moznosti
    private List<string> _poliaKtoreSaNevykreslia;                        //- ktore policko sa vynecha a nevykresli

    private Kontrola skontroluj;
    public Transform[] slots_control;     //- panel slotov ktore sa kontroluju ci nenastali zmeny    
    private Generator_uloh priklad;
    private int pom_suc0;
    private int pom_suc1;
    public int lvl;                       //- Level

    private FirebaseCommunicationLibrary _fbc;
    public Dictionary<string, string> ExamArray = new Dictionary<string, string>();
    public Dictionary<string, GameObject> ListStudent = new Dictionary<string, GameObject>();
    private PlayerData playerData = new PlayerData();

    private string pathToSharedData;
    private bool useButtonShareSchreenWith = false;
    DatabaseReference controlChangeData;
    DatabaseReference controlSharedScreenWithMe;
    DatabaseReference controlAllStudentInClass;

    private string _pathWay;

    private void Awake()
    {
        Button btnBack = GameObject.Find("Back").GetComponent<Button>();
        btnBack.onClick.AddListener(delegate
        {
            UnbindAllHandler();
            SceneManager.LoadScene("LoggedSelectLevel");

        });
        Button btnZrus = GameObject.Find("btnZrus").GetComponent<Button>();
        btnZrus.onClick.AddListener(delegate
        {
            showSharedWith.enabled = false;
            GameObject.Find("tbOznacVsetkych").GetComponent<Toggle>().isOn = false;
            DeselectAllUser();

        });
        Button btnShare = GameObject.Find("Share").GetComponent<Button>();
        btnShare.onClick.AddListener(delegate
        {
            showSharedWith.enabled = true;
        });
        Button btnZdielaj = GameObject.Find("btnZdielaj").GetComponent<Button>();
        btnZdielaj.onClick.AddListener(delegate
        {
            ShareScreenWith();
        });
        Toggle tbOznacVsetkych = GameObject.Find("tbOznacVsetkych").GetComponent<Toggle>();
        tbOznacVsetkych.onValueChanged.AddListener(delegate
        {
            SelectAllUser();
        });
        _fbc = new FirebaseCommunicationLibrary();
    }

    // Use this for initialization
    void Start()
    {
        playerData = GlobalData.playerData;

#if DEBUG
        playerData.Name = "TestLingo";
        playerData.UserId = "ZAT4DktlgdYBVGwXYRpOfA3temm1";
        playerData.SelectedClass = "-KweS-rI-bTBYVX3g3vS";
        playerData.LoggedUser = true;
#endif
        _pathWay = "/USERS/" + playerData.UserId + "/ACTUAL_SCREEN/";
        controlAllStudentInClass = FirebaseDatabase.DefaultInstance
                                                   .GetReference("/CLASSES/" + playerData.SelectedClass + "/ONLINE_STUDENTS");
        controlAllStudentInClass.ChildAdded += HandleShowAllUserInClassAdd;
        controlAllStudentInClass.ChildRemoved += HandleShowAllUserInClassRemove;

        controlSharedScreenWithMe = FirebaseDatabase.DefaultInstance
                                                        .GetReference("/USERS/" + playerData.UserId + "/waitForShare");
        controlSharedScreenWithMe.ChildAdded += HandleControlRequestSharedScreen;


        reset = reset.GetComponent<Button>();
        skontroluj = new Kontrola(4);
        generator_uloh();

        /*FirebaseDatabase.DefaultInstance.GetReference(pathWay)
                         .GetValueAsync().ContinueWith(task =>
                         {
                             if (task.IsFaulted)
                             {
                                 Debug.Log("skontroluj pripojenie");
                                 SceneManager.LoadScene("CheckConnection");
                             }
                             else if (task.IsCompleted)
                             {
                                 DataSnapshot snapshot = task.Result;
                                 table_M = new Dictionary<string, int?>();
                                 typ = new List<string>();
                                 List<string> podmienka = new List<string>();

                                 foreach (var x in snapshot.Children.ToList())
                                 {
                                     if (x.Key.Contains(FLAG_PODMIENKA) && x.Value.ToString().Contains(FLAG_DISABLED))
                                     {
                                         string hodnotaString = x.Value.ToString().Substring(0, x.Value.ToString().IndexOf('-'));
                                         int? hodnota = null;
                                         if (hodnotaString != "null")
                                             hodnota = Int32.Parse(hodnotaString);
                                         table_M.Add(x.Key.Substring(0, x.Key.IndexOf('-')), hodnota);
                                         podmienka.Add(x.Key.Substring(0, x.Key.IndexOf('-')));
                                     }
                                     else if (!x.Key.Contains(FLAG_PODMIENKA) && x.Value.ToString().Contains(FLAG_DISABLED))
                                     {
                                         string hodnotaString = x.Value.ToString().Substring(0, x.Value.ToString().IndexOf('-'));
                                         int? hodnota = null;
                                         if (hodnotaString != "null")
                                             hodnota = Int32.Parse(hodnotaString);
                                         table_M.Add(x.Key, hodnota);
                                     }
                                     else if (x.Key.Contains(FLAG_PODMIENKA) && !x.Value.ToString().Contains(FLAG_DISABLED))
                                     {
                                         int? hodnota = null;
                                         if (x.Value.ToString() == "null")
                                             typ.Add(x.Key.Substring(0, x.Key.IndexOf('-')));
                                         if (x.Value.ToString() != "null")
                                             hodnota = Int32.Parse(x.Value.ToString());
                                         table_M.Add(x.Key.Substring(0, x.Key.IndexOf('-')), hodnota);
                                         podmienka.Add(x.Key.Substring(0, x.Key.IndexOf('-')));
                                     }
                                     else
                                     {
                                         int? hodnota = null;
                                         if (x.Value.ToString() == "null")
                                             typ.Add(x.Key);
                                         if (x.Value.ToString() != "null")
                                             hodnota = Int32.Parse(x.Value.ToString());
                                         table_M.Add(x.Key, hodnota);
                                     }
                                 }

                                 pom_suc0 = Int32.Parse(podmienka[0].Substring(podmienka[0].IndexOf('_') + 1));
                                 pom_suc1 = Int32.Parse(podmienka[1].Substring(podmienka[1].IndexOf('_') + 1));

                                 draw();
                                 CreateArrayExam();
                                 HasChanged();
                             }

                         });*/

        draw();
        CreateArrayExam();

        gratulation = gratulation.GetComponent<Canvas>();
        gratulation.enabled = false;
        nespravne = nespravne.GetComponent<Canvas>();
        nespravne.enabled = false;
        infoAboutShare = infoAboutShare.GetComponent<Canvas>();
        infoAboutShare.enabled = false;
        showSharedWith = showSharedWith.GetComponent<Canvas>();
        showSharedWith.enabled = false;

        HasChanged();

    }
    public void ShareScreenWith()
    {
        SharedScreen screen = new SharedScreen()
        {
            screeen_locker = false,
            admin_name = playerData.Name,
            screen_name = "screen1a",
        };
        String key = FirebaseDatabase.DefaultInstance.GetReference("/SHARED_SCREEN").Push().Key;
        LeaderBoardEntry entry = new LeaderBoardEntry(ExamArray);
        Dictionary<string, object> entryValues = entry.ToDictionary();
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        pathToSharedData = "/SHARED_SCREEN/" + key + "/data/";
        childUpdates[pathToSharedData] = entryValues;
        _fbc.addSharedScreen(key, screen);
        _fbc.inserMyIdToSharedScreen(playerData.UserId, playerData.Name, key);
        FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(childUpdates);

        List<string> selectedStudent = new List<string>();
        foreach (var student in ListStudent)
        {
            if (student.Value.transform.GetChild(0).GetComponent<Toggle>().isOn)
            {
                selectedStudent.Add(student.Key);
            }
        }
        SendRequest(key, selectedStudent);

        controlChangeData = FirebaseDatabase.DefaultInstance
                                            .GetReference("/SHARED_SCREEN/" + key + "/data/");
        controlChangeData.ChildChanged += HandleChildChanged;
        showSharedWith.enabled = false;
        DeselectAllUser();

        useButtonShareSchreenWith = true;
    }

    public void SendRequest(string Screeen_key, List<string> zoznamLudi)
    {
        foreach (var user in zoznamLudi)
        {
            SharedScreenRequest request = new SharedScreenRequest()
            {
                admin_name = playerData.Name,
                share_object = Screeen_key
            };
            String key = FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + user + "/waitForShare/").Push().Key;
            _fbc.SendRequestWithShareScreen(user, key, request);
        }
    }

    public void SelectAllUser()
    {
        bool selectAll = GameObject.Find("tbOznacVsetkych").GetComponent<Toggle>().isOn;
        if (selectAll)
        {
            foreach (var student in ListStudent)
            {
                student.Value.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
            }
        }
        else
        {
            DeselectAllUser();
        }
    }
    public void DeselectAllUser()
    {
        foreach (var student in ListStudent)
        {
            student.Value.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
        }
    }

    public void AcceptShareScreen(string screenKey, string requestKey)
    {
        pathToSharedData = "/SHARED_SCREEN/" + screenKey + "/data/";
        infoAboutShare.enabled = false;
        useButtonShareSchreenWith = true;
        Console.WriteLine("AcceptShareScren id={0} name={1} screenKey={2}", playerData.UserId, playerData.Name, screenKey);
        _fbc.inserMyIdToSharedScreen(playerData.UserId, playerData.Name, screenKey);
        controlChangeData = FirebaseDatabase.DefaultInstance
                                            .GetReference(pathToSharedData);
        controlChangeData.ChildChanged += HandleChildChanged;
        controlChangeData.ChildAdded += HandleChildChanged;
        FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + playerData.UserId + "/waitForShare/").Child(requestKey).RemoveValueAsync();
    }
    public void MissedShareScreen(string requestKey)
    {
        infoAboutShare.enabled = false;
        FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + playerData.UserId + "/waitForShare/").Child(requestKey).RemoveValueAsync();
    }

    #region HandleUsers
    public void HandleShowAllUserInClassAdd(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string key = args.Snapshot.Key;
        string value = args.Snapshot.Value.ToString();

        FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + key)
                        .GetValueAsync().ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                Debug.Log("skontroluj pripojenie");
                                SceneManager.LoadScene("CheckConnection");
                            }
                            else if (task.IsCompleted)
                            {
                                DataSnapshot snapshot = task.Result;
                                if (snapshot.Child("selectClass").Value.Equals(playerData.SelectedClass))
                                    generateStudentToogleList(key, new Vector3(-1.5f, 0, 0), value);
                            }
                        });
    }

    public void HandleShowAllUserInClassRemove(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string key = args.Snapshot.Key;
        Destroy(ListStudent[key]);
        ListStudent.Remove(key);

    }

    public void HandleControlRequestSharedScreen(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string key = args.Snapshot.Key;
        string adminName = args.Snapshot.Child("admin_name").Value.ToString();
        string screenKey = args.Snapshot.Child("share_object").Value.ToString();
        infoAboutShare.enabled = true;
        Text text = GameObject.Find("txtRequestFrom").GetComponent<Text>();
        text.text = adminName + " stebou zdieľa úlohu.";
        Button btnSuhlas = GameObject.Find("btnSuhlas").GetComponent<Button>();
        btnSuhlas.onClick.AddListener(delegate
        {
            AcceptShareScreen(screenKey, key);
        });
        Button btnNesuhlas = GameObject.Find("btnNesuhlas").GetComponent<Button>();
        btnNesuhlas.onClick.AddListener(delegate
        {
            MissedShareScreen(key);
        });
    }

    public void HandleChildChanged(object sender, ChildChangedEventArgs args)
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
        HasChanged(false);
    }

    public void UnbindAllHandler()
    {
        controlSharedScreenWithMe.ChildAdded -= HandleControlRequestSharedScreen;
        controlAllStudentInClass.ChildAdded -= HandleShowAllUserInClassAdd;
        controlAllStudentInClass.ChildRemoved -= HandleShowAllUserInClassRemove;
        if (useButtonShareSchreenWith)
            controlChangeData.ChildChanged -= HandleChildChanged;
    }
    #endregion


    public void CreateArrayExam()
    {
        ExamArray = new Dictionary<string, string>();
        foreach (Transform objekt in GameObject.Find("Panel1").gameObject.transform)
        {
            foreach (Transform slot in objekt)
            {
                if (!slot.name.Contains("SlotM"))
                {
                    DajFlagStatickymHodnotamVtrojuholniku(slot, FLAG_DISABLED);
                }
                else
                {
                    DajFlagStatickymHodnotamVtrojuholniku(slot);
                }
            }
        }
        _fbc.UpdateResult(ExamArray, _pathWay);
    }

    public void DajFlagStatickymHodnotamVtrojuholniku(Transform slot, string flag_disabled = "")
    {
        GameObject item = slot.GetComponent<Slot>().item;
        if (slot.name.Equals(SLOT + priklad.pom_suc0) || slot.name.Equals(SLOT + priklad.pom_suc1))
        {
            DajFlagSlotomSpodmienkou(slot, item, FLAG_PODMIENKA, flag_disabled);

        }
        else
        {
            DajFlagSlotomSpodmienkou(slot, item, "", flag_disabled);
        }

    }

    public void DajFlagSlotomSpodmienkou(Transform slot, GameObject item, string flag_slot = "", string flag_disabled = "")
    {
        if (item)
        {
            ExamArray[slot.name + flag_slot] = item.name.Substring(0, item.name.IndexOf("(")) + flag_disabled;
        }
        else
            ExamArray.Add(slot.name + flag_slot, "null");
    }

    public void HasChanged(bool zaznamenajDoDB = true)
    {
        // metoda kontroluje ci nenastali zmeny v slotoch
        List<int> kontrola = new List<int> { };
        foreach (Transform objekt in GameObject.Find("Panel1").gameObject.transform)
        {
            foreach (Transform slot in objekt)
            {
                if (!slot.name.Contains("SlotM"))
                {
                    GameObject item = slot.GetComponent<Slot>().item;
                    if (item)
                    {
                        kontrola.Add(int.Parse(item.name.Substring(0, item.name.IndexOf("("))));
                    }
                }
                if (!_poliaKtoreSaNevykreslia.Contains(slot.name))
                    continue;
                if (slot.GetComponent<Slot>().item == null)
                {
                    if (slot.name.Equals(SLOT + pom_suc0) || slot.name.Equals(SLOT + pom_suc1))
                    {
                        ExamArray[slot.name + FLAG_PODMIENKA] = "null";
                    }
                    else
                    {
                        ExamArray[slot.name] = "null";
                    }
                }
                else
                {
                    if (slot.name.Equals(SLOT + pom_suc0) || slot.name.Equals(SLOT + pom_suc1))
                    {
                        ExamArray[slot.name + FLAG_PODMIENKA] = slot.GetComponent<Slot>().item.name.Substring(0, slot.GetComponent<Slot>().item.name.IndexOf("("));
                    }
                    else
                    {
                        ExamArray[slot.name] = slot.GetComponent<Slot>().item.name.Substring(0, slot.GetComponent<Slot>().item.name.IndexOf("("));
                    }
                }
            }
        }
        _fbc.UpdateResult(ExamArray, _pathWay);
        if (kontrola.Count == 10)
        {
            pom_suc0 = priklad.reverse(kontrola)[pom_suc0];
            pom_suc1 = priklad.reverse(kontrola)[pom_suc1];
            pom_suc0 = kontrola.FindIndex(x => x == pom_suc0);
            pom_suc1 = kontrola.FindIndex(x => x == pom_suc1);

            bool pom = skontroluj.Vyhodnot(kontrola, pom_suc1, pom_suc0, 45);
            if (pom)
            {
                congrats_show();
            }
            else
            {
                nespravne_show();
            }
        }
    }

    void generator_uloh()
    {
        priklad = new Generator_uloh(lvl);
        table_M = new Dictionary<string, int?> { };
        _poliaKtoreSaNevykreslia = new List<string> { };
        List<string> pozicia = new List<string> { };
        table = priklad.get_array(10);
        pom_suc0 = table[priklad.pom_suc0];
        pom_suc1 = table[priklad.pom_suc1];
        pom_suc0 = table.FindIndex(x => x == pom_suc0);
        pom_suc1 = table.FindIndex(x => x == pom_suc1);
        _poliaKtoreSaNevykreslia.Add(SLOT + pom_suc0);
        _poliaKtoreSaNevykreslia.Add(SLOT + pom_suc1);


        int pocitadloSlotov = 0;
        foreach (var num in table)
        {
            table_M.Add(SLOT + pocitadloSlotov, num);
            pocitadloSlotov++;
        }

        while (_poliaKtoreSaNevykreslia.Count < 5)
        {
            int x = UnityEngine.Random.Range(0, 10);
            if (!_poliaKtoreSaNevykreslia.Contains(SLOT + x))
                _poliaKtoreSaNevykreslia.Add(SLOT + x);
        }

        while (pozicia.Count < 5)
        {
            int y = UnityEngine.Random.Range(0, 14);
            if (!pozicia.Contains(SLOTM + y))
                pozicia.Add(SLOTM + y);
        }

        List<int> moznosti = new List<int> { };
        while (moznosti.Count < 14)
        {
            int x = UnityEngine.Random.Range(1, 101);
            if (moznosti.Contains(x) == false && x != table_M[_poliaKtoreSaNevykreslia[0]] && x != table_M[_poliaKtoreSaNevykreslia[1]] && x != table_M[_poliaKtoreSaNevykreslia[2]] && x != table_M[_poliaKtoreSaNevykreslia[3]] && x != table_M[_poliaKtoreSaNevykreslia[4]])
            {
                moznosti.Add(x);
            }
        }

        pocitadloSlotov = 0;
        foreach (var moznost in moznosti)
        {
            table_M.Add(SLOTM + pocitadloSlotov, moznost);
            pocitadloSlotov++;
        }

        table_M[pozicia[0]] = table_M[_poliaKtoreSaNevykreslia[0]];
        table_M[pozicia[1]] = table_M[_poliaKtoreSaNevykreslia[1]];
        table_M[pozicia[2]] = table_M[_poliaKtoreSaNevykreslia[2]];
        table_M[pozicia[3]] = table_M[_poliaKtoreSaNevykreslia[3]];
        table_M[pozicia[4]] = table_M[_poliaKtoreSaNevykreslia[4]];
    }

    public void Destroy()
    {
        int y = 1;
        for (int i = 0; i < GameObject.Find("Panel1").gameObject.transform.childCount - 3; i++)
        {
            for (int j = 0; j < GameObject.Find(NAZOV_PANELOV + y).gameObject.transform.childCount; j++)
            {

                var pom = GameObject.Find(NAZOV_PANELOV + y).gameObject.transform.GetChild(j);
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

    public void ChangeCol(int col, bool f)
    {
        GameObject slotName = GameObject.Find(SLOT + col);
        Image change = slotName.GetComponent<Image>();
        if (f)
        {
            change.color = new Color(0, 0, 255, 0.61f);
        }
        else
        {
            change.color = new Color(255, 255, 255, 0.61f);
        }
    }

    public void draw()
    {
        // metoda vykresli vygenerovane riesenie do prazdnych slotov
        slots = new GameObject[24];
        int y = 1;
        int poc = 0;
        for (int i = 0; i < GameObject.Find("Panel1").gameObject.transform.childCount - 4; i++)
        {
            for (int j = 0; j < GameObject.Find(NAZOV_PANELOV + y).gameObject.transform.childCount; j++)
            {
                slots[j] = GameObject.Find(NAZOV_PANELOV + y).gameObject.transform.GetChild(j).gameObject;
                if (!_poliaKtoreSaNevykreslia.Contains(slots[j].name))
                {
                    GameObject newItem = Instantiate(itemPrefab[(int)table_M[slots[j].name]]) as GameObject;
                    newItem.transform.parent = slots[j].transform;
                    newItem.transform.localScale = new Vector3(1, 1, 1);
                    if (poc < 10)
                    {   // nastavy aby sa hodnoty ktore su nazaciatku umiestnene nedali presuvat
                        newItem.GetComponent<DragHandeler>().enabled = false;
                    }
                    ChangeCol(pom_suc0, true);
                    ChangeCol(pom_suc1, true);
                }
                poc++;
            }
            y++;
        }
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

    IEnumerator congrats_hide()
    {
        yield return new WaitForSeconds(2.0f);

        gratulation.enabled = false;
        SceneManager.LoadScene(UnityEngine.Random.Range(8, 10));
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);

        nespravne.enabled = false;
        ChangeCol(pom_suc0, false);
        ChangeCol(pom_suc1, false);
        generator_uloh();
        Restart();

    }

    #region ToogleList
    private void generateStudentToogleList(string ObjName, Vector3 vector, string txtName)
    {

        makeRow(GameObject.Find("Content"), ObjName, vector);
        makeTogglePrefabs(GameObject.Find(ObjName), txtName);

    }
    private void makeRow(GameObject cnvs, string objName, Vector3 vector)
    {
        GameObject rowObj = createRowObj(cnvs, objName);
        CanvasRenderer renderer = rowObj.AddComponent<CanvasRenderer>();

        GridLayoutGroup grid = rowObj.AddComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 1;
        grid.cellSize = new Vector2(780, 40);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.spacing = new Vector2(12, 0);

        grid.transform.localScale = new Vector3(1, 1, 1);
        grid.GetComponent<RectTransform>().localPosition = vector;
        grid.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        grid.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        grid.GetComponent<RectTransform>().sizeDelta = new Vector2(820, 60);
        ListStudent[objName] = rowObj;
    }
    GameObject createRowObj(GameObject cnvs, string objName)
    {
        var panelM = new GameObject(objName);
        panelM.transform.SetParent(cnvs.transform);
        panelM.layer = LayerMask.NameToLayer("UI");
        return panelM;
    }
    private void makeTogglePrefabs(GameObject ObjName, string txtName)
    {
        GameObject newItem = Instantiate(togglePrefab) as GameObject;
        newItem.transform.SetParent(ObjName.transform);
        newItem.layer = LayerMask.NameToLayer("UI");
        newItem.name = "Toggle";
        newItem.transform.GetChild(1).GetComponent<Text>().text = txtName;
        newItem.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

    }
    #endregion
}
