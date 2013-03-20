using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    public abstract class Thing
    {
        #region Atributos

        private Vector2 tamanio;
        internal Vector2 centro;
        private float rotacion = 0;
        private Universes.Galaxia galaxia;
        internal long identificadorProcesado;
        private BoundingCircle boundingCircle;
        
        private bool activo = false;
        private bool solido = true;
        private bool movible = true;
        private bool eliminado = false;

        public float RotacionEnGrados
        {
            get { return rotacion; }
        }

        public float RotacionEnRadianes
        {
            get { return (float) (System.Math.PI * rotacion / 180.0); }
        }

        public Vector2 RotacionVector
        {
            get { return new Vector2((float)Math.Cos(RotacionEnRadianes), (float)Math.Sin(RotacionEnRadianes)); }
        }

        public Vector2 Centro 
        {
            get { return centro; }
        }

        public Vector2 Tamanio
        {
            get { return tamanio; }
        }

        public Universes.Galaxia Galaxia
        {
            get { return galaxia; }
        }

        public BoundingCircle BoundingCircle
        {
            get { return boundingCircle; }
        }

        public bool Solido
        {
            get { return solido; }
        }

        public bool Movible
        {
            get { return movible; }
        }

        public bool Activo
        {
            get { return activo; }
        }

        public bool Eliminado
        {
            get { return eliminado; }
        }

        #endregion

        protected Thing(Galaxia galaxia, Vector2 size, Vector2 position, float rotation)
        {
            this.centro = position;
            this.tamanio = size;
            this.rotacion = rotation;
            this.galaxia = galaxia;

            boundingCircle = new BoundingCircle(centro, (float)Math.Sqrt(((tamanio.X / 2.0f) * (tamanio.X / 2.0f) + (tamanio.Y / 2.0f) * (tamanio.Y / 2.0f))));

            galaxia.AgregarThing(this);
        }

        #region Metodos accedidos por Galaxia (no deben ser usados desde otro lado!!

        internal void SetNewCenter(Vector2 newCenter)
        {
            centro = newCenter;
            boundingCircle.Center = newCenter;
        }

        #endregion

        #region Metodos a sobrecargar

        public virtual void Dibujar()
        {
        }

        public virtual BoundingObject[] CrearBoundingObjects()
        {
            return null;
        }

        public virtual void Procesar(float fDeltaSegundos)
        {
        }

        public virtual void OnImpacto(Thing thing)
        {
        }

        public virtual void OnEliminado()
        {
        }

        public virtual void DibujarEnMapa(Vector2 centro, float tamanio, GUI.GUIGraphicEngine guiGraphicEngine)
        {
        }

        #endregion

        #region Metodos accedidos solo por las subclases

        protected void SetSolido(bool solido)
        {
            this.solido = solido;
        }

        protected void SetMovible(bool movible)
        {
            this.movible = movible;
        }

        protected void SetActivo(bool activo)
        {
            if (this.activo != activo)
            {
                this.activo = activo;

                if (activo)
                    galaxia.ThingActivado(this);
                else
                    galaxia.ThingDesactivado(this);
            }
        }

        protected void SetTamanio(Vector2 tamanio)
        {
            this.tamanio = tamanio;
            boundingCircle.Radius = (float)Math.Sqrt(((tamanio.X / 2.0f) * (tamanio.X / 2.0f) + (tamanio.Y / 2.0f) * (tamanio.Y / 2.0f)));
        }

        protected void Eliminar()
        {
            if (eliminado == true)
                throw new Exception("El objeto ya habia sido eliminado!!");

            if (Activo)
                SetActivo(false);

            eliminado = true;
            galaxia.EliminarThing(this);
        }

        #endregion

        #region Mover

        public bool MoverAdelante(float distancia, bool evaluarColisiones, bool generarImpacto)
        {
            Vector2 direction = RotacionVector * distancia;

            return MoverA(direction, evaluarColisiones, generarImpacto);
        }

        public bool MoverA(Vector2 direccion, bool evaluarColisiones, bool generarImpacto)
        {
            if (eliminado)
                return false;

            if (evaluarColisiones && Solido)
            {
                float len = direccion.Length;
                direccion = direccion.Normalized();

                while(len > 0.0f)
                {
                    float d;

                    if (len > 10.0f)
                        d = 10.0f;
                    else
                        d = len;

                    Vector2 newPosition = centro + direccion * d;

                    if (galaxia.GetSectorEnPosicion(newPosition) != null)
                    {
                        Thing thingCollided = galaxia.ColisionaConThing(this, newPosition, Tamanio, RotacionEnGrados);

                        if (thingCollided == null)
                        {
                            galaxia.ActualizarCentroThing(this, newPosition);
                        }
                        else
                        {
                            if (generarImpacto)
                            {
                                OnImpacto(thingCollided);
                                thingCollided.OnImpacto(this);
                            }

                            return false;
                        }

                        len -= d;
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                Vector2 newPosition = centro + direccion;

                if (galaxia.GetSectorEnPosicion(newPosition) != null)
                {
                    galaxia.ActualizarCentroThing(this, newPosition);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Rotar

        public bool Rotar(float anguloEnGrados, bool evaluarColisiones, bool generarImpacto)
        {
            if (eliminado)
                return false;

            if (evaluarColisiones && Solido)
            {
                float grados = Math.Abs(anguloEnGrados);
                float direccion;

                if (anguloEnGrados > 0.0f)
                    direccion = 1.0f;
                else
                    direccion = -1.0f;

                while (grados > 0.0f)
                {
                    float d;

                    if (grados > 10.0f)
                        d = 10.0f;
                    else
                        d = grados;

                    float nuevaRotacion = RotacionEnGrados + d * direccion;

                    if (nuevaRotacion < 0.0f)
                        nuevaRotacion += 360.0f;
                    else if (nuevaRotacion > 360.0f)
                        nuevaRotacion -= 360.0f;

                    Thing thingCollided = galaxia.ColisionaConThing(this, Centro, Tamanio, nuevaRotacion);

                    if (thingCollided == null)
                    {
                        rotacion = nuevaRotacion;
                    }
                    else
                    {
                        if (generarImpacto)
                        {
                            OnImpacto(thingCollided);
                            thingCollided.OnImpacto(this);
                        }

                        return false;
                    }

                    grados -= d;
                }

                return true;
            }
            else
            {
                float nuevaRotacion = RotacionEnGrados + anguloEnGrados;

                if (nuevaRotacion < 0.0f)
                    nuevaRotacion += 360.0f;
                else if (nuevaRotacion > 360.0f)
                    nuevaRotacion -= 360.0f;

                rotacion = nuevaRotacion;
            }

            return false;
        }

        #endregion
    }
}
