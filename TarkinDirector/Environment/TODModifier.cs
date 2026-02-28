using System;
using UnityEngine;

#if EFT_RUNTIME
using Comfort.Common;
using EFT;
#endif

namespace tarkin.Director
{
    public class TODModifier : MonoBehaviour
    {
        [Range(0, 23)]
        [SerializeField] private int todHour = 12;
        [Range(0, 59)]
        [SerializeField] private int todMinute = 00;

        void OnEnable()
        {

#if EFT_RUNTIME
            float minutesSinceMidnight = (todHour * 60) + todMinute;

            DateTime currentDateTime = Singleton<GameWorld>.Instance.GameDateTime.Calculate();
            DateTime modifiedDateTime = currentDateTime.Date + TimeSpan.FromMinutes(minutesSinceMidnight);

            Singleton<GameWorld>.Instance.GameDateTime.Reset(modifiedDateTime);
#endif
        }
    }
}
