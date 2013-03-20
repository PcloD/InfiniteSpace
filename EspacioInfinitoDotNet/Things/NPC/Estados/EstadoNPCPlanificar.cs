using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;
using EspacioInfinitoDotNet.Universes;

namespace EspacioInfinitoDotNet.Things.NPC.Estados
{
    public class EstadoNPCPlanificar : EstadoNPC
    {
        public EstadoNPCPlanificar(ThingNaveNPC nave) : base(nave) { }

        public override string Describir()
        {
            return "Planificando";
        }

        public override EstadoNPC OnDaniadoPor(Thing thing)
        {
            if (thing is ThingNave)
            {
                ThingNave naveAtacante = (ThingNave)thing;

                Faccion.RelacionConOtraFaccionEnum relacionConFaccionAtacante = nave.Faccion.GetRelacion(naveAtacante.Faccion);

                switch (nave.Faccion.TipoFaccion)
                {
                    case Faccion.TipoFaccionEnum.Agresiva:
                        //Si la nave atacada pertenece a una facción agresiva, ataca de vuelta siempre! (al menos que sea de la misma facción)
                        return new EstadoNPCAtacar(nave, naveAtacante);

                    case Faccion.TipoFaccionEnum.Neutral:
                        //Si la nave atacada pertenece a una facción neutral, solo devuelve el ataque si la relación con esa facción es agresiva
                        if (nave.Faccion.GetRelacion(naveAtacante.Faccion) == Faccion.RelacionConOtraFaccionEnum.Agresiva)
                            return new EstadoNPCAtacar(nave, naveAtacante);
                        else
                            return new EstadoNPCHuir(nave, naveAtacante);

                    case Faccion.TipoFaccionEnum.Pavisa:
                    case Faccion.TipoFaccionEnum.Recolectora:
                        //Finalmente, si la nave atacada pertenece a una facción pasiva o recolectora, comienza a huir!
                        return new EstadoNPCHuir(nave, naveAtacante);
                }
            }

            return this;
        }

        public override EstadoNPC Procesar(float fDeltaSegundos)
        {
            EstadoNPC nuevoEstado = this;

            switch (nave.Faccion.TipoFaccion)
            {
                case Faccion.TipoFaccionEnum.Agresiva:
                    {
                        Thing[] thingsEnRadio = nave.Galaxia.GetThingsEnRadio(nave.Centro, RadioBusquedaAtaque);
                        bool atacando = false;

                        foreach (Thing thing in thingsEnRadio)
                        {
                            if (thing is ThingNave &&
                                thing != nave)
                            {
                                ThingNave thingNave = (ThingNave)thing;

                                if (nave.Faccion.GetRelacion(thingNave.Faccion) == Faccion.RelacionConOtraFaccionEnum.Agresiva)
                                {
                                    nuevoEstado = new EstadoNPCAtacar(nave, thingNave);
                                    atacando = true;
                                    break;
                                }
                            }
                        }

                        if (!atacando)
                        {
                            Vector2 destino = nave.Galaxia.BuscarPosicionLibreAlAzar(nave.SectorNativo.Centro, RadioBusquedaRecursos, nave.Tamanio.X);
                            nuevoEstado = new EstadoNPCNavegarA(nave, destino);
                        }

                        break;
                    }

                case Faccion.TipoFaccionEnum.Neutral:
                case Faccion.TipoFaccionEnum.Pavisa:
                    {
                        Vector2 destino = nave.Galaxia.BuscarPosicionLibreAlAzar(nave.SectorNativo.Centro, RadioBusquedaRecursos, nave.Tamanio.X);
                        nuevoEstado = new EstadoNPCNavegarA(nave, destino);
                        break;
                    }

                case Faccion.TipoFaccionEnum.Recolectora:
                    {
                        Recursos.Recurso[] recursos = nave.Galaxia.BuscarRecursos(nave.SectorNativo.Centro, RadioBusquedaRecursos);
                        Recursos.Recurso recursoARecolectar = null;

                        for (int i = 0; i < recursos.Length; i++)
                            if (recursos[i].CantidadDisponible >= 300.0f)
                            {
                                recursoARecolectar = recursos[i];
                                break;
                            }

                        if (recursoARecolectar != null)
                        {
                            nuevoEstado = new EstadoNPCRecolectar(nave, recursoARecolectar);
                        }
                        else
                        {
                            Vector2 destino = nave.Galaxia.BuscarPosicionLibreAlAzar(nave.SectorNativo.Centro, RadioBusquedaRecursos, nave.Tamanio.X);
                            nuevoEstado = new EstadoNPCNavegarA(nave, destino);
                        }
                        break;
                    }
            }

            return nuevoEstado;
        }
    }
}
