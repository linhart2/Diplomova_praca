using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UIAddons;
using Firebase.Database;
using UnityEngine.SceneManagement;
using System.Linq;

public class LogLvlUniversal : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
{
    private const string NAZOV_PANELOV = "Panel1_";
    private const string SLOT = "Slot_";
    private const string SLOTM = "SlotM_";
    private const string FLAG_PODMIENKA = "-PODMIENKA";
    private const string FLAG_DISABLED = "-DISABLED";

    public Canvas gratulation;      //- object gratulacia
    public Canvas nespravne;        //- object nespravne
    public Canvas showSharedWith;      //- object zdielat s....
    public Canvas infoAboutShare;
    public GameObject[] itemPrefab;
    public GameObject togglePrefab;
    public int vysledokPomocnehoSuctu;
    public int urovenVstupPreKontrolu;


    private Kontrola _skontroluj;
    private Dictionary<string, GameObject> _listStudent = new Dictionary<string, GameObject>();
    private PlayerData _playerData = new PlayerData();
    private FirebaseCommunicationLibrary _fbc;
    private Dictionary<string, string> _examArray;
    private List<int> _zoznamCiselPrikladu = new List<int>();
    private string pathToSharedData;
    private string _pathActualPlayerScreen;
    private string _pathActualPlayerScreenDate;
    private int _pomSuc0 = -1;
    private int _pomSuc1 = -1;
    private bool _useButtonShareSchreenWith = false;
    private DatabaseReference _controlChangeData;
    private DatabaseReference _controlSharedScreenWithMe;
    private DatabaseReference _controlAllStudentInClass;
    private List<string> _poliaKtoreSaNevykreslia = new List<string>();                        //- ktore policko sa vynecha a nevykresli
    private int _pocetPoliKtoreSaKontroluju;
    private List<string> _poliaOznaceneDisable = new List<string>();
    private List<string> _podmienka = new List<string>();



    // Use this for initialization
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

