using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GangBot
{
    public class LocationInteractionModel : MonoBehaviour
    {
        [SerializeField] private Button actionButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button completeButton;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject buttonPanel;
        [SerializeField] private string location;

        private long lastGangedDate = 0;
        private string[] gangers;

        public void Start()
        {
            RedisProviderService_Inited();
        }

        private void RedisProviderService_Inited()
        {
            UpdateData(() =>
            {
                actionButton.onClick.AddListener(() =>
                {
                    buttonPanel.SetActive(true);
                });
                startButton.onClick.AddListener(async () =>
                {
                    if (gangers == null || gangers.Length == 0)
                    {
                        gangers = new string[] { ClientBehavior.Instance.PlayerName };
                    }
                    else
                    {
                        var list = gangers.ToList();
                        list.Add(ClientBehavior.Instance.PlayerName);
                        gangers = list.ToArray();
                    }

                    var dto = new LocationDTO() { Gangers = gangers };
                    ClientBehavior.Instance.RedisProviderService.SetValue(location, dto, () =>
                    {
                        UpdateData(() =>
                        {
                            buttonPanel.SetActive(false);
                        });
                    });

                });
                completeButton.onClick.AddListener(async () =>
                {
                    var dto = new LocationDTO() { Gangers = null, lastGangedDate = DateTime.UtcNow.ToFileTimeUtc() };
                    ClientBehavior.Instance.RedisProviderService.SetValue(location, dto, () =>
                    {
                        UpdateData(() =>
                        {
                            buttonPanel.SetActive(false);
                        });
                    });
                });

                InvokeRepeating("UpdateTimer", 0, 1);
                InvokeRepeating("UpdateDataRepeated", 30, 30);
            });
        }

        private void UpdateDataRepeated()
        {
            UpdateData(null);

        }

        private void UpdateData(Action callback)
        {
            ClientBehavior.Instance.RedisProviderService.GetValue<LocationDTO>(location, dto =>
            {
                if (dto == null)
                {
                    dto = new LocationDTO();
                    dto.lastGangedDate = DateTime.Now.ToFileTimeUtc();
                }
                lastGangedDate = dto.lastGangedDate;
                gangers = dto.Gangers;

                callback?.Invoke();
            });

        }

        private void UpdateTimer()
        {
            if (gangers != null && gangers.Length > 0)
            {
                timerText.SetText($"Ganging by {string.Join(", ", gangers)}");
                timerText.color = Color.yellow;
                return;
            }

            if (lastGangedDate >= 0)
            {
                var date = DateTime.FromFileTimeUtc(lastGangedDate);
                var diff = DateTime.UtcNow - date;
                timerText.SetText(diff.ToString("hh\\:mm\\:ss"));

                var tmp = new TimeSpan(0,20,0);
                if (diff > tmp)
                {
                    timerText.color = Color.red;
                }
                else
                {
                    timerText.color = Color.green;
                }
            }
        }

        public void Update() {
        
        }
    }
}
