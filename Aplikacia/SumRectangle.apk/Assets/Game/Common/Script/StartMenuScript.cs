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
        btnOfflineVersion = btnOfflineVersion.GetComponent<Button>();
        btnOnlineVersion.onClick.AddListener(delegate
        {
            SceneManager.LoadScene("Loading");
        });
    }
}
