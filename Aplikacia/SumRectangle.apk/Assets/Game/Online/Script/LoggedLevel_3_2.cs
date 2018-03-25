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

public class LoggedLevel_3_2 : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
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
    public Canvas infoAboutPinExamToTable;
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
    private List<string> _poliaKtoreSaNevykreslia = new List<string>();                        //- ktore policko sa vynecha a nevykresli
    private Generator_uloh _priklad;
    private List<int> _table;
    private int _pomSuc0 = -1;
    private int _pomSuc1 = -1;
    private List<string> _poliaOznaceneDisable;

    private string _keyPinnedExam = null;
    private Dictionary<string, string> _zalohaExamArray;
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
        Button btnPinToTable = GameObject.Find("PinToTable").GetComponent<Button>();
        btnPinToTable.onClick.AddListener(delegate
        {
            PripniPrikladNaTabulu();
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
        _playerData.SelectedClass = "-L7EL2Ny6sBTqGX_kZtU";
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

        _skontroluj = new Kontrola(3);
        generator_uloh();

        draw();
        _zalohaExamArray = _examArray.ToDictionary(x => x.Key, x => x.Value);
        gratulation = gratulation.GetComponent<Canvas>();
        gratulation.enabled = false;
        nespravne = nespravne.GetComponent<Canvas>();
        nespravne.enabled = false;
        infoAboutShare = infoAboutShare.GetComponent<Canvas>();
        infoAboutShare.enabled = false;
        infoAboutPinExamToTable = infoAboutPinExamToTable.GetComponent<Canvas>();
        infoAboutPinExamToTable.enabled = false;
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
            screen_name = "LogLvl2_2",
            pomSucet0and1 = new List<int>() { _pomSuc0, _pomSuc1 },
            poliaOznaceneDisable = _poliaOznaceneDisable,
            poliaKtoreSaNevykreslia = _poliaKtoreSaNevykreslia
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
        GlobalData.playerData.cestaKZdielanymDatam = "/SHARED_SCREEN/" + screenKey + "/data/";
        _fbc.inserMyIdToSharedScreen(_playerData.UserId, _playerData.Name, screenKey);
        FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + _playerData.UserId + "/waitForShare/").Child(requestKey).RemoveValueAsync();
        FirebaseDatabase.DefaultInstance.GetReference("/SHARED_SCREEN/" + screenKey + "/").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snap = task.Result;
                GlobalData.playerData.zdielaneDataAll = snap;
                string nameScene = snap.Child("screen_name").Value.ToString();
                UnbindAllHandler();
                SceneManager.LoadScene(nameScene);
            }
        });
    }
    public void MissedShareScreen(string requestKey)
    {
        infoAboutShare.enabled = false;
        FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + _playerData.UserId + "/waitForShare/").Child(requestKey).RemoveValueAsync();
    }

    public void PripniPrikladNaTabulu()
    {
        if (_keyPinnedExam == null)
        {
            var nazov = "Level3 - " + DateTime.Now.ToString("d.M HH:m:s");
            SharedScreen screen = new SharedScreen()
            {
                screeen_locker = false,
                admin_name = _playerData.Name,
                nazov_ulohy = nazov,
                screen_name = "LogLvl2_2",
                pomSucet0and1 = new List<int>() { _pomSuc0, _pomSuc1 },
                poliaOznaceneDisable = _poliaOznaceneDisable,
                poliaKtoreSaNevykreslia = _poliaKtoreSaNevykreslia
            };
            _keyPinnedExam = FirebaseDatabase.DefaultInstance.GetReference("/EXAMS_PINNED_TO_TABLE").Push().Key;
            _fbc.addPinExamToTable(_playerData.SelectedClass, _keyPinnedExam);
            _fbc.addPinnedToTable(_keyPinnedExam, screen);
            Text text = GameObject.Find("InfoOpripnutiUlohyText").GetComponent<Text>();
            text.text = "Úloha bola pridaná na tabuľu pod názvom " + nazov;
            InfoAboutPin();
        }
        else
        {
            Text text = GameObject.Find("InfoOpripnutiUlohyText").GetComponent<Text>();
            text.text = "Úloha bola aktualizovaná";
            InfoAboutPin();
        }
        LeaderBoardEntry entry = new LeaderBoardEntry(_examArray);
        Dictionary<string, object> entryValues = entry.ToDictionary();
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        string _pathToPinnedExam = "/EXAMS_PINNED_TO_TABLE/" + _keyPinnedExam + "/data/";
        childUpdates[_pathToPinnedExam] = entryValues;
        FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(childUpdates);
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
                                UnbindAllHandler();
                                SceneManager.LoadScene("CheckConnection");
                            }
                            else if (task.IsCompleted)
                            {
                                DataSnapshot snapshot = task.Result;
                                if (snapshot.Child("selectClass").Value.Equals(_playerData.SelectedClass) && key != _playerData.UserId)
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
        foreach (var zaloha in _zalohaExamArray)
        {
            _examArray[zaloha.Key] = zaloha.Value;
        }
        draw();
        HasChanged();

    }

    public void draw()
    {
        //create a new item, name it, and set the parent
        foreach (Transform objekt in GameObject.Find("Panel1").gameObject.transform)
        {
            foreach (Transform slot in objekt)
            {
                if (_examArray[slot.name] != "null")
                {
                    GameObject newItem = Instantiate(itemPrefab[int.Parse(_examArray[slot.name])]) as GameObject;
                    newItem.transform.parent = slot.transform;
                    newItem.transform.localScale = new Vector3(1, 1, 1);
                    if (_poliaOznaceneDisable.Contains(slot.name))
                    {   // nastavy aby sa hodnoty ktore su nazaciatku umiestnene nedali presuvat
                        newItem.GetComponent<DragHandeler>().enabled = false;
                    }
                }
            }
        }
        if (_pomSuc0 != -1 && _pomSuc1 != -1)
        {
            ChangeCol(_pomSuc0, true);
            ChangeCol(_pomSuc1, true);
        }
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
        if (_useButtonShareSchreenWith && zaznamenajDoDB)
        {
            _fbc.UpdateResult(_examArray, _pathToSharedData);
        }
        if (kontrola.Count == 6)
        {
            int _pomSuc0Hodnota = 0;
            int _pomSuc1Hodnota = 0;
            if (_pomSuc0 != -1 && _pomSuc1 != -1)
            {
                _pomSuc0Hodnota = reverse(kontrola)[_pomSuc0];
                _pomSuc1Hodnota = reverse(kontrola)[_pomSuc1];
                _pomSuc0 = kontrola.FindIndex(x => x == _pomSuc0Hodnota);
                _pomSuc1 = kontrola.FindIndex(x => x == _pomSuc1Hodnota);
            }
            bool pom = _pomSuc0 == -1 && _pomSuc1 == -1 ? _skontroluj.Vyhodnot(kontrola) : _skontroluj.Vyhodnot(kontrola, _pomSuc1, _pomSuc0, _pomSuc0Hodnota + _pomSuc1Hodnota);
            if (pom)
            {
                if (_keyPinnedExam != null)
                {
                    _fbc.zapisStavRozriesenejUlohy(_playerData.UserId, _keyPinnedExam, "1");
                }
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
        _priklad = new Generator_uloh(3);
        _examArray = new Dictionary<string, string>();
        _poliaKtoreSaNevykreslia = new List<string>();
        List<string> pozicia = new List<string>();
        _table = _priklad.get_array(6);
        _pomSuc0 = _table[_priklad.pom_suc0];
        _pomSuc1 = _table[_priklad.pom_suc1];
        _pomSuc0 = _table.FindIndex(x => x == _pomSuc0);
        _pomSuc1 = _table.FindIndex(x => x == _pomSuc1);
        _poliaKtoreSaNevykreslia.Add(SLOT + _pomSuc0);
        _poliaKtoreSaNevykreslia.Add(SLOT + _pomSuc1);

        int pocitadloSlotov = 0;
        foreach (var num in _table)
        {
            _examArray.Add(SLOT + pocitadloSlotov, num.ToString());
            pocitadloSlotov++;
        }

        while (_poliaKtoreSaNevykreslia.Count < 3)
        {
            int x = UnityEngine.Random.Range(0, 6);
            if (!_poliaKtoreSaNevykreslia.Contains(SLOT + x))
                _poliaKtoreSaNevykreslia.Add(SLOT + x);
        }

        _poliaOznaceneDisable = new List<string>();
        for (int i = 0; i < 6; i++)
        {
            if (!_poliaKtoreSaNevykreslia.Contains(SLOT + i))
            {
                _poliaOznaceneDisable.Add(SLOT + i);
            }
        }

        while (pozicia.Count < 3)
        {
            int y = UnityEngine.Random.Range(0, 7);
            if (!pozicia.Contains(SLOTM + y))
                pozicia.Add(SLOTM + y);
        }

        List<int> moznosti = new List<int> { };
        while (moznosti.Count < 7)
        {
            int x = UnityEngine.Random.Range(1, 65);
            if (moznosti.Contains(x) == false && x != int.Parse(_examArray[_poliaKtoreSaNevykreslia[0]]) && x != int.Parse(_examArray[_poliaKtoreSaNevykreslia[1]]) && x != int.Parse(_examArray[_poliaKtoreSaNevykreslia[2]]))
            {
                moznosti.Add(x);
            }
        }

        pocitadloSlotov = 0;
        foreach (var moznost in moznosti)
        {
            _examArray.Add(SLOTM + pocitadloSlotov, moznost.ToString());
            pocitadloSlotov++;
        }

        _examArray[pozicia[0]] = _examArray[_poliaKtoreSaNevykreslia[0]];
        _examArray[pozicia[1]] = _examArray[_poliaKtoreSaNevykreslia[1]];
        _examArray[pozicia[2]] = _examArray[_poliaKtoreSaNevykreslia[2]];

        foreach (var p in _poliaKtoreSaNevykreslia)
        {
            _examArray[p] = "null";
        }
    }

    public List<int> reverse(List<int> x)
    {
        List<int> pom = new List<int> { };
        for (int i = (x.Count - 1); i >= 0; i--)
        {
            pom.Add(x[i]);
        }
        return pom;
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

    public void InfoAboutPin()
    {
        //zobrazi oznam a zavola metodu na skrytie
        infoAboutPinExamToTable.enabled = true;
        StartCoroutine(InfoAboutPinHide());
    }

    IEnumerator congrats_hide()
    {
        yield return new WaitForSeconds(2.0f);
        UnbindAllHandler();
        gratulation.enabled = false;
        SceneManager.LoadScene(UnityEngine.Random.Range(25, 29));
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);
        nespravne.enabled = false;
    }

    IEnumerator InfoAboutPinHide()
    {
        yield return new WaitForSeconds(2.0f);
        infoAboutPinExamToTable.enabled = false;
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
        newItem.transform.GetChild(1).GetComponent<Text>().color = Color.black;
        newItem.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

    }
    #endregion
}