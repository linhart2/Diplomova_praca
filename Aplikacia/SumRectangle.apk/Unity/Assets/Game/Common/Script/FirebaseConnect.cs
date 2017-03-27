using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class FirebaseCommunication {
	DatabaseReference mDatabaseRef;
	public string JsonData;

	public FirebaseCommunication(){
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://sumrectangle.firebaseio.com/");
		mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;

		FirebaseDatabase.DefaultInstance
			.GetReference("/Class_ID/member/email/example/result/")
			.ValueChanged += HandleValueChanged;
	}

	public void CreateNewClass(string class_id, string name) {
		//User user = new User(name, email);
		//string json = JsonUtility.ToJson(user);
		User user = new User(name, name);
		string json = JsonUtility.ToJson(user);
		mDatabaseRef.Child("test").SetValueAsync("ssssssss");

		mDatabaseRef.Child(class_id).Child("class_name").SetValueAsync(name);
		mDatabaseRef.Child (class_id).Child ("member").Child ("email").Child ("student_name").SetValueAsync ("danielLinhart");
		mDatabaseRef.Child (class_id).Child ("member").Child ("email").Child ("example").Child("result").SetValueAsync ("000000000");
		mDatabaseRef.Child (class_id).Child ("member").Child ("email").Child ("example").Child("result_bool").SetValueAsync ("true");
	}

	public void UpdateResult(string examp) {
		// Create new entry at /user-scores/$userid/$scoreid and at
		// /leaderboard/$scoreid simultaneously
		//string key = mDatabaseRef.Child("scores").Push().Key;

		Dictionary<string, object> childUpdates = new Dictionary<string, object>();
		childUpdates["/Class_ID/member/email/example/result"] = examp;

		mDatabaseRef.UpdateChildrenAsync(childUpdates);
	}

	public void ReadData(string key){
		FirebaseDatabase.DefaultInstance
			.GetReference(key)
			.GetValueAsync().ContinueWith(task => {
				if (task.IsFaulted) {
					// Handle the error...
				}
				else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;

					// Do something with snapshot...
					Debug.Log(snapshot.ToString());
				}
			});
	}

	void HandleValueChanged(object sender, ValueChangedEventArgs args) {
		if (args.DatabaseError != null) {
			Debug.LogError(args.DatabaseError.Message);
			return;
		}
		//Debug.Log (args.Snapshot);
		// Do something with the data in args.Snapshot
	}
		
}

public class User {
	public string username;
	public string email;

	public User() {
	}

	public User(string username, string email) {
		this.username = username;
		this.email = email;
	}
}

public class LeaderBoardEntry {
	public string uid;
	public int score = 0;

	public LeaderBoardEntry() {
	}

	public LeaderBoardEntry(string uid, int score) {
		this.uid = uid;
		this.score = score;
	}

	public Dictionary<string, object> ToDictionary() {
		Dictionary<string, object> result = new Dictionary<string, object>();
		result["uid"] = uid;
		result["score"] = score;

		return result;
	}
}