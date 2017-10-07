using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class FirebaseConnect {
	DatabaseReference mDatabaseRef;
	public string JsonData;

	public FirebaseConnect(){
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://sumrectangle.firebaseio.com/");
		mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
	}

	public void UpdateResult(Dictionary<string,string> ex) {
		//string key = mDatabaseRef.Child("scores").Push().Key;
		LeaderBoardEntry entry = new LeaderBoardEntry(ex);
		Dictionary<string, object> entryValues = entry.ToDictionary();

		Dictionary<string, object> childUpdates = new Dictionary<string, object>();
		childUpdates["/Class_ID/member/email/example/result/"] = entryValues;

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

//[System.Serializable]
public class LeaderBoardEntry {
	public Dictionary<string,string> exam;

	public LeaderBoardEntry() {
	}

	public LeaderBoardEntry(Dictionary<string,string> exam) {
		this.exam = exam;
	}

	public Dictionary<string, object> ToDictionary() {
		Dictionary<string, object> result = new Dictionary<string, object>();
		foreach (KeyValuePair<string, string> item in exam)
		{
			result[item.Key] = item.Value;
		}
		return result;
	}
}