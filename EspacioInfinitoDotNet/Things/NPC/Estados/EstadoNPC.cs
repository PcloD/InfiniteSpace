using System;
using System.Collections.Generic;
using System.Text;

namespace EspacioInfinitoDotNet.Things.NPC.Estados
{
    public abstract class EstadoNPC
    {
        public const int RadioBusquedaRecursos = 10000;
        public const int RadioBusquedaAtaque = 3000;
        
        static protected System.Random rnd = new Random((int)DateTime.Now.Ticks);
        
        protected ThingNaveNPC nave;

        public EstadoNPC(ThingNaveNPC nave)
        {
            this.nave = nave;
        }

        public abstract EstadoNPC OnDaniadoPor(Thing thing);
        public abstract EstadoNPC Procesar(float fDeltaSegundos);
        public abstract String Describir();
    }
}
