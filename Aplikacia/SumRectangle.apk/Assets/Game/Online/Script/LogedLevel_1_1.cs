using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UIAddons;
using Firebase.Database;
using UnityEngine.SceneManagement;
using UnityEditor;

public class LogedLevel_1_1 : MonoBehaviour, UnityEngine.EventSystems.IHasChanged
{
    [SerializeField]
    public Canvas gratulation;      //- object gratulacia
    public Canvas nespravne;        //- object nespravne
    public Canvas unlock_level;     //- object otvoreny novy level
    public Canvas showSharedWith;      //- object zdielat s....
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
    //SaveLoadProgress saveloadprogress;
    FirebaseConnect firebase;
    public Dictionary<string, GameObject> ListStudent = new Dictionary<string, GameObject>();
    private PlayerData playerData = new PlayerData();

    void Start()
    {
        playerData = GlobalData.playerData;
        //saveloadprogress = new SaveLoadProgress();
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
        showSharedWith = showSharedWith.GetComponent<Canvas>();
        nespravne = nespravne.GetComponent<Canvas>();
        unlock_level = unlock_level.GetComponent<Canvas>();
        skontroluj = new Kontrola(2);
        priklad = new Generator_uloh(lvl);
        table = priklad.get_array(3);
        CreateArrayExam(table);
        draw();
        //saveloadprogress.Load(lvl);
        //progressBar.slider.value = saveloadprogress.progress;
        //zobrazSlider = saveloadprogress.zobraz;
        gratulation.enabled = false;
        showSharedWith.enabled = false;
        nespravne.enabled = false;
        unlock_level.enabled = false;
        progressBar.slider.maxValue = 10f;
        progressBar.slider.minValue = 0f;
        progressBar.slider.value = 0f;
        //saveloadprogress.Load(lvl);
        //progressBar.slider.value = saveloadprogress.progress;
        //zobrazSlider = saveloadprogress.zobraz;
        StartFillingUpProgressBar();
        HasChanged();

        Button btnZrus = GameObject.Find("btnZrus").GetComponent<Button>();
        btnZrus.onClick.AddListener(delegate
                    {
                        showSharedWith.enabled = false;
                        GameObject.Find("tbOznacVsetkych").GetComponent<Toggle>().isOn = false;
                        foreach (var student in ListStudent)
                        {
                            student.Value.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
                        }
                    });
        Button btnShare = GameObject.Find("Share").GetComponent<Button>();
        btnShare.onClick.AddListener(delegate
                        {
                            showSharedWith.enabled = true;
                        });
        Toggle tbOznacVsetkych = GameObject.Find("tbOznacVsetkych").GetComponent<Toggle>();
        tbOznacVsetkych.onValueChanged.AddListener(delegate
                        {
                            SelectAll();
                        });
    }

    public void ShareScreenWith()
    {

    }

