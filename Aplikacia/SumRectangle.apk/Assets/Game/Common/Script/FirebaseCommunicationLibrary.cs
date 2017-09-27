using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseCommunicationLibrary{
	public Firebase.Auth.FirebaseAuth auth;
    public DatabaseReference mDatabaseRef;
    public ILoadScene scena;


	public FirebaseCommunicationLibrary(){
		auth = FirebaseAuthInit ();
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://sumrectangle.firebaseio.com/");
		mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
	}

	private Firebase.Auth.FirebaseAuth FirebaseAuthInit(){
		return Firebase.Auth.FirebaseAuth.DefaultInstance;
	}

    public void RegistrationNewAccount(string meno,string priezvisko,string email,string heslo,ILoadScene scena){
        this.scena = scena;
		auth.CreateUserWithEmailAndPasswordAsync(email, heslo).ContinueWith(task => {
			if (task.IsCanceled) {
				Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				return;
			}
			if(task.IsCompleted){
				Firebase.Auth.FirebaseUser newUser = task.Result;
				Debug.LogFormat("User signed in successfully: {0} ({1})",
					newUser.DisplayName, newUser.UserId);
                RegistrationSaveData(new Student(meno, priezvisko, email),newUser.UserId);
                this.scena.LoadScene();
			}
		});
	}

    public void RegistrationSaveData(Student student,string userId ) {
        string json = JsonUtility.ToJson(student);
		mDatabaseRef.Child("USERS").Child(userId).SetRawJsonValueAsync(json);
		
	}
}
public interface ILoadScene
{
    void LoadScene();
}

public class LoadScene : ILoadScene{
    public int scena;
    public LoadScene(int scena){
        this.scena = scena;
    }

    void ILoadScene.LoadScene(){
        SceneManager.LoadScene(this.scena);
    }
}


public class Student
{
    string firstName;
    string lastName;
    string email;
    public Student(string meno, string priezvisko, string email){
        this.firstName = meno;
        this.lastName = priezvisko;
        this.email = email;
    }
}