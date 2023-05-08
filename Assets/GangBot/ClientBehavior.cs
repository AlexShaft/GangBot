using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Networking.DataBase.RedisProvider;
using Assets.GangBot;
using UnityEngine.UI;
using TMPro;
using Assets.Networking.Auth.Discord;
using Assets.Networking;

public class ClientBehavior : MonoBehaviour
{
    public static ClientBehavior Instance { get; private set; }

    public RedisProviderService RedisProviderService { get; private set; }

    [SerializeField] private WaitingScreen waitingScreenModel;
    [SerializeField] private TextMeshProUGUI playerName;

   

    public string PlayerName { get; private set; }

    void Awake()
    {       

        Instance = this;       

    }

    private void Start()
    {
        NetworkManager.Instance.UnauthorizedError += Instance_UnauthorizedError;
        DiscordOAuthManager.instance.Authorized += OnLogin;
        DiscordOAuthManager.instance.StartOAuth2Flow();
        ShowWaiting("Authorizing...");
    }

    private void Instance_UnauthorizedError(string error)
    {
        ShowWaiting(error);
    }

    public void OnLogin(string name)
    {
        PlayerName = name;
        playerName.text = PlayerName;
        HideWaiting();
        RedisProviderService = new RedisProviderService();
    }

    public void ShowWaiting(string message)
    {
        waitingScreenModel.gameObject.SetActive(true);
        waitingScreenModel.SetText(message);
    }

    public void HideWaiting()
    {
        waitingScreenModel.gameObject.SetActive(false);
    }
}
