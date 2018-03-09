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

public class LoggedLevel_1_2 : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
{
    #region inicializacne Data
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
    public GameObject togglePrefab;

    private Kontrola _skontroluj;
    private Dictionary<string, GameObject> _listStudent = new Dictionary<string, GameObject>();
    private PlayerData _playerData = new PlayerData();
    private FirebaseCommunicationLibrary _fbc;
    private Dictionary<string, string> _examArray;
    private string _pathToSharedData;
    private string _pathActualPlayerScreen;
    private string _pathActualPlayerScreenDate;
    private bool _useButtonShareSchreenWith = false;
    private DatabaseReference _controlChangeData;
    private DatabaseReference _controlSharedScreenWithMe;
    private DatabaseReference _controlAllStudentInClass;
    private Dictionary<string, int?> table_M = new Dictionary<string, int?>();  // - zadany priklad + moznosti
    private List<string> _poliaKtoreSaNevykreslia = new List<string>();                        //- ktore policko sa vynecha a nevykresli
    private Generator_uloh _priklad;
    private List<int> _table;
    #endregion

    private void Awake()
    {
        Button btnRestart = GameObject.Find("Reset").GetComponent<Button>();
        btnRestart.onClick.AddListener(delegate
        {
            Restart();
        });
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
        _playerData = GlobalData.playerData;

#if DEBUG
        _playerData.Name = "TestLingo";
        _playerData.UserId = "ZAT4DktlgdYBVGwXYRpOfA3temm1";
        _playerData.SelectedClass = "-KweS-rI-bTBYVX3g3vS";
        _playerData.LoggedUser = true;
#endif
        _pathActualPlayerScreen = string.Format("/USERS/{0}/ACTUAL_SCREEN/SCREEN", _playerData.UserId);
        _pathActualPlayerScreenDate = string.Format("/USERS/{0}/ACTUAL_SCREEN/DATE", _playerData.UserId);

        _controlAllStudentInClass = FirebaseDatabase.DefaultInstance
                                                   .GetReference("/CLASSES/" + _playerData.SelectedClass + "/ONLINE_STUDENTS");
        _controlAllStudentInClass.ChildAdded += HandleShowAllUserInClassAdd;
        _controlAllStudentInClass.ChildRemoved += HandleShowAllUserInClassRemove;

        _controlSharedScreenWithMe = FirebaseDatabase.DefaultInstance
                                                        .GetReference("/USERS/" + _playerData.UserId + "/waitForShare");
        _controlSharedScreenWithMe.ChildAdded += HandleControlRequestSharedScreen;

        _skontroluj = new Kontrola(2);
        generator_uloh();

        draw();


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

    #region zdielanie
    public void ShareScreenWith()
    {
        SharedScreen screen = new SharedScreen()
        {
            screeen_locker = false,
            admin_name = _playerData.Name,
            screen_name = "screen1a",
        };
        String key = FirebaseDatabase.DefaultInstance.GetReference("/SHARED_SCREEN").Push().Key;
        LeaderBoardEntry entry = new LeaderBoardEntry(_examArray);
        Dictionary<string, object> entryValues = entry.ToDictionary();
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        _pathToSharedData = "/SHARED_SCREEN/" + key + "/data/";
        childUpdates[_pathToSharedData] = entryValues;
        _fbc.addSharedScreen(key, screen);
        _fbc.inserMyIdToSharedScreen(_playerData.UserId, _playerData.Name, key);
        FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(childUpdates);

        List<string> selectedStudent = new List<string>();
        foreach (var student in _listStudent)
        {
            if (student.Value.transform.GetChild(0).GetComponent<Toggle>().isOn)
            {
                selectedStudent.Add(student.Key);
            }
        }
        SendRequest(key, selectedStudent);

        _controlChangeData = FirebaseDatabase.DefaultInstance
                                            .GetReference("/SHARED_SCREEN/" + key + "/data/");
        _controlChangeData.ChildChanged += HandleChildChanged;
        showSharedWith.enabled = false;
        DeselectAllUser();

        _useButtonShareSchreenWith = true;
    }

    public void SendRequest(string Screeen_key, List<string> zoznamLudi)
    {
        foreach (var user in zoznamLudi)
        {
            SharedScreenRequest request = new SharedScreenRequest()
            {
                admin_name = _playerData.Name,
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
            foreach (var student in _listStudent)
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
        foreach (var student in _listStudent)
        {
            student.Value.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
        }
    }

    public void AcceptShareScreen(string screenKey, string requestKey)
    {
        _pathToSharedData = "/SHARED_SCREEN/" + screenKey + "/data/";
        infoAboutShare.enabled = false;
        _useButtonShareSchreenWith = true;
        Console.WriteLine("AcceptShareScren id={0} name={1} screenKey={2}", _playerData.UserId, _playerData.Name, screenKey);
        _fbc.inserMyIdToSharedScreen(_playerData.UserId, _playerData.Name, screenKey);
        _controlChangeData = FirebaseDatabase.DefaultInstance
                                            .GetReference(_pathToSharedData);
        _controlChangeData.ChildChanged += HandleChildChanged;
        _controlChangeData.ChildAdded += HandleChildChanged;
        FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + _playerData.UserId + "/waitForShare/").Child(requestKey).RemoveValueAsync();
    }
    public void MissedShareScreen(string requestKey)
    {
        infoAboutShare.enabled = false;
        FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + _playerData.UserId + "/waitForShare/").Child(requestKey).RemoveValueAsync();
    }
    #endregion

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
                                if (snapshot.Child("selectClass").Value.Equals(_playerData.SelectedClass))
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
        Destroy(_listStudent[key]);
        _listStudent.Remove(key);

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
        _examArray[key] = value.ToString();
        CleanScreen();
        draw();
        HasChanged(false);
    }

