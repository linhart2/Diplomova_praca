using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public bool loadScene = true;
    [SerializeField]
    private Text loadingText;

    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        loadingText.text = "Loading...";

        if (loadScene == true)
        {

            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));

        }
    }
}
