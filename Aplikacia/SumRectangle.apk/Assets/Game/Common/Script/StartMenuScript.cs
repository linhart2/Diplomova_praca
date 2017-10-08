using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour {

	public Button btnOfflineVersion;
	public Button btnOnlineVersion;
    public Text txtTest;
    FirebaseCommunicationLibrary fbC;
    private PlayerData playerData = new PlayerData();

	void Start () {
        fbC = new FirebaseCommunicationLibrary();
        playerData = GlobalData.playerData;
		btnOfflineVersion = btnOfflineVersion.GetComponent<Button>();
		btnOnlineVersion.onClick.AddListener(delegate {
            if (playerData.LoggedUser)
            {
                SceneManager.LoadScene(19);
            }
            else
            {
                SceneManager.LoadScene(17);
            }
		});
	}
}
