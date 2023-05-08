using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GangBot
{
    public class WaitingScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        public void SetText(string text)
        {
            label.text = text; 
        }
    }
}
