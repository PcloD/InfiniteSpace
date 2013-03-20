using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Recursos
{
    public abstract class Recurso
    {
        public const string RecursoHelio = "Helio";
        public const string RecursoPlasma = "Plasma";

        #region Atributos

        string descripcion;
        protected float cantidadDisponible;
        private float maximaDistanciaConsumir;

        public string Descripcion
        {
            get { return descripcion; }
        }

        public float CantidadDisponible
        {
            get { return cantidadDisponible; }
        }

        public float MaximaDistanciaConsumir
        {
            get { return maximaDistanciaConsumir; }
        }

        public abstract Vector2 Posicion
        {
            get;
        }


        #endregion

        public Recurso(string descripcion, float cantidadDisponible, float maximaDistanciaConsumir)
        {
            this.descripcion = descripcion;
            this.cantidadDisponible = cantidadDisponible;
            this.maximaDistanciaConsumir = maximaDistanciaConsumir;
        }

        public float Consumir(float cantidad)
        {
            if (cantidad > cantidadDisponible)
                cantidad = cantidadDisponible;

            cantidadDisponible -= cantidad;

            return cantidad;
        }

    }
}
