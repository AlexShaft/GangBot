using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Security.Cryptography;
using Assets.Networking.Auth.Discord;

namespace Assets.Networking.DataBase.RedisProvider
{
    public class RedisProviderService
    {
        public RedisProviderService()
        {
        }

        

        public void GetValue<T>(string key, Action<T> callback)
        {
            NetworkManager.Instance.Get<T>($"https://localhost:7228/Redis?key={key}", callback);
        }

        public void SetValue<T>(string key, T value, Action callback)
        {
            var json = JsonConvert.SerializeObject(value);

            NetworkManager.Instance.Post($"https://localhost:7228/Redis?key={key}&val={json}", callback);
        }
    }
}
