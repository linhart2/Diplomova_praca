using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;

public class FirebaseCommunicationLibrary
{

    private Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    private DatabaseReference mDatabaseRef;
    private ILoadScene scena;
    private PlayerData playerData = new PlayerData();


    public FirebaseCommunicationLibrary()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://sumrectangle.firebaseio.com/");
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    public void SaveGlobalDataAll(string name, string userId, List<string> classses, bool loggedUser)
    {
        GlobalData.playerData.Name = name;
        GlobalData.playerData.UserId = userId;
        GlobalData.playerData.Classes = classses;
        GlobalData.playerData.LoggedUser = loggedUser;
    }

    public void SaveGlobalDataUserId(string userId)
    {
        GlobalData.playerData.UserId = userId;
    }
    public void SaveGlobalDataLoggedUser(bool loggedUser)
    {
        GlobalData.playerData.LoggedUser = loggedUser;
    }

    public void SaveGlobalSelectedClass(string @class)
    {
        GlobalData.playerData.SelectedClass = @class;
    }


    #region Auth
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                playerData.LoggedUser = false;
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                playerData.LoggedUser = true;
                playerData.UserId = auth.CurrentUser.UserId;
            }
        }
        SaveGlobalDataUserId(playerData.UserId);
        SaveGlobalDataLoggedUser(playerData.LoggedUser);
    }

    public void OnDestroy(ILoadScene scena)
    {
        this.scena = scena;
        auth.StateChanged -= AuthStateChanged;
        auth.SignOut();
        auth = null;
        SaveGlobalDataAll(null, null, null, false);
        SaveGlobalSelectedClass(null);
        this.scena.LoadScene();
    }

    public void RegistrationNewAccount(string meno, string priezvisko, string email, string heslo, string hesloAgain, ILoadScene scena)
    {
        this.scena = scena;
        auth.CreateUserWithEmailAndPasswordAsync(email, heslo).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            if (task.IsCompleted)
            {
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
                Student student = new Student(meno, priezvisko, email);
                RegistrationSaveData(student, newUser.UserId);
                playerData.Name = UserName(meno, priezvisko);
                this.scena.LoadScene();
            }
        });
    }

    public void Login(string email, string heslo, ILoadScene scena)
    {
        this.scena = scena;
        Firebase.Auth.Credential credential =
                    Firebase.Auth.EmailAuthProvider.GetCredential(email, heslo);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            SceneManager.LoadScene(29);
        });
    }

    #endregion

    #region InsertDatabase
    private void RegistrationSaveData(Student student, string userId)
    {
        mDatabaseRef.Child("USERS").Child(userId).SetRawJsonValueAsync(JsonUtility.ToJson(student));
    }
    private void addStudentToClass(string userId, string hesloTriedy, string classID)
    {
        mDatabaseRef.Child("USERS").Child(userId).Child("MY_CLASS").Child(classID).SetValueAsync(hesloTriedy);
    }
    public void addSharedScreen(string screenId, SharedScreen screen)
    {
        mDatabaseRef.Child("SHARED_SCREEN").Child(screenId).SetRawJsonValueAsync(JsonUtility.ToJson(screen));
    }
    public void inserMyIdToSharedScreen(string userId, string userName, string screenId)
    {
        mDatabaseRef.Child("SHARED_SCREEN").Child(screenId).Child("users_id").Child(userId).SetValueAsync(userName);
    }
    public void SendRequestWithShareScreen(string userId, string key, SharedScreenRequest value)
    {
        mDatabaseRef.Child("USERS").Child(userId).Child("waitForShare").Child(key).SetRawJsonValueAsync(JsonUtility.ToJson(value));
    }

    /*private void addClass(string heslo, string teacherID, string className)
    {
        string key = mDatabaseRef.Child("CLASSES").Push().Key;
        ClassRoom classroom = new ClassRoom(heslo, teacherID, className);
        string json = JsonUtility.ToJson(classroom);
        mDatabaseRef.Child("CLASSES").Child(key).SetRawJsonValueAsync(json);
    }*/
    #endregion

    #region LoadDataFromDB
    public void GetUserData(string userId, bool changeScene = false, int scene = 19)
    {

        if (playerData.LoggedUser)
        {
            FirebaseDatabase.DefaultInstance.GetReference("/USERS/" + userId + "/")
                        .GetValueAsync().ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                Debug.Log("skontroluj pripojenie");
                                SceneManager.LoadScene(0);
                            }
                            else if (task.IsCompleted)
                            {
                                DataSnapshot snapshot = task.Result;
                                playerData.Name = UserName(snapshot.Child("firstName").Value.ToString(), snapshot.Child("lastName").Value.ToString());
                                List<string> _classData = new List<string>();
                                foreach (var classData in snapshot.Child("MY_CLASS").Children.ToList())
                                {
                                    _classData.Add(classData.Value.ToString());
                                }
                                playerData.Classes = _classData;
                                playerData.UserId = userId;

                                if (changeScene)
                                {
                                    GlobalData.playerData = playerData;
                                    SaveGlobalDataAll(playerData.Name, userId, playerData.Classes, playerData.LoggedUser);
                                    SceneManager.LoadScene(scene);
                                }

                            }
                        });
        }
        else
        {
            SaveGlobalDataAll(null, null, null, false);
            SaveGlobalSelectedClass(null);
            SceneManager.LoadScene(17);
        }
    }

    public Dictionary<string, string> getShareData(string path)
    {
        Dictionary<string, string> _data = new Dictionary<string, string>();
        FirebaseDatabase.DefaultInstance.GetReference(path)
                    .GetValueAsync().ContinueWith(task =>
                    {
                        if (task.IsFaulted) { }
                        else if (task.IsCompleted)
                        {
                            DataSnapshot snapshot = task.Result;
                            foreach (var x in snapshot.Children)
                            {
                                _data[x.Key] = x.Value.ToString();
                            }
                        }
                    });

        return _data;
    }

    public void FindClass(string hesloTriedy, string studentID)
    {
        FirebaseDatabase.DefaultInstance.GetReference("/CLASSES").OrderByChild("heslo").EqualTo(hesloTriedy)
                    .GetValueAsync().ContinueWith(task =>
                    {
                        if (task.IsFaulted) { }
                        else if (task.IsCompleted)
                        {
                            DataSnapshot snapshot = task.Result;
                            bool exists = (snapshot.Value != null);
                            string classID = snapshot.Children.SingleOrDefault().Key;
                            classExistCallback(hesloTriedy, classID, studentID, exists);
                        }
                    });
    }
    #endregion

    #region Callback
    private void classExistCallback(string hesloTriedy, string classID, string studentID, bool exists)
    {
        if (exists)
        {
            Debug.Log("class " + hesloTriedy + " exists!!!");
            addStudentToClass(studentID, hesloTriedy, classID);
            GlobalData.playerData.Classes.Add(hesloTriedy);
            SaveGlobalSelectedClass(hesloTriedy);
            SceneManager.LoadScene(20);

        }
        else
        {
            Debug.Log("class " + hesloTriedy + " not exists!!!");
        }
    }
    #endregion

    #region Property
    private string UserName(string fName, string lName) { return string.Format("{0} {1}", fName, lName); }
    #endregion

}


