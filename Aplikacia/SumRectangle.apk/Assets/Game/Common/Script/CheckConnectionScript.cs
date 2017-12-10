using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckConnectionScript : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Check internet connection !!!");
        }
        else
        {
            SceneManager.LoadScene("Loading");
        }
    }
}