    public void SelectAll()
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
            foreach (var student in ListStudent)
            {
                student.Value.transform.GetChild(0).GetComponent<Toggle>().isOn = false;
            }
        }
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
        string value = args.Snapshot.GetRawJsonValue();
        Student data = JsonUtility.FromJson<Student>(value);
        generateStudentToogleList("row" + key, new Vector3(-1.5f, 0, 0), string.Format("{0} {1}", data.firstName, data.lastName));

    }
    public void HandleShowAllUserInClassChange(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string key = args.Snapshot.Key;
        string value = args.Snapshot.GetRawJsonValue();
        Student data = JsonUtility.FromJson<Student>(value);
        Text text = ListStudent["row" + key].transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        text.text = string.Format("{0} {1}", data.firstName, data.lastName);
    }
    public void HandleShowAllUserInClassRemove(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        string key = args.Snapshot.Key;
        Destroy(ListStudent["row" + key]);
        ListStudent.Remove("row" + key);

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
    #endregion

    #region ToogleList
    public void generateStudentToogleList(string ObjName, Vector3 vector, string txtName)
    {

        makeRow(GameObject.Find("Content"), ObjName, vector);
        makeToggle(GameObject.Find(ObjName), txtName);

    }

    void makeRow(GameObject cnvs, string objName, Vector3 vector)
    {
        GameObject rowObj = createRowObj(cnvs, objName);
        CanvasRenderer renderer = rowObj.AddComponent<CanvasRenderer>();

        GridLayoutGroup grid = rowObj.AddComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 1;
        grid.cellSize = new Vector2(740, 40);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.spacing = new Vector2(12, 0);

        grid.transform.localScale = new Vector3(1, 1, 1);
        grid.GetComponent<RectTransform>().localPosition = vector;
        grid.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        grid.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        grid.GetComponent<RectTransform>().sizeDelta = new Vector2(780, 60);
        ListStudent[objName] = rowObj;
    }
    void makeToggle(GameObject cnvs, string txtName)
    {
        GameObject toggleObj = createToggleObj(cnvs, "Toggle");
        GameObject bgObj = createBackgroundObj(toggleObj);
        GameObject checkMarkObj = createCheckmarkObj(bgObj);
        GameObject labelObj = createLabelObj(toggleObj);
        attachAllComponents(toggleObj, bgObj, checkMarkObj, labelObj, txtName);
    }

    GameObject createRowObj(GameObject cnvs, string objName)
    {
        var panelM = new GameObject(objName);
        panelM.transform.SetParent(cnvs.transform);
        panelM.layer = LayerMask.NameToLayer("UI");
        return panelM;
    }


    //1.Create a *Toggle* GameObject then make it child of the *Canvas*.
    GameObject createToggleObj(GameObject cnvs, string toogleName)
    {
        GameObject toggle = new GameObject(toogleName);
        toggle.transform.SetParent(cnvs.transform);
        toggle.layer = LayerMask.NameToLayer("UI");
        return toggle;
    }

    //2.Create a Background GameObject then make it child of the Toggle GameObject.
    GameObject createBackgroundObj(GameObject toggle)
    {
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(toggle.transform);
        bg.layer = LayerMask.NameToLayer("UI");
        return bg;
    }

    //3.Create a Checkmark GameObject then make it child of the Background GameObject.
    GameObject createCheckmarkObj(GameObject bg)
    {
        GameObject chmk = new GameObject("Checkmark");
        chmk.transform.SetParent(bg.transform);
        chmk.layer = LayerMask.NameToLayer("UI");
        return chmk;
    }

    //4.Create a Label GameObject then make it child of the Toggle GameObject.
    GameObject createLabelObj(GameObject toggle)
    {
        GameObject lbl = new GameObject("Label");
        lbl.transform.SetParent(toggle.transform);
        lbl.layer = LayerMask.NameToLayer("UI");
        return lbl;
    }

    //5.Now attach components like Image, Text and Toggle to each GameObject like it appears in the Editor.
    void attachAllComponents(GameObject toggle, GameObject bg, GameObject chmk, GameObject lbl, string txtName)
    {
        //Attach Text to label
        Text txt = lbl.AddComponent<Text>();
        txt.text = txtName;
        txt.alignment = TextAnchor.MiddleLeft;
        Font arialFont =
        (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        txt.font = arialFont;
        txt.fontSize = 24;
        txt.lineSpacing = 1;
        txt.color = new Color(50 / 255, 50 / 255, 50 / 255, 255 / 255);

        txt.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        txt.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        txt.GetComponent<RectTransform>().offsetMax = new Vector2(12.05f, 1.5f);
        txt.GetComponent<RectTransform>().offsetMin = new Vector2(48.35f, 1.5f);
        //txt.GetComponent<RectTransform>().sizeDelta = new Vector4(48.35f, 1.5f, -12.05f, 1.5f);
        txt.transform.localScale = new Vector3(1, 1, 1);
        //txtRect.y

        //Attach Image Component to the Checkmark
        Image chmkImage = chmk.AddComponent<Image>();
        chmkImage.sprite = (Sprite)AssetDatabase.GetBuiltinExtraResource(typeof(Sprite), "UI/Skin/Checkmark.psd");
        chmkImage.type = Image.Type.Simple;
        chmkImage.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        chmkImage.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        chmkImage.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
        chmkImage.transform.localScale = new Vector3(1, 1, 1);


        //Attach Image Component to the Background
        Image bgImage = bg.AddComponent<Image>();
        bgImage.sprite = (Sprite)AssetDatabase.GetBuiltinExtraResource(typeof(Sprite), "UI/Skin/UISprite.psd");
        bgImage.type = Image.Type.Sliced;

        bgImage.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
        bgImage.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        bgImage.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
        bgImage.GetComponent<RectTransform>().localPosition = new Vector3(20, -20, 0);
        bgImage.transform.localScale = new Vector3(1, 1, 1);

        //Attach Toggle Component to the Toggle
        Toggle toggleComponent = toggle.AddComponent<Toggle>();
        toggleComponent.transition = Selectable.Transition.ColorTint;
        toggleComponent.targetGraphic = bgImage;
        toggleComponent.isOn = false;
        toggleComponent.toggleTransition = Toggle.ToggleTransition.Fade;
        toggleComponent.graphic = chmkImage;
        toggleComponent.transform.localScale = new Vector3(1, 1, 1);
        toggle.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
    }
    #endregion
}