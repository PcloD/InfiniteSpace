using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;
using EspacioInfinitoDotNet.Recursos;

namespace EspacioInfinitoDotNet.Things.NPC.Estados
{
    public class EstadoNPCRecolectar : EstadoNPC
    {
        private Recurso recurso;
        private Vector2 destino;
        private float tiempoIntentosFallidos = 0;
        private float totalARecolectar = 600.0f;
        private float velocidadRecoleccion = 200.0f;
        private float recolectado = 0;
        bool recolectando = false;
        bool recoleccionRealizada = false;

        public EstadoNPCRecolectar(ThingNaveNPC nave, Recurso recurso)
            : base(nave)
        {
            this.recurso = recurso;

            //Randomizo el destino para que quede dentro del radio de consumisión disponible, pero no siempre
            //en el mismo lugar
            Vector2 rand = new Vector2(0, 0);
            
            while (rand.X == 0 || rand.Y == 0)
                rand = new Vector2(rnd.Next(-900, 900) / 1000.0f, rnd.Next(-900, 900) / 1000.0f);

            this.destino = recurso.Posicion + rand * recurso.MaximaDistanciaConsumir;
        }

        public override string Describir()
        {
            if (recolectando)
                return "Recolectando";
            else if (recoleccionRealizada)
                return "Retornando Recursos";
            else
                return "Buscando Recursos";
        }

        public override EstadoNPC OnDaniadoPor(Thing thing)
        {
            return new EstadoNPCPlanificar(nave).OnDaniadoPor(thing);
        }

        public override EstadoNPC Procesar(float fDeltaSegundos)
        {
            if (!recolectando)
            {
                //Esta volviendo o yendo a recolectar
                Vector2 diferencia = destino - nave.Centro;

                if (diferencia.Length > 50.0f)
                {
                    //Todavia no llegue a destino
                    if (tiempoIntentosFallidos < 0.5f)
                    {
                        if (!EstadoNPCNavegarA.IrA(nave, destino, fDeltaSegundos))
                            tiempoIntentosFallidos += fDeltaSegundos;
                        return this;
                    }
                    else
                    {
                        //Fallé muchas veces en llegar, planifico de nuevo
                        return new EstadoNPCPlanificar(nave);
                    }
                }
                else
                {
                    //Llegué a destino

                    if (recoleccionRealizada) //Ya habia hecho la recolección, lo que quiere decir que ya llegué de vuelta al planeta!
                        return new EstadoNPCPlanificar(nave);
                    else
                    {
                        recolectando = true; //Todavia no se hizo la recolección, la inicio
                        return this;
                    }
                }
            }
            else
            {
                Vector2 diferencia = destino - nave.Centro;

                if (diferencia.Length <= 50.0f)
                {
                    //Esta recolectando

                    float consumido = recurso.Consumir(velocidadRecoleccion * fDeltaSegundos);

                    recolectado += consumido;

                    if (recolectado >= totalARecolectar || consumido == 0)
                    {
                        //Termino la recolección, ya sea porque consumí todos los recursos disponibles o porque
                        //recolecté todo lo que habia

                        if (recolectado == 0)
                        {
                            //No se recolectó nada, tengo que planificar una nueva recolección
                            return new EstadoNPCPlanificar(nave);
                        }
                        else
                        {
                            //Se recolectó algo, tengo que volver al planeta para dejar los recursos

                            recoleccionRealizada = true;
                            recolectando = false;

                            destino = nave.Galaxia.BuscarPosicionPlanetaFaccion(nave.SectorNativo.Centro, RadioBusquedaRecursos, nave.Faccion);

                            if (destino.X == 0 && destino.Y == 0)
                                return new EstadoNPCPlanificar(nave); //No se encontro planeta, replanifico
                            else
                                return this;
                        }
                    }
                    else
                        return this;
                }
                else
                {
                    //Algo me desplazo, tengo que volver a ubicarme

                    recolectando = false;
                    return this;
                }
            }
        }
    }
}
