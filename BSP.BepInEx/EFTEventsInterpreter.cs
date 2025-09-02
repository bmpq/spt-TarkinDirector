using EFT.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tarkin.BSP.Bep
{
    internal static class EFTEventsInterpreter
    {
        public static event Action<EDoorState> OnDoorStateChanged;

        public static void SendDoorStateChanged(EDoorState newState)
        {
            OnDoorStateChanged?.Invoke(newState);
        }
    }
}
