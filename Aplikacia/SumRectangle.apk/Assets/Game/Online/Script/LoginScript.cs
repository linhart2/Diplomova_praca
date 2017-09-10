using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LoginScript : MonoBehaviour {

	public Button btnLogin;
	public Button btnRegistration;
	public InputField txtLoginName;
	public InputField txtLoginPassworld;
	public Firebase.Auth.FirebaseAuth auth;
	public Firebase.Auth.FirebaseUser user;

	void Start () {
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		btnRegistration.onClick.AddListener (delegate {
			RegisterNewAccount (txtLoginName.text, txtLoginPassworld.text);
		});
		//RegisterNewAccount("lingo3993@azet.sk","Test1234");
	}

	public void RegisterNewAccount(string email, string password) {
		auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
			if (task.IsCanceled) {
				Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				return;
			}
			Firebase.Auth.FirebaseUser newUser = task.Result;
			Debug.LogFormat("User signed in successfully: {0} ({1})",
				newUser.DisplayName, newUser.UserId);
			Application.LoadLevel(19); 
		});
	}




}
