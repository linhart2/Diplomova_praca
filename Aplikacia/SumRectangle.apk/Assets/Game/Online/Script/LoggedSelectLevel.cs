using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoggedSelectLevel : MonoBehaviour
{
    private FirebaseCommunicationLibrary fbc;
    private PlayerData playerData = new PlayerData();
    private DatabaseReference _examsOnBoard;
    Dictionary<string, string> tabExam = new Dictionary<string, string>();
    public GameObject togglePrefab;
    public Canvas _blackBoard;
    private string _examsOnBoardDbVal;

    private void Awake()
    {
        fbc = new FirebaseCommunicationLibrary();
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
        _blackBoard = _blackBoard.GetComponent<Canvas>();
        _blackBoard.enabled = false;
        _examsOnBoard = FirebaseDatabase.DefaultInstance
                                                   .GetReference("/TABLES/" + playerData.SelectedClass);
        _examsOnBoard.ChildAdded += showExamsOnBoardAdd;
        _examsOnBoard.ChildRemoved += showExamsOnBoardRemove;


        Button btnShowTable = GameObject.Find("btnTable").GetComponent<Button>();
        btnShowTable.onClick.AddListener(delegate
        {
            fbc.insertIntoTableViews(playerData.SelectedClass, playerData.UserId, GameObject.Find("Content").transform.childCount.ToString());
            GameObject.Find("txtPocetUloh").GetComponent<Text>().text = "0";
            _blackBoard.enabled = true;
        });

        Button btnHideTable = GameObject.Find("btnCloseTable").GetComponent<Button>();
        btnHideTable.onClick.AddListener(delegate
        {
            fbc.insertIntoTableViews(playerData.SelectedClass, playerData.UserId, GameObject.Find("Content").transform.childCount.ToString());
            GameObject.Find("txtPocetUloh").GetComponent<Text>().text = "0";
            _blackBoard.enabled = false;
        });
    }
    #region Handle
    void showExamsOnBoardAdd(object sender, ChildChangedEventArgs args)
    {

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        FirebaseDatabase.DefaultInstance
                        .GetReference("USERS/" + playerData.UserId)
      .GetValueAsync().ContinueWith(task =>
      {
          if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              if (!snapshot.Child("SOLVE_EXAMS").Child(args.Snapshot.Key).Exists || snapshot.Child("SOLVE_EXAMS").Child(args.Snapshot.Key).Value.ToString() != "1")
              {
                  var exam = args.Snapshot;
                  FirebaseDatabase.DefaultInstance
                                .GetReference("EXAMS/" + exam.Key)
                      .GetValueAsync().ContinueWith(task1 =>
                      {
                          if (task.IsCompleted)
                          {
                              DataSnapshot snap = task1.Result;
                              var nazov = snap.Child("nazovUlohy").Value.ToString();
                              FirebaseDatabase.DefaultInstance
                                              .GetReference("USERS/" + snap.Child("teacherID").Value)
                          .GetValueAsync().ContinueWith(task2 =>
                          {
                              if (task.IsCompleted)
                              {

                                  FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + playerData.UserId + "/TABLE_VIEWS/" + playerData.SelectedClass).GetValueAsync().ContinueWith(task3 =>
                                  {
                                      if (task3.IsCompleted)
                                      {
                                          DataSnapshot snap2 = task2.Result;
                                          var meno = fbc.UserName(snap2.Child("firstName").Value.ToString(), snap2.Child("lastName").Value.ToString());
                                          generateExamToogleList(exam.Key, new Vector3(-1.5f, 0, 0), GetMenoNazovUlohy(meno, nazov), GetFormatedExamsCount(exam.Value.ToString()));

                                          DataSnapshot snaps = task3.Result;
                                          if (snaps.Value == null)
                                              _examsOnBoardDbVal = "0";
                                          else
                                              _examsOnBoardDbVal = snaps.Value.ToString();
                                          GameObject.Find("txtPocetUloh").GetComponent<Text>().text = GetCountsNewExamsOnBoard(int.Parse(_examsOnBoardDbVal), GameObject.Find("Content").transform.childCount);
                                      }
                                  });


                              }
                          });

                          }
                      });
              }
          }
      });
    }

    public void showExamsOnBoardRemove(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + playerData.UserId + "/TABLE_VIEWS/" + playerData.SelectedClass).GetValueAsync().ContinueWith(task3 =>
        {
            if (task3.IsCompleted)
            {
                DataSnapshot snaps = task3.Result;
                if (snaps.Value == null)
                    _examsOnBoardDbVal = "0";
                else
                    _examsOnBoardDbVal = snaps.Value.ToString();
                GameObject.Find("txtPocetUloh").GetComponent<Text>().text = GetCountsNewExamsOnBoard(int.Parse(_examsOnBoardDbVal), GameObject.Find("Content").transform.childCount);
            }
        });
        string key = args.Snapshot.Key;
        tabExam.Remove(key);
        Destroy(GameObject.Find(key));
    }

    #endregion

    private string GetMenoNazovUlohy(string meno, string nazov)
    {
        return string.Format("{0} => {1}", meno, nazov);
    }
    private string GetFormatedExamsCount(string menovatel, string citatel = "0")
    {
        return string.Format("{0}/{1}", citatel, menovatel);
    }
    private string GetCountsNewExamsOnBoard(int valueInDB, int actualValue)
    {
        var newExamsOnBoard = actualValue - valueInDB;
        if (newExamsOnBoard <= 0)
            return "0";
        return newExamsOnBoard.ToString();
    }

    public void BackToSelectClass(string value)
    {
        if (playerData.SelectedClass != null)
        {
            fbc.setSelectedClass(playerData.UserId, "null");
            fbc.removeOfflineStudent(playerData.SelectedClass, playerData.UserId);
        }
        SceneManager.LoadScene(value);
    }

    #region ToogleList
    private void generateExamToogleList(string ObjName, Vector3 vector, string txtName, string txtPocet)
    {

        makeRow(GameObject.Find("Content"), ObjName, vector);
        makeTogglePrefabs(GameObject.Find(ObjName), txtName, txtPocet, ObjName);

    }
    private void makeRow(GameObject cnvs, string objName, Vector3 vector)
    {
        GameObject rowObj = createRowObj(cnvs, objName);
        CanvasRenderer renderer = rowObj.AddComponent<CanvasRenderer>();

        GridLayoutGroup grid = rowObj.AddComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 1;
        grid.cellSize = new Vector2(720, 40);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.spacing = new Vector2(12, 0);

        grid.transform.localScale = new Vector3(1, 1, 1);
        grid.GetComponent<RectTransform>().localPosition = vector;
        grid.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        grid.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        grid.GetComponent<RectTransform>().sizeDelta = new Vector2(735, 60);
    }
    GameObject createRowObj(GameObject cnvs, string objName)
    {
        var panelM = new GameObject(objName);
        panelM.transform.SetParent(cnvs.transform);
        panelM.layer = LayerMask.NameToLayer("UI");
        return panelM;
    }
    private void makeTogglePrefabs(GameObject ObjName, string txtName, string txtPocet, string idSelectedExamOnBoard)
    {
        GameObject newItem = Instantiate(togglePrefab) as GameObject;
        newItem.transform.SetParent(ObjName.transform);
        newItem.layer = LayerMask.NameToLayer("UI");
        newItem.name = "Toggle";
        newItem.transform.GetChild(0).GetComponent<Text>().text = txtName;
        newItem.transform.GetChild(1).GetComponent<Text>().text = txtPocet;
        newItem.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate
        {
            GlobalData.playerData.idSelectedExamOnBoard = idSelectedExamOnBoard;
            FirebaseDatabase.DefaultInstance.GetReference("/EXAMS/" + idSelectedExamOnBoard + "/priklady").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + playerData.UserId + "/SOLVE_EXAMS/" + idSelectedExamOnBoard).GetValueAsync().ContinueWith(task2 =>
                        {
                            if (task2.IsCompleted)
                            {
                                DataSnapshot snapshot = task.Result;
                                DataSnapshot snapshot2 = task2.Result;
                                GlobalData.playerData.selectedExamOnBoardCount = (int)snapshot.ChildrenCount;
                                GlobalData.playerData.selectedExamOnBoard = snapshot.Children.ToList();
                                if (snapshot2.Value != null)
                                {
                                    string[] parse = snapshot2.Value.ToString().Split('/');
                                    int preskocPrvychX = int.Parse(parse[0]);
                                    GlobalData.playerData.selectedExamOnBoard.RemoveRange(0, preskocPrvychX);
                                    txtPocet = snapshot2.Value.ToString();
                                    newItem.transform.GetChild(1).GetComponent<Text>().text = txtPocet;
                                }
                                FirebaseDatabase.DefaultInstance.GetReference("/USERS").Child(playerData.UserId).Child("SOLVE_EXAMS").Child(idSelectedExamOnBoard).SetValueAsync(txtPocet);
                                var x = GlobalData.playerData.selectedExamOnBoard.First();
                                switch (x.ChildrenCount)
                                {
                                    case 3:
                                        SceneManager.LoadScene("LogLvl_1");
                                        break;
                                    case 6:
                                        SceneManager.LoadScene("LogLvl_1");
                                        break;
                                    case 10:
                                        SceneManager.LoadScene("LogLvl_1");
                                        break;
                                }
                            }
                        });
                }
            });
        });
        newItem.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

    }
    #endregion
}
