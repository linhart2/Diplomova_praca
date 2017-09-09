using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using System;

public class LoginScript : MonoBehaviour {

	public Button btnLogin;
	public Button btnRegistration;
	public InputField txtLoginName;
	public InputField txtLoginPassworld; 

	void Start () {
		Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

		auth.CreateUserWithEmailAndPasswordAsync ("danko.linhart@gmail.com", "123456").ContinueWith (task => {
			if(task.IsCanceled){
				Debug.Log("Canceled");
				return;
			}
			if(task.IsFaulted){
				Debug.Log("faulted");
				return;
			}
			Firebase.Auth.FirebaseUser newUser = task.Result;
			Debug.Log(newUser.UserId);
			if(task.IsCompleted){
				Debug.Log("Completed");
			}
		});
	}
}
