using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreateLevelScript_menu : MonoBehaviour {


    public Button uroven1;
    public Button uroven2;
    public Button uroven3;

    // Use this for initialization
    void Start () {
        uroven1 = uroven1.GetComponent<Button>();
        uroven2 = uroven2.GetComponent<Button>();
        uroven3 = uroven3.GetComponent<Button>();


    }

    public void StartLevel_1() //this function will be used on our Play button

    {
        Application.LoadLevel(10); //this will load our first level from our build settings. "1" is the second scene in our game

    }
    public void StartLevel_2() //this function will be used on our Play button

    {
        Application.LoadLevel(11); //this will load our first level from our build settings. "1" is the second scene in our game

    }
    public void StartLevel_3() //this function will be used on our Play button

    {
        Application.LoadLevel(12); //this will load our first level from our build settings. "1" is the second scene in our game

    }
}