    public void UnbindAllHandler()
    {
        _controlSharedScreenWithMe.ChildAdded -= HandleControlRequestSharedScreen;
        _controlAllStudentInClass.ChildAdded -= HandleShowAllUserInClassAdd;
        _controlAllStudentInClass.ChildRemoved -= HandleShowAllUserInClassRemove;
        if (_useButtonShareSchreenWith)
            _controlChangeData.ChildChanged -= HandleChildChanged;
    }
    #endregion
    #region game
    public void CreateArrayExam()
    {
        _examArray = new Dictionary<string, string>();
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
        _fbc.UpdateResult(_examArray, _pathActualPlayerScreen);
    }

    public void DajFlagStatickymHodnotamVtrojuholniku(Transform slot, string flag_disabled = "")
    {
        GameObject item = slot.GetComponent<Slot>().item;
        if (item)
        {
            _examArray[slot.name] = item.name.Substring(0, item.name.IndexOf("(")) + flag_disabled;
        }
        else
            _examArray.Add(slot.name, "null");
    }

    public void CleanScreen()
    {
        int y = 1;
        for (int i = 0; i < GameObject.Find("Panel1").gameObject.transform.childCount - 4; i++)
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
        CleanScreen();
        draw();
    }

    public void draw()
    {
        // metoda vykresli vygenerovane riesenie do prazdnych slotov
        GameObject[] _slots = new GameObject[7];
        int y = 1;
        int poc = 0;
        for (int i = 0; i < GameObject.Find("Panel1").gameObject.transform.childCount - 4; i++)
        {
            for (int j = 0; j < GameObject.Find(NAZOV_PANELOV + y).gameObject.transform.childCount; j++)
            {
                _slots[j] = GameObject.Find(NAZOV_PANELOV + y).gameObject.transform.GetChild(j).gameObject;
                if (!_poliaKtoreSaNevykreslia.Contains(_slots[j].name))
                {
                    GameObject newItem = Instantiate(itemPrefab[(int)table_M[_slots[j].name]]) as GameObject;
                    newItem.transform.parent = _slots[j].transform;
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
        CreateArrayExam();
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

                if (slot.GetComponent<Slot>().item == null)
                {
                    _examArray[slot.name] = "null";
                }
                else
                {
                    _examArray[slot.name] = slot.GetComponent<Slot>().item.name.Substring(0, slot.GetComponent<Slot>().item.name.IndexOf("("));
                }
            }
        }
        _fbc.UpdateResult(_examArray, _pathActualPlayerScreen);
        _fbc.zapisDatumActualScreen(_pathActualPlayerScreenDate);
        if (kontrola.Count == 3)
        {
            bool pom = _skontroluj.Vyhodnot(kontrola);
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
        _priklad = new Generator_uloh(1);
        table_M = new Dictionary<string, int?> { };
        _poliaKtoreSaNevykreslia = new List<string> { };
        List<string> pozicia = new List<string> { };
        _table = _priklad.get_array(3);

        int pocitadloSlotov = 0;
        foreach (var num in _table)
        {
            table_M.Add(SLOT + pocitadloSlotov, num);
            pocitadloSlotov++;
        }

        while (_poliaKtoreSaNevykreslia.Count < 2)
        {
            int x = UnityEngine.Random.Range(0, 3);
            if (!_poliaKtoreSaNevykreslia.Contains(SLOT + x))
                _poliaKtoreSaNevykreslia.Add(SLOT + x);
        }

        while (pozicia.Count < 2)
        {
            int y = UnityEngine.Random.Range(0, 4);
            if (!pozicia.Contains(SLOTM + y))
                pozicia.Add(SLOTM + y);
        }

        List<int> moznosti = new List<int> { };
        while (moznosti.Count < 4)
        {
            int x = UnityEngine.Random.Range(1, 10);
            if (moznosti.Contains(x) == false && x != table_M[_poliaKtoreSaNevykreslia[0]] && x != table_M[_poliaKtoreSaNevykreslia[1]])
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
        UnbindAllHandler();
        gratulation.enabled = false;
        SceneManager.LoadScene(UnityEngine.Random.Range(21, 25));
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);
        nespravne.enabled = false;
    }
    #endregion
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
        _listStudent[objName] = rowObj;
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