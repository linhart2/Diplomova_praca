using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimationBack : MonoBehaviour {

    public Button back;    

    // Use this for initialization
    void Start () {
        back = back.GetComponent<Button>();       
    }

    public void BackLevel()

    {        
        Application.LoadLevel(0);
    }
}
