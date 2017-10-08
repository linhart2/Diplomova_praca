using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowClassScript : MonoBehaviour {

	FirebaseCommunicationLibrary fbC;
    public Text txtLoggedUser;
    public Button btnOdhlasit;

	void Start () {
		fbC = new FirebaseCommunicationLibrary();
        fbC.GetUserData(new SetText(txtLoggedUser));
        btnOdhlasit.onClick.AddListener(delegate {
			fbC.OnDestroy(new LoadScene(17));
		});
	}

	
}
