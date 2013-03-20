using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things.NPC.Estados
{
    public class EstadoNPCNavegarA : EstadoNPC
    {
        private Vector2 destino;
        private float tiempoIntentosFallidos = 0;

        public EstadoNPCNavegarA(ThingNaveNPC nave, Vector2 destino)
            : base(nave)
        {
            this.destino = destino;
        }

        public override string Describir()
        {
            return "Navegando";
        }

        public override EstadoNPC OnDaniadoPor(Thing thing)
        {
            return new EstadoNPCPlanificar(nave).OnDaniadoPor(thing);
        }

        public static bool RotarA(ThingNaveNPC nave, Vector2 destino, float fDeltaSegundos)
        {
            Vector2 diferencia = destino - nave.Centro;

            float rotacionRequerida = diferencia.Normalized().AngleInDegress - nave.RotacionEnGrados;

            return nave.Rotar(rotacionRequerida, true, true);
        }

        public static bool IrA(ThingNaveNPC nave, Vector2 destino, float fDeltaSegundos)
        {
            Vector2 diferencia = destino - nave.Centro;

            if (RotarA(nave, destino, fDeltaSegundos))
                if (nave.MoverAdelante(nave.Velocidad * fDeltaSegundos, true, true))
                    return true;

            return false;
        }

        public override EstadoNPC Procesar(float fDeltaSegundos)
        {
            Vector2 diferencia = destino - nave.Centro;

            if (diferencia.Length > 50.0f && tiempoIntentosFallidos < 0.5f)
            {
                if (!IrA(nave, destino, fDeltaSegundos))
                    tiempoIntentosFallidos += fDeltaSegundos;

                return this;
            }
            else
            {
                return new EstadoNPCPlanificar(nave);
            }
        }
    }
}
