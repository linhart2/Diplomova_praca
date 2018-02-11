using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowClassScript : MonoBehaviour
{

    FirebaseCommunicationLibrary fbc;
    public Text txtLoggedUser;
    public Button btnOdhlasit;
    public Button btnGotoClass;
    public InputField inputInsertClassKey;
    private PlayerData playerData = new PlayerData();

    private void Awake()
    {
        fbc = new FirebaseCommunicationLibrary();
    }

    void Start()
    {
        playerData = GlobalData.playerData;
        GlobalData.playerData.SelectedClass = null;

        txtLoggedUser.text = string.Format("{0} {1}", txtLoggedUser.text, playerData.Name);
        btnOdhlasit.onClick.AddListener(delegate
        {
            fbc.OnDestroy(new LoadScene(17));
        });
        btnGotoClass.onClick.AddListener(delegate
        {
            fbc.FindClass(inputInsertClassKey.text, playerData.UserId, playerData.Name);
        });
#if DEBUG
        playerData.Name = "TestLingo";
        playerData.UserId = "ZAT4DktlgdYBVGwXYRpOfA3temm1";
        playerData.SelectedClass = "ZSsMSHalic_skuska_1A";
        playerData.LoggedUser = true;
#endif
        if (playerData.Classes != null && playerData.Classes.Count > 0)
        {
            int i = 0;
            foreach (var trieda in playerData.Classes)
            {
                AddButtonWithClassName("obj" + trieda.Value, new Vector3(0, -i * 30, 0), trieda.Value, trieda.Key);
                i++;
            }
        }
    }

    private void AddButtonWithClassName(string ObjName, Vector3 vector, string className, string classKey)
    {
        var panelG = new GameObject(ObjName).AddComponent<GridLayoutGroup>();
        panelG.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        panelG.constraintCount = 2;
        panelG.cellSize = new Vector2(350, 30);
        panelG.childAlignment = TextAnchor.MiddleCenter;
        panelG.startAxis = GridLayoutGroup.Axis.Horizontal;
        panelG.startCorner = GridLayoutGroup.Corner.UpperLeft;
        panelG.transform.parent = GameObject.Find("Content").gameObject.transform;
        panelG.transform.localScale = new Vector3(1, 1, 1);
        panelG.GetComponent<RectTransform>().localPosition = vector;
        panelG.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
        panelG.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        Button newButton = Instantiate(btnGotoClass) as Button;
        newButton.transform.SetParent(panelG.transform, false);
        newButton.GetComponentInChildren<Text>().text = className;
        newButton.onClick.AddListener(delegate
        {
            SelectClass(className, classKey);
        });

    }

    private void SelectClass(string className, string classId)
    {
        GlobalData.playerData.SelectedClass = classId;
        fbc.insertIntoStudentsInClass(classId, playerData.UserId, playerData.Name);
        fbc.insertIntoOnlineStudent(classId, playerData.UserId, playerData.Name);
        fbc.setSelectedClass(playerData.UserId, classId);
        SceneManager.LoadScene("LogedSelectLevel");
    }
}
