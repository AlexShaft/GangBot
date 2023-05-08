using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Networking
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance;
        private void Awake()
        {
            Instance = this;
        }

        public string AccessToken { get; set; }

        public event Action<string> UnauthorizedError;

        IEnumerator GetRequest(string uri, Action<string> callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");
                webRequest.SetRequestHeader("Authorization", $"Bearer {AccessToken}");

                Debug.LogError("check access token : " + AccessToken);

                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        UnauthorizedError?.Invoke(webRequest.error);
                        callback(null);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        UnauthorizedError?.Invoke(webRequest.error);
                        callback(null);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        callback(webRequest.downloadHandler.text);
                        break;
                    default:
                        UnauthorizedError?.Invoke(webRequest.error);
                        callback(null);
                        break;
                }
            }
        }

        IEnumerator PostRequest(string uri, Action callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, "", "json"))
            {
                webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");
                webRequest.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        UnauthorizedError?.Invoke(webRequest.error);
                        callback();
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        UnauthorizedError?.Invoke(webRequest.error);
                        callback();
                        break;
                    case UnityWebRequest.Result.Success:
                        callback();
                        break;
                    default:
                        UnauthorizedError?.Invoke(webRequest.error);
                        callback();
                        break;
                }
            }
        }

        public void Get(string url, Action<string> callback)
        {
            StartCoroutine(GetRequest(url, res => {
                if (!string.IsNullOrEmpty(res))
                {
                    callback(res);
                }
                else
                {
                    callback(default);
                }
            }));
        }

        public void Get<T>(string url, Action<T> callback)
        {
            StartCoroutine(GetRequest(url, res => {
                if (!string.IsNullOrEmpty(res))
                {
                    var obj = JsonConvert.DeserializeObject<T>(res);
                    callback(obj);
                }
                else
                {
                    callback(default);
                }
            }));
        }

        public void Post(string url, Action callback)
        {
            StartCoroutine(PostRequest(url, callback));
        }
    }
}
