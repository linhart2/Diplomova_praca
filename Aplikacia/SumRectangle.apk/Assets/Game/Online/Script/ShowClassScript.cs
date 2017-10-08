using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowClassScript : MonoBehaviour {

	FirebaseCommunicationLibrary fbC;
    public Text txtLoggedUser;
    public Button btnOdhlasit;
    private PlayerData playerData = new PlayerData();

	void Start () {
        playerData = GlobalData.playerData;
		fbC = new FirebaseCommunicationLibrary();
        txtLoggedUser.text = string.Format("{0} {1}",txtLoggedUser.text,playerData.Name);
        btnOdhlasit.onClick.AddListener(delegate {
			fbC.OnDestroy(new LoadScene(17));
		});
	}

	
}
