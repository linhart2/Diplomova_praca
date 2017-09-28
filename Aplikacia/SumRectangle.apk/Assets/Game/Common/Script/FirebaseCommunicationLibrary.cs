﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;

public class FirebaseCommunicationLibrary
{

	private Firebase.Auth.FirebaseAuth auth;
	Firebase.Auth.FirebaseUser user;
	private DatabaseReference mDatabaseRef;
	private ILoadScene scena;

	public FirebaseCommunicationLibrary()
	{
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://sumrectangle.firebaseio.com/");
		mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
		//auth.StateChanged += AuthStateChanged;
		//AuthStateChanged(this, null);
	}

	/*void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                SceneManager.LoadScene(18);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                SceneManager.LoadScene(19);
            }
        }
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }*/

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
				this.scena.LoadScene();
			}
		});
	}

	public void Login(string email, string heslo, ILoadScene scena)
	{
		this.scena = scena;
		Firebase.Auth.Credential credential =
					Firebase.Auth.EmailAuthProvider.GetCredential(email, heslo);
		auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
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
			this.scena.LoadScene();
		});
	}

	private void RegistrationSaveData(Student student, string userId)
	{
		mDatabaseRef.Child("USERS").Child(userId).SetRawJsonValueAsync(JsonUtility.ToJson(student));
	}



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
		this.firstName = firstName;
		this.lastName = lastName;
		this.email = email;

	}
}