    void Start()
    {
        switch (urovenVstupPreKontrolu)
        {
            case 2:
                _pocetPoliKtoreSaKontroluju = 3;
                break;
            case 3:
                _pocetPoliKtoreSaKontroluju = 6;
                break;
            case 4:
                _pocetPoliKtoreSaKontroluju = 10;
                break;
        }
        _playerData = GlobalData.playerData;

#if DEBUG
        _playerData.Name = "TestLingo";
        _playerData.UserId = "ZAT4DktlgdYBVGwXYRpOfA3temm1";
        _playerData.SelectedClass = "-L7EL2Ny6sBTqGX_kZtU";
        _playerData.LoggedUser = true;
#endif
        _pathActualPlayerScreen = string.Format("/USERS/{0}/ACTUAL_SCREEN/SCREEN", _playerData.UserId);
        _pathActualPlayerScreenDate = string.Format("/USERS/{0}/ACTUAL_SCREEN/DATE", _playerData.UserId);

        pathToSharedData = _playerData.cestaKZdielanymDatam;
        _playerData.cestaKZdielanymDatam = null;
        int rozlisTypUlohy = _playerData.selectedExamOnBoard != null ? 0 : 1;
        switch (rozlisTypUlohy)
        {
            case 0:
                _examArray = _playerData.selectedExamOnBoard.First().Children.ToDictionary(t => string.Format("{0}{1}", SLOTM, t.Key.Substring(t.Key.IndexOf('_') + 1)), t => t.Value.ToString());
                doplnChybajuceData(_examArray);
                break;
            case 1:
                _examArray = _playerData.zdielaneDataAll.Child("data").Children.ToDictionary(t => t.Key, t => t.Value.ToString());
                _pomSuc0 = int.Parse(_playerData.zdielaneDataAll.Child("pomSucet0and1").Child("0").Value.ToString());
                _pomSuc1 = int.Parse(_playerData.zdielaneDataAll.Child("pomSucet0and1").Child("1").Value.ToString());
                _poliaOznaceneDisable = _playerData.zdielaneDataAll.Child("poliaOznaceneDisable").Children.Select(val => val.Value.ToString()).ToList();
                _poliaKtoreSaNevykreslia = _playerData.zdielaneDataAll.Child("poliaOznaceneDisable").Children.Select(val => val.Value.ToString()).ToList();
                break;
            default:
                break;
        }

        if (pathToSharedData != null && pathToSharedData != string.Empty)
        {
            _useButtonShareSchreenWith = true;
            _controlChangeData = FirebaseDatabase.DefaultInstance
                                            .GetReference(pathToSharedData);
            _controlChangeData.ChildChanged += HandleChildChanged;
            _controlChangeData.ChildAdded += HandleChildChanged;
        }

        _controlAllStudentInClass = FirebaseDatabase.DefaultInstance
                                                   .GetReference("/CLASSES/" + _playerData.SelectedClass + "/ONLINE_STUDENTS");
        _controlAllStudentInClass.ChildAdded += HandleShowAllUserInClassAdd;
        _controlAllStudentInClass.ChildRemoved += HandleShowAllUserInClassRemove;

        _controlSharedScreenWithMe = FirebaseDatabase.DefaultInstance
                                                        .GetReference("/USERS/" + _playerData.UserId + "/waitForShare");
        _controlSharedScreenWithMe.ChildAdded += HandleControlRequestSharedScreen;

        gratulation = gratulation.GetComponent<Canvas>();
        infoAboutShare = infoAboutShare.GetComponent<Canvas>();
        showSharedWith = showSharedWith.GetComponent<Canvas>();
        nespravne = nespravne.GetComponent<Canvas>();

        _skontroluj = new Kontrola(urovenVstupPreKontrolu);

        draw();

        gratulation.enabled = false;
        showSharedWith.enabled = false;
        nespravne.enabled = false;
        infoAboutShare.enabled = false;

        HasChanged();
    }
    public void ShareScreenWith()
    {
        SharedScreen screen = new SharedScreen()
        {
            screeen_locker = false,
            admin_name = _playerData.Name,
            screen_name = "LogLvl4_2",
        };
        String key = FirebaseDatabase.DefaultInstance.GetReference("/SHARED_SCREEN").Push().Key;
        LeaderBoardEntry entry = new LeaderBoardEntry(_examArray);
        Dictionary<string, object> entryValues = entry.ToDictionary();
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        pathToSharedData = "/SHARED_SCREEN/" + key + "/data/";
        childUpdates[pathToSharedData] = entryValues;
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
        pathToSharedData = "/SHARED_SCREEN/" + screenKey + "/data/";
        infoAboutShare.enabled = false;
        _useButtonShareSchreenWith = true;
        Console.WriteLine("AcceptShareScren id={0} name={1} screenKey={2}", _playerData.UserId, _playerData.Name, screenKey);
        _fbc.inserMyIdToSharedScreen(_playerData.UserId, _playerData.Name, screenKey);
        _controlChangeData = FirebaseDatabase.DefaultInstance
                                            .GetReference(pathToSharedData);
        _controlChangeData.ChildChanged += HandleChildChanged;
        _controlChangeData.ChildAdded += HandleChildChanged;
        FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + _playerData.UserId + "/waitForShare/").Child(requestKey).RemoveValueAsync();
    }
    public void MissedShareScreen(string requestKey)
    {
        infoAboutShare.enabled = false;
        FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + _playerData.UserId + "/waitForShare/").Child(requestKey).RemoveValueAsync();
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
                                UnbindAllHandler();
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
        ClearScreen();
        draw();
        HasChanged(false);
    }

    public void UnbindAllHandler()
    {
        _controlSharedScreenWithMe.ChildAdded -= HandleControlRequestSharedScreen;
        _controlAllStudentInClass.ChildAdded -= HandleShowAllUserInClassAdd;
        _controlAllStudentInClass.ChildRemoved -= HandleShowAllUserInClassRemove;
        if (_useButtonShareSchreenWith)
        {
            _controlChangeData.ChildChanged -= HandleChildChanged;
            _controlChangeData.ChildAdded -= HandleChildChanged;
        }
    }
    #endregion

    #region game
    public void ClearScreen()
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
        ClearScreen();
        draw();
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
                    if (_pomSuc0 != -1 && _pomSuc1 != -1)
                    {
                        ChangeCol(_pomSuc0, true);
                        ChangeCol(_pomSuc1, true);
                    }
                }
            }
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
            _fbc.UpdateResult(_examArray, pathToSharedData);
        }
        if (kontrola.Count == _pocetPoliKtoreSaKontroluju)
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
                congrats_show();
            }
            else
            {
                nespravne_show();
            }
        }
    }

    public void doplnChybajuceData(Dictionary<string, string> input)
    {
        int input_Pocet = input.Count;
        switch (input_Pocet)
        {
            case 3:
            case 6:
            case 10:
                for (int i = 0; i < input_Pocet; i++)
                    input.Add(SLOT + i, "null");
                break;
        }
        _examArray = new ShuffleDictionary().Shuffle(_examArray);
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
        UnbindAllHandler();
        if (_playerData.selectedExamOnBoard != null)
        {
            _playerData.selectedExamOnBoard.RemoveAt(0);
            var pocet = _playerData.selectedExamOnBoardCount - _playerData.selectedExamOnBoard.Count();
            if (_playerData.selectedExamOnBoard.Count() <= 0)
            {
                _fbc.zapisStavRozriesenejUlohy(_playerData.UserId, _playerData.idSelectedExamOnBoard, "1");
                UnbindAllHandler();
                SceneManager.LoadScene("LoggedSelectLevel");
            }
            else
            {
                _fbc.zapisStavRozriesenejUlohy(_playerData.UserId, _playerData.idSelectedExamOnBoard, pocet + "/" + _playerData.selectedExamOnBoardCount);
                switch (_playerData.selectedExamOnBoard.First().ChildrenCount)
                {
                    case 3:
                        UnbindAllHandler();
                        SceneManager.LoadScene("LogLvl1_1");
                        break;
                    case 6:
                        UnbindAllHandler();
                        SceneManager.LoadScene("LogLvl3_1");
                        break;
                    case 10:
                        UnbindAllHandler();
                        SceneManager.LoadScene("LogLvl4_1");
                        break;
                }
            }
        }
        else
        {
            if (_playerData.idSelectedExamOnBoard != null)
            {
                _fbc.zapisStavRozriesenejUlohy(_playerData.UserId, _playerData.idSelectedExamOnBoard, "1");
            }
            UnbindAllHandler();
            SceneManager.LoadScene("LoggedSelectLevel");
        }
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);
        nespravne.enabled = false;
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