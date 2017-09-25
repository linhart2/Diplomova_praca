using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseCommunicationLibrary{
	public Firebase.Auth.FirebaseAuth auth;
	public Firebase.Auth.FirebaseUser user;

	public FirebaseCommunicationLibrary(){
		auth = FirebaseAuthInit ();
	}

	private Firebase.Auth.FirebaseAuth FirebaseAuthInit(){
		return Firebase.Auth.FirebaseAuth.DefaultInstance;
	}

	public void RegistrationNewAccount(string meno,string priezvisko,string email,string heslo,string hesloAgain){
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
				//RegistrationSaveData(meno,priezvisko,email);
			}
		});
	}

	public void RegistrationSaveData(string meno,string priezvisko,string email, string userId ) {
		
	}

	public void FirebaseDatabaseInit(){
		
	} 
}

public class Student
{
	public string firstName {
		get;
		set;
	}
	public string lastName {
		get;
		set;
	}
	public string email {
		get;
		set;
	}
	public string password {
		get;
		set;
	}
}