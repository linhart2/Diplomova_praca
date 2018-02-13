using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UIAddons;
using Firebase.Database;
using UnityEngine.SceneManagement;
using System.Linq;

public class LogLvl_1 : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
{

    public Canvas gratulation;      //- object gratulacia
    public Canvas nespravne;        //- object nespravne
    public Canvas showSharedWith;      //- object zdielat s....
    public Canvas infoAboutShare;
    public GameObject[] itemPrefab;
    public GameObject togglePrefab;
    public int lvl;

    private GameObject[] slots;
    private Kontrola _skontroluj;
    private Dictionary<string, GameObject> _listStudent = new Dictionary<string, GameObject>();
    private PlayerData _playerData = new PlayerData();
    private FirebaseCommunicationLibrary _fbc;
    private Dictionary<string, string> _examArray;
    private List<int> zoznamCiselPrikladu = new List<int>();
    private string pathToSharedData;
    private bool useButtonShareSchreenWith = false;
    private DatabaseReference controlChangeData;
    private DatabaseReference controlSharedScreenWithMe;
    private DatabaseReference controlAllStudentInClass;


    private void Awake()
    {
        Button btnBack = GameObject.Find("Back").GetComponent<Button>();
        btnBack.onClick.AddListener(delegate
        {
            UnbindAllHandler();
            SceneManager.LoadScene("LogedSelectLevel");

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
        _playerData = GlobalData.playerData;

#if DEBUG
        _playerData.Name = "TestLingo";
        _playerData.UserId = "ZAT4DktlgdYBVGwXYRpOfA3temm1";
        _playerData.SelectedClass = "-KweS-rI-bTBYVX3g3vS";
        _playerData.LoggedUser = true;
#endif
        foreach (var priklad in _playerData.selectedExamOnBoard.First().Children)
        {
            if (priklad.Value.ToString() != "null")
            {
                zoznamCiselPrikladu.Add(Int32.Parse(priklad.Value.ToString()));
            }
        }
        controlAllStudentInClass = FirebaseDatabase.DefaultInstance
                                                   .GetReference("/CLASSES/" + _playerData.SelectedClass + "/ONLINE_STUDENTS");
        controlAllStudentInClass.ChildAdded += HandleShowAllUserInClassAdd;
        controlAllStudentInClass.ChildRemoved += HandleShowAllUserInClassRemove;

        controlSharedScreenWithMe = FirebaseDatabase.DefaultInstance
                                                        .GetReference("/USERS/" + _playerData.UserId + "/waitForShare");
        controlSharedScreenWithMe.ChildAdded += HandleControlRequestSharedScreen;

        gratulation = gratulation.GetComponent<Canvas>();
        infoAboutShare = infoAboutShare.GetComponent<Canvas>();
        showSharedWith = showSharedWith.GetComponent<Canvas>();
        nespravne = nespravne.GetComponent<Canvas>();

        _skontroluj = new Kontrola(2);
        CreateArrayExam(zoznamCiselPrikladu);
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
            screen_name = "screen1a",
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
        useButtonShareSchreenWith = true;
        Console.WriteLine("AcceptShareScren id={0} name={1} screenKey={2}", _playerData.UserId, _playerData.Name, screenKey);
        _fbc.inserMyIdToSharedScreen(_playerData.UserId, _playerData.Name, screenKey);
        controlChangeData = FirebaseDatabase.DefaultInstance
                                            .GetReference(pathToSharedData);
        controlChangeData.ChildChanged += HandleChildChanged;
        controlChangeData.ChildAdded += HandleChildChanged;
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

    #region game
    public void CreateArrayExam(List<int> pr)
    {
        _examArray = new Dictionary<string, string>();
        foreach (Transform objekt in GameObject.Find("Panel1").gameObject.transform)
        {
            foreach (Transform slot in objekt)
            {
                if (slot.name.Contains("SlotM"))
                {
                    int index = Int32.Parse(slot.name.Substring(slot.name.IndexOf("_") + 1));
                    _examArray.Add(slot.name, pr[index].ToString());
                }
                else
                {
                    _examArray.Add(slot.name, "null");
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
                if (_examArray[slot.name] != "null")
                {
                    GameObject newItem = Instantiate(itemPrefab[int.Parse(_examArray[slot.name])]) as GameObject;
                    newItem.transform.parent = slot.transform;
                    newItem.transform.localScale = new Vector3(1, 1, 1);
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
                if (slot.GetComponent<Slot>().item == null)
                {
                    _examArray[slot.name] = "null";
                }
                else
                {
                    _examArray[slot.name] = slot.GetComponent<Slot>().item.name.Substring(0, slot.GetComponent<Slot>().item.name.IndexOf("("));
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
        if (useButtonShareSchreenWith && zaznamenajDoDB)
        {
            _fbc.UpdateResult(_examArray, pathToSharedData);
        }
        solution_control(kontrola);

    }

    public void solution_control(List<int> kontrola)
    {
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
        _playerData.selectedExamOnBoard.RemoveAt(0);
        var pocet = _playerData.selectedExamOnBoardCount - _playerData.selectedExamOnBoard.Count();

        gratulation.enabled = false;
        UnbindAllHandler();
        if (_playerData.selectedExamOnBoard.Count() <= 0)
        {
            _fbc.zapisStavRozriesenejUlohy(_playerData.UserId, _playerData.idSelectedExamOnBoard, "1");
            SceneManager.LoadScene("LogedSelectLevel");
        }
        else
        {
            _fbc.zapisStavRozriesenejUlohy(_playerData.UserId, _playerData.idSelectedExamOnBoard, pocet + "/" + _playerData.selectedExamOnBoardCount);
            SceneManager.LoadScene("LogLvl_1");
        }

    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);
        nespravne.enabled = false;
        UnbindAllHandler();
        SceneManager.LoadScene("LogLvl_1");
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
