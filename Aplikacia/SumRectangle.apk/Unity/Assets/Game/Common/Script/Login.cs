using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {
	public Button connect;
	public Button skip;

	// Use this for initialization
	void Start () {
		connect = connect.GetComponent<Button>();
		skip = skip.GetComponent<Button>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Connect() //this function will be used on our Play button
	{
		Application.LoadLevel(17); //this will load our first level from our build settings. "1" is the second scene in our game

	}
	public void Skip() //this function will be used on our Play button
	{
		Application.LoadLevel(1); //this will load our first level from our build settings. "1" is the second scene in our game

	}
}
