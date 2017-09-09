using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour {

	public Button btnOfflineVersion;
	public Button btnOnlineVersion;
	// Use this for initialization
	void Start () {
		btnOfflineVersion = btnOfflineVersion.GetComponent<Button>();
		btnOnlineVersion = btnOnlineVersion.GetComponent<Button>();
	}
}
