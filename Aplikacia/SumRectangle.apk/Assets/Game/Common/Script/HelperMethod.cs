using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelperMethod : MonoBehaviour {

	public Canvas quitMenu;
	public Canvas question;
	public Animator anim;

	void Start () {
		
		if (quitMenu != null) quitMenu = quitMenu.GetComponent<Canvas>();
		if (quitMenu != null)quitMenu.enabled = false;
		if (question != null)question = question.GetComponent<Canvas>();
		if (question != null)question.enabled = false;
		if (anim != null)anim = question.GetComponent<Animator>();
		if (anim != null)anim.enabled = false;
	}

	public void SwitchScene(int value)
	{
		Application.LoadLevel(value); 
	}

	public void StartLevel(string value1) //this function will be used on our Play button
	{
		string[] val = value1.Split (',');
		int value = Random.RandomRange(System.Convert.ToInt32 (val [0]), System.Convert.ToInt32 (val [1]));
		Application.LoadLevel (value);
	}

	public void Show_question()
	{
		if (anim != null)anim.enabled = true;
		question.enabled = true;
	}

	public void Hide_question()
	{
		if (anim != null)anim.enabled = false;
		question.enabled = false;
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
