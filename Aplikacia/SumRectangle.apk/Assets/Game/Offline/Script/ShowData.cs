using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowData : MonoBehaviour {
	public Button uroven1;
	public Button uroven2;
	public Button exit;
	public Canvas quitMenu;

	// Use this for initialization
	void Start () {
		uroven1 = uroven1.GetComponent<Button>();
		uroven2 = uroven2.GetComponent<Button>();
		exit = exit.GetComponent<Button>();
		quitMenu = quitMenu.GetComponent<Canvas>();
		quitMenu.enabled = false;
	}
	
	public void ExitPress() //this function will be used on our Exit button

	{
		quitMenu.enabled = true; //enable the Quit menu when we click the Exit button        
	}

	public void NoPress() //this function will be used for our "NO" button in our Quit Menu

	{
		quitMenu.enabled = false; //we'll disable the quit menu, meaning it won't be visible anymore        
	}
	public void YesPress() //This function will be used on our "Yes" button in our Quit menu
	{
		Application.Quit(); //this will quit our game. Note this will only work after building the game
	}
}
