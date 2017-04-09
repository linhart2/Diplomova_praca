using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackButton : MonoBehaviour {

    public Button back;
    public Button show_question;
    public Button hide_question;
    public Canvas question;
    bool pom = true;
    
    
    void Start()
    {
        
        back = back.GetComponent<Button>();
        
        show_question = show_question.GetComponent<Button>();
        hide_question = hide_question.GetComponent<Button>();
        question = question.GetComponent<Canvas>();
        question.enabled = false;
    }

    public void BackLevel(int pom)

    {
        Application.LoadLevel(pom); 

    }

    public void Show_question()

    {
        question.enabled = true;
    }

    public void Hide_question()

    {
        question.enabled = false;
    }
}
