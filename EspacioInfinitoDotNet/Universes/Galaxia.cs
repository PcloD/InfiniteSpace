using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using EspacioInfinitoDotNet.Maths;
using EspacioInfinitoDotNet.Things;
using EspacioInfinitoDotNet.Graphics;

namespace EspacioInfinitoDotNet.Universes
{
    public class Galaxia
    {
        #region Atributos

        private List<Sector> sectores;
        private Dictionary<SectorID, Sector> dicSectores;
        private Dictionary<Sector, Sector> sectoresConThingsActivos;
        private Vector2 posicionInicialJugador;
        private List<Thing> thingsActivos;
        private long identificadorProcesado = 1;
        private System.Random rnd = new Random((int)DateTime.Now.Ticks);
        private System.Drawing.Size tamanioEnSectores;
        private Generadores.Generador generador;
        
        private List<Faccion> facciones = new List<Faccion>();
        private Faccion faccionJugador;

        public Vector2 Tamanio
        {
            get { return new Vector2(tamanioEnSectores.Width * Sector.TamanioSector, tamanioEnSectores.Height * Sector.TamanioSector); }
        }

        public Vector2 PosicionInicialJugador
        {
            get { return posicionInicialJugador; }
        }

        public System.Drawing.Size TamanioEnSectores
        {
            get { return tamanioEnSectores; }
        }

        public Faccion FaccionJugador
        {
            get { return faccionJugador; }
        }

        #endregion

        #region Creacion

        private Galaxia()
        {
            sectores = new List<Sector>();
            dicSectores = new Dictionary<SectorID, Sector>();
            sectoresConThingsActivos = new Dictionary<Sector, Sector>();
            thingsActivos = new List<Thing>();
        }

        static public Galaxia Crear(Generadores.Generador generador)
        {
            Galaxia gal = new Galaxia();

            gal.generador = generador;

            gal.Inicializar();
           
            return gal;
        }

        private void Inicializar()
        {
            generador.Inicializar(this);

            tamanioEnSectores = generador.GetTamanioEnSectores();

            facciones.AddRange(generador.InicializarFacciones());

            posicionInicialJugador = generador.GetPosicionInicialJugador();

            faccionJugador = generador.GetFaccionJugador();
        }

        #endregion

        #region Manejo de sectores

        public List<Sector> Sectores
        {
            get { return sectores; }
        }

        public int SectoresCantidad
        {
            get { return sectores.Count; }
        }

        public int SectoresConThingsActivosCantidad
        {
            get { return sectoresConThingsActivos.Count; }
        }

        internal void AgregarSector(Sector sector)
        {
            sectores.Add(sector);

            dicSectores.Add(sector.SectorID, sector);

            Sector sec = GetSectorEnPosicion(new Vector2(sector.Centro.X, sector.Centro.Y));

            if (sec != sector)
                throw new Exception("Error en Galaxy");
        }

        public Sector[] GetSectoresVisibles(Vector2 centro, Frustum frustum)
        {
            int minX, minY, maxX, maxY;

            Sector sectorCentro = GetSectorEnPosicion(centro);
            Sector sector;

            //Calculo el rango de sectores visibles en X y en Y
            minX = maxX = sectorCentro.SectorID.X;
            
            do
            {
                minX--;
                sector = GetSector(new SectorID(minX, sectorCentro.SectorID.Y));
            } while(frustum.RectangleInside(sector.Centro, sector.Tamanio));
            minX++;

            do
            {
                maxX++;
                sector = GetSector(new SectorID(maxX, sectorCentro.SectorID.Y));
            } while (frustum.RectangleInside(sector.Centro, sector.Tamanio));
            maxX--;

            minY = maxY = sectorCentro.SectorID.Y;

            do
            {
                minY--;
                sector = GetSector(new SectorID(sectorCentro.SectorID.X, minY));
            } while (frustum.RectangleInside(sector.Centro, sector.Tamanio));
            minY++;

            do
            {
                maxY++;
                sector = GetSector(new SectorID(sectorCentro.SectorID.X, maxY));
                
            } while (frustum.RectangleInside(sector.Centro, sector.Tamanio));
            maxY--;

            //Ya tengo el rango de sectores visibles, ahora los cargo en un vector y lo devuelvo
            Sector[] sectoresVisibles = new Sector[(maxX - minX + 1) * (maxY - minY + 1)];
            int n = 0;

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    sectoresVisibles[n++] = GetSector(new SectorID(x, y));

            return sectoresVisibles;
        }

