using UnityEngine;
using System.Collections;
using UnityEngine.UI;// we need this namespace in order to access UI elements within our script

public class SelectLevelScript : MonoBehaviour
{
    public Button uroven1;
    public Button uroven2;
    public Button uroven3;
    public Button uroven4;
    public Button edit;
    public Button exit;
	public Button show_question;
	public Canvas quitMenu;
    public Canvas question;
    Animator anim;

    void Start()
    {
        uroven1 = uroven1.GetComponent<Button>();
        uroven2 = uroven2.GetComponent<Button>();
        uroven3 = uroven3.GetComponent<Button>();
        uroven4 = uroven4.GetComponent<Button>();
        edit = edit.GetComponent<Button>();
        exit = exit.GetComponent<Button>();
		show_question = show_question.GetComponent<Button>();
        quitMenu = quitMenu.GetComponent<Canvas>();
		question = question.GetComponent<Canvas>();
		anim = question.GetComponent<Animator>();
        quitMenu.enabled = false;
        question.enabled = false;
        anim.enabled = false;
    }

    public void Show_question()
    {
        anim.enabled = true;       
        question.enabled = true;
    }

    public void Hide_question()
    {
        anim.enabled = false;
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

    public void StartLevel_1() //this function will be used on our Play button
    {
        Application.LoadLevel(Random.RandomRange(2, 4)); //this will load our first level from our build settings. "1" is the second scene in our game

    }
    public void StartLevel_2() //this function will be used on our Play button
    {
        Application.LoadLevel(Random.RandomRange(4, 6)); //this will load our first level from our build settings. "1" is the second scene in our game

    }
    public void StartLevel_3() //this function will be used on our Play button
    {
        Application.LoadLevel(Random.RandomRange(6, 8)); //this will load our first level from our build settings. "1" is the second scene in our game

    }
    public void StartLevel_4() //this function will be used on our Play button
    {
        Application.LoadLevel(Random.RandomRange(8, 10)); //this will load our first level from our build settings. "1" is the second scene in our game

    }

    public void EditLevel() //this function will be used on our Play button
    {
        Application.LoadLevel(10); //this will load our first level from our build settings. "1" is the second scene in our game

    }
}