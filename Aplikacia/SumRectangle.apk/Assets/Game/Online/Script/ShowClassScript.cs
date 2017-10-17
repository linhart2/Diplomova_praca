using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowClassScript : MonoBehaviour
{

    FirebaseCommunicationLibrary fbC;
    public Text txtLoggedUser;
    public Button btnOdhlasit;
    public Button btnGotoClass;
    public InputField inputInsertClassKey;
    private PlayerData playerData = new PlayerData();

    void Start()
    {
        playerData = GlobalData.playerData;
        fbC = new FirebaseCommunicationLibrary();
        txtLoggedUser.text = string.Format("{0} {1}", txtLoggedUser.text, playerData.Name);
        btnOdhlasit.onClick.AddListener(delegate
        {
            fbC.OnDestroy(new LoadScene(17));
        });
        btnGotoClass.onClick.AddListener(delegate
        {
            fbC.FindClass(inputInsertClassKey.text, playerData.UserId);
        });
        for (int i = 0; i < playerData.Classes.Count; i++)
        {
            AddButtonWithClassName("obj" + playerData.Classes[i], new Vector3(0, -i * 30, 0), playerData.Classes[i]);
        }

    }

    private void AddButtonWithClassName(string ObjName, Vector3 vector, string className)
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
            SelectClass(className);
        });

    }

    private void SelectClass(string className)
    {
        GlobalData.playerData.SelectedClass = className;
        SceneManager.LoadScene(20);
    }
}
