using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour
{

    public Button btnOfflineVersion;
    public Button btnOnlineVersion;
    private PlayerData playerData = new PlayerData();

    private void Awake()
    {
        btnOfflineVersion = btnOfflineVersion.GetComponent<Button>();
        btnOnlineVersion = btnOfflineVersion.GetComponent<Button>();
    }
    void Start()
    {
        playerData = GlobalData.playerData;
        GlobalData.playerData.SelectedClass = null;
    }
}
