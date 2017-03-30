using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimationBack : MonoBehaviour {

    public Button back;
	public int navrat;

    // Use this for initialization
    void Start () {
        back = back.GetComponent<Button>();       
    }

	public void BackLevel(int value)
    {        
		Application.LoadLevel(navrat);
    }
}
