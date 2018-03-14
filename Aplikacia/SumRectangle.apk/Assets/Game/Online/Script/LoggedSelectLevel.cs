using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoggedSelectLevel : MonoBehaviour
{
    private FirebaseCommunicationLibrary _fbc;
    private PlayerData _playerData = new PlayerData();
    private DatabaseReference _examsOnBoard;
    private DatabaseReference _controlSharedScreenWithMe;
    private Dictionary<string, string> _tabExam = new Dictionary<string, string>();
    public GameObject togglePrefab;
    public Canvas blackBoard;
    public Canvas infoAboutShare;
    private string _examsOnBoardDbVal;

    private void Awake()
    {
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
        blackBoard = blackBoard.GetComponent<Canvas>();
        blackBoard.enabled = false;
        infoAboutShare = infoAboutShare.GetComponent<Canvas>();
        infoAboutShare.enabled = false;
        _examsOnBoard = FirebaseDatabase.DefaultInstance
                                                   .GetReference("/TABLES/" + _playerData.SelectedClass);
        _examsOnBoard.ChildAdded += showExamsOnBoardAdd;
        _examsOnBoard.ChildRemoved += showExamsOnBoardRemove;

        _controlSharedScreenWithMe = FirebaseDatabase.DefaultInstance
                                                        .GetReference("/USERS/" + _playerData.UserId + "/waitForShare");
        _controlSharedScreenWithMe.ChildAdded += HandleControlRequestSharedScreen;


        Button btnShowTable = GameObject.Find("btnTable").GetComponent<Button>();
        btnShowTable.onClick.AddListener(delegate
        {
            _fbc.insertIntoTableViews(_playerData.SelectedClass, _playerData.UserId, GameObject.Find("Content").transform.childCount.ToString());
            GameObject.Find("txtPocetUloh").GetComponent<Text>().text = "0";
            blackBoard.enabled = true;
        });

        Button btnHideTable = GameObject.Find("btnCloseTable").GetComponent<Button>();
        btnHideTable.onClick.AddListener(delegate
        {
            _fbc.insertIntoTableViews(_playerData.SelectedClass, _playerData.UserId, GameObject.Find("Content").transform.childCount.ToString());
            GameObject.Find("txtPocetUloh").GetComponent<Text>().text = "0";
            blackBoard.enabled = false;
        });
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

    #region Handle
    public void showExamsOnBoardAdd(object sender, ChildChangedEventArgs args)
    {

        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        FirebaseDatabase.DefaultInstance
                        .GetReference("USERS/" + _playerData.UserId)
      .GetValueAsync().ContinueWith(task =>
      {
          if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              var pocetVyriesenychPrikladovUlohy = snapshot.Child("SOLVE_EXAMS").Child(args.Snapshot.Key).Value.ToString();
              if (!snapshot.Child("SOLVE_EXAMS").Child(args.Snapshot.Key).Exists || pocetVyriesenychPrikladovUlohy != "1")
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

                                  FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + _playerData.UserId + "/TABLE_VIEWS/" + _playerData.SelectedClass).GetValueAsync().ContinueWith(task3 =>
                                  {
                                      if (task3.IsCompleted)
                                      {
                                          DataSnapshot snap2 = task2.Result;
                                          var meno = _fbc.UserName(snap2.Child("firstName").Value.ToString(), snap2.Child("lastName").Value.ToString());
                                          if (pocetVyriesenychPrikladovUlohy == null && pocetVyriesenychPrikladovUlohy == string.Empty)
                                              pocetVyriesenychPrikladovUlohy = GetFormatedExamsCount(exam.Value.ToString());
                                          generateExamToogleList(exam.Key, new Vector3(-1.5f, 0, 0), GetMenoNazovUlohy(meno, nazov), pocetVyriesenychPrikladovUlohy);

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
        FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + _playerData.UserId + "/TABLE_VIEWS/" + _playerData.SelectedClass).GetValueAsync().ContinueWith(task3 =>
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
        _tabExam.Remove(key);
        Destroy(GameObject.Find(key));
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

    public void UnbindAllHandler()
    {
        _examsOnBoard.ChildAdded -= showExamsOnBoardAdd;
        _examsOnBoard.ChildRemoved -= showExamsOnBoardRemove;
        _controlSharedScreenWithMe.ChildAdded -= HandleControlRequestSharedScreen;

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
        if (_playerData.SelectedClass != null)
        {
            _fbc.setSelectedClass(_playerData.UserId, "null");
            _fbc.removeOfflineStudent(_playerData.SelectedClass, _playerData.UserId);
        }
        UnbindAllHandler();
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
                    FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + _playerData.UserId + "/SOLVE_EXAMS/" + idSelectedExamOnBoard).GetValueAsync().ContinueWith(task2 =>
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
                                FirebaseDatabase.DefaultInstance.GetReference("/USERS").Child(_playerData.UserId).Child("SOLVE_EXAMS").Child(idSelectedExamOnBoard).SetValueAsync(txtPocet);
                                var x = GlobalData.playerData.selectedExamOnBoard.First();
                                switch (x.ChildrenCount)
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
                                    case 24:
                                        UnbindAllHandler();
                                        SceneManager.LoadScene("LogLvl4_2");
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
