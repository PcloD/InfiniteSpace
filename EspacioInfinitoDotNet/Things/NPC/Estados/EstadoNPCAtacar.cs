using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things.NPC.Estados
{
    public class EstadoNPCAtacar : EstadoNPC
    {
        public const int DistanciaDisparo = 750;
        public const int DistanciaAcercamiento = 500;

        private ThingDaniable thingAAtacar;
        private float tiempoIntentosFallidos = 0;

        public EstadoNPCAtacar(ThingNaveNPC nave, ThingDaniable thingAAtacar)
            : base(nave)
        {
            this.thingAAtacar = thingAAtacar;
        }

        public override string Describir()
        {
            return "Atacando";
        }

        public override EstadoNPC OnDaniadoPor(Thing thing)
        {
            if (thing is ThingNave)
            {
                ThingNave naveAtacante = (ThingNave)thing;

                if (naveAtacante.Faccion != nave.Faccion &&
                    naveAtacante.Faccion != null)
                {
                    Faccion.RelacionConOtraFaccionEnum relacionConFaccionAtacante = nave.Faccion.GetRelacion(naveAtacante.Faccion);

                    if (relacionConFaccionAtacante != Faccion.RelacionConOtraFaccionEnum.Amigable)
                    {
                        //Si la relación con la facción a la que pertenece la nave que me ataca es distinta a amigable, la empiezo a atacar

                        thingAAtacar = naveAtacante;
                    }
                }
            }

            return this;
        }

        public override EstadoNPC Procesar(float fDeltaSegundos)
        {
            //Estoy yendo a atacar
            EstadoNPC nuevoEstado = this;

            if (!thingAAtacar.Eliminado)
            {
                Vector2 diferencia = thingAAtacar.Centro - nave.Centro;

                if (diferencia.Length > DistanciaAcercamiento)
                {
                    //Todavia no me acerque todo lo posible
                    if (tiempoIntentosFallidos < 2 && diferencia.Length < RadioBusquedaAtaque * 1.5)
                    {
                        if (!EstadoNPCNavegarA.IrA(nave, thingAAtacar.Centro, fDeltaSegundos))
                            tiempoIntentosFallidos += fDeltaSegundos;
                    }
                    else
                    {
                        //Fallé muchas veces en llegar o se alejó mucho, planifico de nuevo
                        nuevoEstado = new EstadoNPCPlanificar(nave);
                    }
                }

                diferencia = thingAAtacar.Centro - nave.Centro;
                
                if (diferencia.Length <= DistanciaDisparo)
                {
                    //Estoy cerca para atacar, roto y disparo!
                    EstadoNPCNavegarA.RotarA(nave, thingAAtacar.Centro, fDeltaSegundos);

                    nave.Shoot();

                    return this;
                }
            }
            else
            {
                //Destrui mi objetivo! Replanifico

                nuevoEstado = new EstadoNPCPlanificar(nave);
            }

            return nuevoEstado;
        }
    }
}
