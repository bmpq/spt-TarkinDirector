using Comfort.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Systems.Effects;
using tarkin.BSP.Shared;

namespace tarkin.BSP.Bep.Mediators
{
    internal class EffectEmitterMediator
    {
        public EffectEmitterMediator() 
        {
            EffectEmitter.OnRequest += EffectEmitter_OnRequest;
        }

        private void EffectEmitter_OnRequest(EffectRequest req)
        {
            Effects.Effect effect = Singleton<Effects>.Instance.EffectsArray.Where(e => e.Name == req.Name).FirstOrDefault();
            if (effect == null)
                return;
            Singleton<Effects>.Instance?.AddEffectEmit(effect, req.Position, req.Normal, null, req.DrawDecal, req.Volume);
        }
    }
}