        public Sector GetSectorEnPosicion(Vector2 position)
        {
            int sectorIDx, sectorIDy;

            Math.Sign(

            //if (position.X >= 0.0f)
                sectorIDx = (int)(position.X + Math.Sign(position.X) * Sector.TamanioSector / 2) / Sector.TamanioSector);
            //else
            //    sectorIDx = (int)((position.X - Sector.SectorSize / 2) / Sector.SectorSize);

            //if (position.Y >= 0.0f)
                sectorIDy = (int)((position.Y + Math.Sign(position.Y) * Sector.TamanioSector / 2) / Sector.TamanioSector);
            //else
            //    sectorIDy = (int)((position.Y - Sector.SectorSize / 2) / Sector.SectorSize);

            SectorID sectorID = new SectorID(sectorIDx, sectorIDy);

            return GetSector(sectorID);
        }

        public Sector GetSector(SectorID sectorID)
        {
            Sector sector;

            if (dicSectores.TryGetValue(sectorID, out sector) == false)
                sector = generador.CrearSector(sectorID);

            return sector;
        }

        public bool SectorCargado(SectorID sectorID)
        {
            return dicSectores.ContainsKey(sectorID);
        }


        #endregion

        #region Manejo de things

        public void AgregarThing(Thing thing)
        {
            Sector sector = GetSectorEnPosicion(thing.Centro);

            if (sector == null)
                throw new Exception("El objeto a agregar esta fuera de la galaxia!!");

            AgregarThing(sector, thing);
        }

        private void AgregarThing(Sector sector, Thing thing)
        {
            sector.AgregarThing(thing);

            if (thing.Activo && sector.ThingsActivos == 1)
                sectoresConThingsActivos.Add(sector, sector);
        }

        public void EliminarThing(Thing thing)
        {
            EliminarThing(GetSectorEnPosicion(thing.Centro), thing);
        }

        private void EliminarThing(Sector sector, Thing thing)
        {
            sector.EliminarThing(thing);

            if (thing.Activo && sector.ThingsActivos == 0)
                sectoresConThingsActivos.Remove(sector);
        }

        public void ActualizarCentroThing(Thing thing, Vector2 newCenter)
        {
            Sector sectorFrom = GetSectorEnPosicion(thing.Centro);
            Sector sectorTo = GetSectorEnPosicion(newCenter);

            if (sectorFrom != sectorTo)
            {
                EliminarThing(sectorFrom, thing);
                thing.SetNewCenter(newCenter);
                AgregarThing(sectorTo, thing);
            }
            else
            {
                thing.SetNewCenter(newCenter);
            }
        }

        public void ThingActivado(Thing thing)
        {
            Sector sector = GetSectorEnPosicion(thing.Centro);
            
            sector.ThingActivado(thing);

            if (sector.ThingsActivos == 1) //Es el primer thing que se activa del sector
                sectoresConThingsActivos.Add(sector, sector);

            thingsActivos.Add(thing);
        }

        public void ThingDesactivado(Thing thing)
        {
            Sector sector = GetSectorEnPosicion(thing.Centro);
            
            sector.ThingDesactivado(thing);

            if (sector.ThingsActivos == 0) //No quedan mas things activos en el sector
                sectoresConThingsActivos.Remove(sector);

            thingsActivos.Remove(thing);
        }

        public bool HayThingEnRadio(Vector2 centro, float radio)
        {
            float radioSqr = radio * radio;
            Sector sectorCentro = GetSectorEnPosicion(centro);

            int radioDeBusquedaEnSectores = 1 + (int)(radio / Sector.TamanioSector);

            for (int x = sectorCentro.SectorID.X - radioDeBusquedaEnSectores; x <= sectorCentro.SectorID.X + radioDeBusquedaEnSectores; x++)
            {
                for (int y = sectorCentro.SectorID.Y - radioDeBusquedaEnSectores; y <= sectorCentro.SectorID.Y + radioDeBusquedaEnSectores; y++)
                {
                    Sector sector = GetSector(new SectorID(x, y));

                    foreach (Thing thing in sector.Things)
                        if ((thing.Centro - centro).LengthSqr <= radioSqr)
                            return true;
                }
            }

            return false;
        }

