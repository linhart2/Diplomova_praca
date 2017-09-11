using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationFormScript : MonoBehaviour {

	FirebaseCommunicationLibrary fbC;
	public InputField txtMeno;
	public InputField txtPriezvisko;
	public InputField txtEmail;
	public InputField txtHeslo;
	public InputField txtHesloAgain;
	public Button btnRegistrovat;


	// Use this for initialization
	void Start () {
		fbC = new FirebaseCommunicationLibrary ();
		btnRegistrovat.onClick.AddListener (delegate {
			fbC.RegistrationNewAccount(txtMeno.text,txtPriezvisko.text,txtEmail.text,txtHeslo.text,txtHesloAgain.text);
		});
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
