using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UIAddons;
using Firebase.Database;
using UnityEngine.SceneManagement;

public class LogedLevel_1_1 : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
{
    [SerializeField]
    public Canvas gratulation;      //- object gratulacia
    public Canvas nespravne;        //- object nespravne
    public Canvas unlock_level;     //- object otvoreny novy level
    public Canvas showSharedWith;      //- object zdielat s....
    public Canvas infoAboutShare;

    public CustomProgressBar progressBar; //- object progress bar
    public GameObject[] itemPrefab;
    public GameObject togglePrefab;
    private GameObject[] slots;
    private bool zobrazSlider = true;
    public int lvl;
    public static List<int> table = null;
    [SerializeField]
    public Dictionary<string, string> ExamArray;
    Generator_uloh priklad;
    Kontrola skontroluj;
    //SaveLoadProgress saveloadprogress;
    private FirebaseConnect firebase;
    public Dictionary<string, GameObject> ListStudent = new Dictionary<string, GameObject>();
    private PlayerData playerData = new PlayerData();
    private FirebaseCommunicationLibrary fbc;

    private string pathToSharedData;
    private bool useButtonShareSchreenWith = false;
    DatabaseReference controlChangeData;
    DatabaseReference controlSharedScreenWithMe;
    DatabaseReference controlAllStudentInClass;


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
        firebase = new FirebaseConnect();
        fbc = new FirebaseCommunicationLibrary();
    }

    void Start()
    {
        playerData = GlobalData.playerData;

#if DEBUG
        playerData.Name = "TestLingo";
        playerData.UserId = "ZAT4DktlgdYBVGwXYRpOfA3temm1";
        playerData.SelectedClass = "-KweS-rI-bTBYVX3g3vS";
        playerData.LoggedUser = true;
#endif
        controlAllStudentInClass = FirebaseDatabase.DefaultInstance
                                                   .GetReference("/CLASSES/" + playerData.SelectedClass + "/ONLINE_STUDENTS");
        controlAllStudentInClass.ChildAdded += HandleShowAllUserInClassAdd;
        controlAllStudentInClass.ChildRemoved += HandleShowAllUserInClassRemove;

        controlSharedScreenWithMe = FirebaseDatabase.DefaultInstance
                                                        .GetReference("/USERS/" + playerData.UserId + "/waitForShare");
        controlSharedScreenWithMe.ChildAdded += HandleControlRequestSharedScreen;

        gratulation = gratulation.GetComponent<Canvas>();
        infoAboutShare = infoAboutShare.GetComponent<Canvas>();
        showSharedWith = showSharedWith.GetComponent<Canvas>();
        nespravne = nespravne.GetComponent<Canvas>();
        unlock_level = unlock_level.GetComponent<Canvas>();
        skontroluj = new Kontrola(2);
        priklad = new Generator_uloh(lvl);
        table = priklad.get_array(3);
        CreateArrayExam(table);
        draw();
        gratulation.enabled = false;
        showSharedWith.enabled = false;
        nespravne.enabled = false;
        unlock_level.enabled = false;
        infoAboutShare.enabled = false;
        progressBar.slider.maxValue = 10f;
        progressBar.slider.minValue = 0f;
        progressBar.slider.value = 0f;
        StartFillingUpProgressBar();
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
        fbc.addSharedScreen(key, screen);
        fbc.inserMyIdToSharedScreen(playerData.UserId, playerData.Name, key);
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
            fbc.SendRequestWithShareScreen(user, key, request);
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
        fbc.inserMyIdToSharedScreen(playerData.UserId, playerData.Name, screenKey);
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

    #region game
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
        if (useButtonShareSchreenWith && zaznamenajDoDB)
        {
            firebase.UpdateResult(ExamArray, pathToSharedData);
        }
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
                    //saveloadprogress.SaveLock(lvl);
                    //saveloadprogress.Save(lvl, zobrazSlider, progressBar.slider.value);
                }
                else
                {
                    congrats_show();
                    //saveloadprogress.Save(lvl, zobrazSlider, progressBar.slider.value);
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
        UnbindAllHandler();
        SceneManager.LoadScene(18);
    }

    IEnumerator congrats_hide()
    {
        yield return new WaitForSeconds(2.0f);

        gratulation.enabled = false;
        UnbindAllHandler();
        //SceneManager.LoadScene(UnityEngine.Random.Range(19, 20));
        SceneManager.LoadScene(21);
    }

    IEnumerator nespravne_hide()
    {
        yield return new WaitForSeconds(2.0f);
        nespravne.enabled = false;
        UnbindAllHandler();
        SceneManager.LoadScene(21);
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