        public Thing[] GetThingsEnRadio(Vector2 centro, float radio)
        {
            float radioSqr = radio * radio;
            Sector sectorCentro = GetSectorEnPosicion(centro);
            List<Thing> thingsEnRadio = new List<Thing>();

            int radioDeBusquedaEnSectores = 1 + (int)(radio / Sector.TamanioSector);

            for (int x = sectorCentro.SectorID.X - radioDeBusquedaEnSectores; x <= sectorCentro.SectorID.X + radioDeBusquedaEnSectores; x++)
            {
                for (int y = sectorCentro.SectorID.Y - radioDeBusquedaEnSectores; y <= sectorCentro.SectorID.Y + radioDeBusquedaEnSectores; y++)
                {
                    Sector sector = GetSector(new SectorID(x, y));

                    foreach (Thing thing in sector.Things)
                        if ((thing.Centro - centro).LengthSqr <= radioSqr)
                            thingsEnRadio.Add(thing);
                }
            }

            return thingsEnRadio.ToArray();
        }

        #endregion

        #region Manejo de colisiones entre Things

        public Thing ColisionaConThing(Thing thingToEvaluate, Vector2 center, Vector2 size, float rotationInDegress)
        {
            Sector centerSector = GetSectorEnPosicion(center);

            Thing thingCollided = centerSector.CollidesWithThing(thingToEvaluate, center, size, rotationInDegress);

            if (thingCollided == null)
            {
                //Ningun thing deberia tener un tamaño mayor al de un sector, debido a lo cual con chequear la colisión contra los things
                //que esten en los sectores que rodean al sector destino alcanza para asegurarme de que no hay colision con ningun thing

                SectorID centerSectorID = centerSector.SectorID;

                for (int x = centerSectorID.X - 1; x <= centerSectorID.X + 1; x++)
                {
                    for (int y = centerSectorID.Y - 1; y <= centerSectorID.Y + 1; y++)
                    {
                        if (x != centerSectorID.X || y != centerSectorID.Y)
                        {
                            Sector sec = GetSector(new SectorID(x, y));

                            thingCollided = sec.CollidesWithThing(thingToEvaluate, center, size, rotationInDegress);

                            if (thingCollided != null)
                                return thingCollided;
                            
                        }
                    }
                }
            }

            return thingCollided;    
        }

        #endregion

        #region Procesar

        public void Procesar(float elapsedSeconds, Vector2 centroDesdeElCualProcesar, int sectoresAledaniosAProcesar)
        {
            //Mantengo los things activos en un vector aparte para asegurarme de procesarlos siempre en el mismo orden, ya que si se procesan en orden
            //cambiante, puede ocurrir que objetos que normalmente no colisionarian colisionen, por ejemplo:
            //- El objeto A esta a una unidad del objeto B
            //- Proceso el objeto B, avanza una unidad
            //- Proceso el objeto A, avanza una unidad
            //Ahora, si proceso el objeto B primero, va a colisionar con el objeto A, ya que el objeto A no se va
            //a haber desplazado, por lo tanto produciendo una falsa colisión.

            //Para asegurarme de no procesar 2 veces un mismo objeto, guardo en cada objeto un identificador de "ID de Proceso",
            //que no es ni mas ni menos que un long (64 bits) que incremento cada vez que se completa un ciclo de proceso, si bien
            //no es imposible que se produzca un wrap-around del contador de 64 bits, la probabilidad de que justo coincida con uno usado antes
            //en un objeto justo al momento de procesar, y que por este motivo se pierda de procesar un ciclo, se podria decir que es infima.
            //Igualmente, suponiendo que se produzcan 1000 procesos por segundo, eso nos deja con un total de 18446744073709551 segundos (2^64 / 1000) antes de que se empiecen
            //a repetir, y son 584942417 años.. asi que se podria decir que no deberia pasar.

            //Proceso los things activos
            foreach(Thing thing in thingsActivos.ToArray()) //Hago un .ToArray() para trabajar sobre una copia del vector
                if (!thing.Eliminado &&
                    thing.identificadorProcesado != identificadorProcesado)
                {
                    thing.identificadorProcesado = identificadorProcesado;
                    thing.Procesar(elapsedSeconds);
                }

            //Ahora proceso los things en los sectores cercanos
            SectorID sectorIDCentro = GetSectorEnPosicion(centroDesdeElCualProcesar).SectorID;

            int fromSectorX = sectorIDCentro.X - sectoresAledaniosAProcesar;
            int toSectorX = sectorIDCentro.X + sectoresAledaniosAProcesar;
            int fromSectorY = sectorIDCentro.Y - sectoresAledaniosAProcesar;
            int toSectorY = sectorIDCentro.Y + sectoresAledaniosAProcesar;

            for (int x = fromSectorX; x <= toSectorX; x++)
                for (int y = fromSectorY; y <= toSectorY; y++)
                    GetSector(new SectorID(x, y)).Procesar(elapsedSeconds, identificadorProcesado);

            //Finalmente proceso los things que se encuentren en sectores con things activos
            foreach (Sector sec in new List<Sector>(sectoresConThingsActivos.Keys))
            {
                if (sec.SectorID.X < fromSectorX ||
                    sec.SectorID.X > toSectorX ||
                    sec.SectorID.Y < fromSectorY ||
                    sec.SectorID.Y < toSectorX)
                {
                    sec.Procesar(elapsedSeconds, identificadorProcesado);
                }
            }

            identificadorProcesado++;
        }

