using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour
{

    public Button btnOfflineVersion;
    public Button btnOnlineVersion;
    public Canvas loading;
    public Text txtTest;
    FirebaseCommunicationLibrary fbC;
    private PlayerData playerData = new PlayerData();
    private bool pressBtnOnlineVersion = false;

    void Start()
    {
        if (loading != null) loading = loading.GetComponent<Canvas>();
        if (loading != null) loading.enabled = false;

        fbC = new FirebaseCommunicationLibrary();
        playerData = GlobalData.playerData;
        btnOfflineVersion = btnOfflineVersion.GetComponent<Button>();
        btnOnlineVersion.onClick.AddListener(delegate
        {
            Show_loading();
            pressBtnOnlineVersion = true;
            if (!playerData.LoggedUser)
            {
                SceneManager.LoadScene(17);
            }


        });
    }

    private void Update()
    {
        if (pressBtnOnlineVersion && playerData.LoggedUser && !string.IsNullOrEmpty(playerData.Name))
        {
            Hide_loading();
            pressBtnOnlineVersion = false;
            SceneManager.LoadScene(19);
        }
    }

    public void Show_loading()
    {
        loading.enabled = true;
        StartCoroutine(Hide_loading());
    }

    IEnumerator Hide_loading()
    {
        yield return new WaitForSeconds(15.0f);

        loading.enabled = false;
        Debug.Log("Skontroluj pripojenie");
    }
}
