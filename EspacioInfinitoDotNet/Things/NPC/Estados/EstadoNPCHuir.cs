using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things.NPC.Estados
{
    public class EstadoNPCHuir : EstadoNPC
    {
        private Thing thingAtacante;
        private float tiempoIntentosFallidos = 0;

        public EstadoNPCHuir(ThingNaveNPC nave, Thing thingAtacante)
            : base(nave)
        {
            this.thingAtacante = thingAtacante;
        }

        public override string Describir()
        {
            return "Huyendo";
        }

        public override EstadoNPC OnDaniadoPor(Thing thing)
        {
            return this;
        }

        public override EstadoNPC Procesar(float fDeltaSegundos)
        {
            //Estoy huyendo!!

            Vector2 diferencia = thingAtacante.Centro - nave.Centro;

            if (!thingAtacante.Eliminado &&
                diferencia.Length < RadioBusquedaAtaque)
            {
                //Todavia no estoy lo suficientemente lejos como para "relajarme"
                if (tiempoIntentosFallidos < 2)
                {
                    //La rotacion requerida para alejarme es la del vector que separa los centros de las naves, mas 180 grados
                    //para que vaya en la dirección opuesta!
                    float rotacionRequerida = diferencia.Normalized().AngleInDegress - nave.RotacionEnGrados + 180.0f;

                    if (rotacionRequerida > 360.0f)
                        rotacionRequerida -= 360.0f;

                    if (nave.Rotar(rotacionRequerida, true, true))
                    {
                        if (!nave.MoverAdelante(nave.Velocidad * fDeltaSegundos, true, true))
                        {
                            tiempoIntentosFallidos += fDeltaSegundos;
                        }
                    }
                    else
                        tiempoIntentosFallidos += fDeltaSegundos;

                    return this;
                }
                else
                {
                    //Fallé muchas veces en alejarme, planifico de nuevo
                    return new EstadoNPCPlanificar(nave);
                }
            }
            else
            {
                //Mi atacante fue destruido o me aleje lo suficiente, replanifico!
                return new EstadoNPCPlanificar(nave);
            }
        }
    }
}
