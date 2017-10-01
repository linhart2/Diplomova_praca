using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowClassScript : MonoBehaviour {

	FirebaseCommunicationLibrary fbC;
    public Text txtShowInicialFirstName;
    public Text txtShowInicialLastName;
    public Button btnShowName;

	void Start () {
		fbC = new FirebaseCommunicationLibrary();
        txtShowInicialFirstName.text = fbC.InicialFName;
        txtShowInicialLastName.text = fbC.InicialLName;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
