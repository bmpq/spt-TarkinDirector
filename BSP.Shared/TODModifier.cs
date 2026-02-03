using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class TODModifier : MonoBehaviour
    {
        public static event Action<int> OnTODChangeRequested;

        [Range(0, 23)]
        [SerializeField] private int todHour = 12;
        [Range(0, 59)]
        [SerializeField] private int todMinute = 00;

        void OnEnable()
        {
            OnTODChangeRequested?.Invoke(todHour * 60 + todMinute);
        }
    }
}
