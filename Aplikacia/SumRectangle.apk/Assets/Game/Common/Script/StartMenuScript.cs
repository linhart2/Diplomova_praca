using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour
{
    private PlayerData playerData = new PlayerData();

    void Start()
    {
        playerData = GlobalData.playerData;
        GlobalData.playerData.SelectedClass = null;
    }
}
