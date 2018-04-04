using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class LoginScript : MonoBehaviour
{

    FirebaseCommunicationLibrary fbc;
    public Button btnLogin;
    public Button btnRegistration;
    public InputField txtLoginName;
    public InputField txtLoginPassworld;
    private Color red = new Color(1, 0, 0, 1);
    private Color white = new Color(255, 255, 255, 1);


    private void Awake()
    {
        fbc = new FirebaseCommunicationLibrary();
    }

    void Start()
    {
        GlobalData.playerData.SelectedClass = null;

        btnLogin.onClick.AddListener(delegate
        {
            Login(txtLoginName.text, txtLoginPassworld.text);
        });
    }

    public void Login(string email, string password)
    {
        setFieldColor(txtLoginName, white);
        if (!IsValidEmail(txtLoginName.text))
        {
            setFieldColor(txtLoginName, red);
            return;
        }
        fbc.Login(email, password, new LoadScene(19), GameObject.Find("CHYBA").GetComponent<Text>());
    }

    private void setFieldColor(InputField name, Color color)
    {
        ColorBlock cb = name.colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        name.colors = cb;
    }

    private bool IsValidEmail(string strIn)
    {
        return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }

}