        #endregion

        #region Funciones Auxiliares

        #region Clase usada para ordenar los recursos por distancia

        private class OrdenarRecursosPorDistancia : IComparer<Recursos.Recurso>
        {
            private Vector2 centro;

            public OrdenarRecursosPorDistancia(Vector2 centro)
            {
                this.centro = centro;
            }

            public int Compare(EspacioInfinitoDotNet.Recursos.Recurso x, EspacioInfinitoDotNet.Recursos.Recurso y)
            {
                return (centro - x.Posicion).LengthSqr.CompareTo((centro - y.Posicion).LengthSqr);
            }
        }

        #endregion

        public Recursos.Recurso[] BuscarRecursos(Vector2 centro, float radioDeBusqueda)
        {
            List<Recursos.Recurso> recursos = new List<EspacioInfinitoDotNet.Recursos.Recurso>();
            
            Sector sectorCentro = GetSectorEnPosicion(centro);

            int radioDeBusquedaEnSectores = 1 + (int) (radioDeBusqueda / Sector.TamanioSector);

            for (int x = sectorCentro.SectorID.X - radioDeBusquedaEnSectores; x <= sectorCentro.SectorID.X + radioDeBusquedaEnSectores; x++)
            {
                for (int y = sectorCentro.SectorID.Y - radioDeBusquedaEnSectores; y <= sectorCentro.SectorID.Y + radioDeBusquedaEnSectores; y++)
                {
                    Sector sector = GetSector(new SectorID(x, y));

                    recursos.AddRange(sector.BuscarRecursos(centro, radioDeBusqueda));
                }
            }

            if (recursos.Count > 0)
                recursos.Sort(new OrdenarRecursosPorDistancia(centro));

            return recursos.ToArray();
        }

        public Vector2 BuscarPosicionPlanetaFaccion(Vector2 centro, float radioDeBusqueda, Faccion faccion)
        {
            Vector2 posicion = new Vector2(0, 0);

            Sector sectorCentro = GetSectorEnPosicion(centro);

            int radioDeBusquedaEnSectores = 1 + (int)(radioDeBusqueda / Sector.TamanioSector);

            for (int x = sectorCentro.SectorID.X - radioDeBusquedaEnSectores; x <= sectorCentro.SectorID.X + radioDeBusquedaEnSectores; x++)
            {
                for (int y = sectorCentro.SectorID.Y - radioDeBusquedaEnSectores; y <= sectorCentro.SectorID.Y + radioDeBusquedaEnSectores; y++)
                {
                    Sector sector = GetSector(new SectorID(x, y));

                    if (sector.Faccion == faccion)
                    {
                        foreach (Thing thing in sector.Things)
                            if (thing is ThingPlaneta)
                            {
                                if (posicion.X == 0 && posicion.Y == 0 ||
                                    (posicion - centro).Length > (thing.Centro - centro).Length)
                                {
                                    posicion = thing.Centro;
                                }
                            }
                    }
                }
            }

            return posicion;
        }

        public Vector2 BuscarPosicionLibreAlAzar(Vector2 centro, float radioDeBusqueda, float radioEspacioLibre)
        {
            float x = rnd.Next((int)(centro.X - radioDeBusqueda), (int)(centro.X + radioDeBusqueda));
            float y = rnd.Next((int)(centro.Y - radioDeBusqueda), (int)(centro.Y + radioDeBusqueda));

            return GetSectorEnPosicion(new Vector2(x, y)).BuscarEspacioVacio(radioEspacioLibre);
        }

        #endregion
    }
}
