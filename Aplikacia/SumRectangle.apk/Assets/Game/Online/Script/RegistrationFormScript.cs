using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class RegistrationFormScript : MonoBehaviour {

	FirebaseCommunicationLibrary fbC;
	public InputField txtMeno;
	public InputField txtPriezvisko;
	public InputField txtEmail;
	public InputField txtHeslo;
	public InputField txtHesloAgain;
	public Button btnRegistrovat;
	private Color red = new Color (1, 0, 0, 1);
	private Color white = new Color (255, 255, 255, 1);


	// Use this for initialization
	void Start () {
		fbC = new FirebaseCommunicationLibrary ();		
		btnRegistrovat.onClick.AddListener (delegate {
			registrovat();
		});
	}
	private void registrovat (){
		setFieldColor (txtMeno, white);
		setFieldColor (txtPriezvisko, white);
		setFieldColor (txtEmail, white);
		setFieldColor (txtHeslo, white);
		setFieldColor (txtHesloAgain, white);
		if (!IsValidName (txtMeno.text)) {			
			setFieldColor (txtMeno, red);
			return;
		}
		if (!IsValidName (txtPriezvisko.text)) {
			setFieldColor (txtPriezvisko, red);
			return;
		}
		if (!IsValidEmail (txtEmail.text)) {
			setFieldColor (txtEmail, red);
			return;
		}
		if (!IsValidPassword (txtHeslo.text, txtHesloAgain.text)) {
			setFieldColor (txtHeslo, red);
			setFieldColor (txtHesloAgain, red);
			return;
		}
        fbC.RegistrationNewAccount(txtMeno.text, txtPriezvisko.text, txtEmail.text, txtHeslo.text, new LoadScene(19));
	}

	private void setFieldColor(InputField name, Color color){
		ColorBlock cb = name.colors;
		cb.normalColor = color;
		cb.highlightedColor = color;
		name.colors = cb;
	}

	private bool IsValidName(string name){
		if (name.Equals ("")) 
			return false;
		return true;
	}
	private bool IsValidEmail(string strIn)
	{
		return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"); 
	}
	private bool IsValidPassword(string pass1, string pass2){
		if (pass1.Length < 6)
			return false;
		if (pass1.Equals (pass2))
			return true;
		return false;
	}

}