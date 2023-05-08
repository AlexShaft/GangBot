using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Networking.Auth.Discord
{
    public class DiscordOAuthManager : MonoBehaviour
    {
        [SerializeField] private string clientId;
        [SerializeField] private string clientSecret;
        [SerializeField] private string redirectUri;
        [SerializeField] private string scope = "identify guilds";
        [SerializeField] private string editor_redirect_uri = "";

        private string _code;
        private string _accessToken;

        public static DiscordOAuthManager instance;

        public string AccessToken { get { return _accessToken; } }

        public event Action<string> Authorized;

        private void Awake()
        {
            instance= this;
        }

        public void StartOAuth2Flow()
        {
            var authUrl = $"https://discord.com/api/oauth2/authorize?client_id={clientId}&redirect_uri={UnityWebRequest.EscapeURL(redirectUri)}&response_type=code&scope={UnityWebRequest.EscapeURL(scope)}";

            // Open the Discord OAuth2 URL in a new browser window
            Application.OpenURL(authUrl);
        }

#if UNITY_EDITOR
        [ContextMenu("Simulate OnRedirectUriCalled")]
        public void SimulateOnRedirectUriCalled()
        {
            OnRedirectUriCalled(editor_redirect_uri);
        }
#endif


        public void OnRedirectUriCalled(string uri)
        {
            Debug.LogError("[OnRedirectUriCalled] " + uri);

            // Extract the code from the URI
            _code = GetCodeFromUri(uri);

            Login(_code, redirectUri, res =>
            {
                Debug.LogError("response for login : " + res);
                var loginData = JsonConvert.DeserializeObject<LoginResponse>(res);
                NetworkManager.Instance.AccessToken = loginData.token;
                Authorized?.Invoke(loginData.userName);
            });
        }

        public void Login(string code, string redirectUri, Action<string> callback)
        {
            NetworkManager.Instance.Get($"https://localhost:7228/api/Auth/discord?code={code}&redirectUri={redirectUri}", callback);
        }

        private string GetCodeFromUri(string uri)
        {
            var uriObj = new Uri(uri);
            var query = uriObj.Query.TrimStart('?');

            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            var queryParams = query.Split('&');

            foreach (var param in queryParams)
            {
                var keyValue = param.Split('=');

                if (keyValue.Length == 2 && keyValue[0] == "code")
                {
                    return keyValue[1];
                }
            }

            return null;
        }

        [System.Serializable]
        public class LoginResponse
        {
            public string token;
            public string userName;
            public int expires_in;
        }
    }
}
