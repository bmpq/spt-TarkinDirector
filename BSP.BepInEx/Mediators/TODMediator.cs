using Comfort.Common;
using EFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tarkin.BSP.Shared;

namespace tarkin.BSP.Bep.Mediators
{
    internal class TODMediator
    {
        public TODMediator() 
        {
            TODModifier.OnTODChangeRequested += TODModifier_OnTODChangeRequested;
        }

        private void TODModifier_OnTODChangeRequested(int minutesSinceMidnight)
        {
            DateTime currentDateTime = Singleton<GameWorld>.Instance.GameDateTime.Calculate();
            DateTime modifiedDateTime = currentDateTime.Date + TimeSpan.FromMinutes(minutesSinceMidnight);

            Singleton<GameWorld>.Instance.GameDateTime.Reset(modifiedDateTime);
        }
    }
}
