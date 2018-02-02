using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour
{

    public Button btnOfflineVersion;
    public Button btnOnlineVersion;
    private PlayerData playerData = new PlayerData();

    void Start()
    {
        playerData = GlobalData.playerData;
        GlobalData.playerData.SelectedClass = null;
        btnOfflineVersion = btnOfflineVersion.GetComponent<Button>();
        btnOnlineVersion = btnOfflineVersion.GetComponent<Button>();
    }
}
