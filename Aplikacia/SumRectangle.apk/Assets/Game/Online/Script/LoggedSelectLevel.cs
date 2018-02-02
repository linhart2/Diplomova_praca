using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoggedSelectLevel : MonoBehaviour
{
    private FirebaseCommunicationLibrary fbc;
    private PlayerData playerData = new PlayerData();

    // Use this for initialization
    void Start()
    {
        playerData = GlobalData.playerData;
        fbc = new FirebaseCommunicationLibrary();
#if DEBUG
        playerData.Name = "TestLingo";
        playerData.UserId = "ZAT4DktlgdYBVGwXYRpOfA3temm1";
        playerData.SelectedClass = "-KweS-rI-bTBYVX3g3vS";
        playerData.LoggedUser = true;
#endif
    }
    public void BackToSelectClass(string value)
    {
        if (playerData.SelectedClass != null)
        {
            fbc.setSelectedClass(playerData.UserId, "null");
            fbc.removeOfflineStudent(playerData.SelectedClass, playerData.UserId);
        }
        SceneManager.LoadScene(value);
    }
}
