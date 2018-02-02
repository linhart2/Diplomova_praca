using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private Text loadingText;
    private PlayerData playerData = new PlayerData();
    private FirebaseCommunicationLibrary fbC;

    private void Start()
    {
        GlobalData.playerData.SelectedClass = null;
        fbC = new FirebaseCommunicationLibrary();
        fbC.GetUserData(GlobalData.playerData.UserId, true);
        StartCoroutine(EndLoading());

    }

    void Update()
    {
        loadingText.text = "Loading ...";

        if (true)
        {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Check internet connection !!!");
            SceneManager.LoadScene("CheckConnection");
        }
    }



    IEnumerator EndLoading()
    {
        yield return new WaitForSeconds(30.0f);

        SceneManager.LoadScene("CheckConnection");
    }
}