public interface ILoadScene
{
    void LoadScene();
}

class LoadScene : ILoadScene
{
    private int idScene;

    public LoadScene(int idScene)
    {
        this.idScene = idScene;
    }
    void ILoadScene.LoadScene()
    {
        SceneManager.LoadScene(this.idScene);
    }
}

public class Student
{
    public string firstName;
    public string lastName;
    public string email;

    public Student(string firstName, string lastName, string email)
    {
        this.firstName = firstName.First().ToString().ToUpper() + firstName.Substring(1);
        this.lastName = lastName.First().ToString().ToUpper() + lastName.Substring(1);
        this.email = email;
    }
}

public class ClassRoom
{
    public string heslo;
    public string teacherID;
    public string className;

    public ClassRoom() { }
    public ClassRoom(string heslo, string teacherID, string className)
    {
        this.heslo = heslo;
        this.teacherID = teacherID;
        this.className = className;
    }
}

public class SharedScreen
{
    public Dictionary<string, object> data = new Dictionary<string, object>();
    public bool screeen_locker;
    public string screen_name;
    public string admin_name;
    public List<string> users_id = new List<string>();

    public SharedScreen() { }
    public SharedScreen(Dictionary<string, object> data, bool screeen_locker, string screen_name, string admin_name, List<string> users_id)
    {
        this.data = data;
        this.screeen_locker = screeen_locker;
        this.screen_name = screen_name;
        this.admin_name = admin_name;
        this.users_id = users_id;
    }
}

public class SharedScreenRequest
{


    public string share_object;
    public string admin_name;


    public SharedScreenRequest() { }
    public SharedScreenRequest(string share_object, string admin_name)
    {
        this.share_object = share_object;
        this.admin_name = admin_name;

    }
}

