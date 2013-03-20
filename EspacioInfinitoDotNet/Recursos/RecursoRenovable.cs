using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Recursos
{
    public abstract class RecursoRenovable : Recurso
    {
        private float velocidadRenovacion;
        private float maximoDisponible;

        public float VelocidadRenovacion
        {
            get { return velocidadRenovacion; }
        }

        public float MaximoDisponible
        {
            get { return maximoDisponible; }
        }

        public RecursoRenovable(string descripcion, float cantidadDisponible, float maximaDistanciaConsumir, float velocidadRenovacion, float maximoDisponible)
            : base(descripcion, cantidadDisponible, maximaDistanciaConsumir)
        {
            this.velocidadRenovacion = velocidadRenovacion;
            this.maximoDisponible = maximoDisponible;
        }

        public void Renovar(float fDeltaTimeSeconds)
        {
            if (cantidadDisponible + velocidadRenovacion * fDeltaTimeSeconds > maximoDisponible)
                cantidadDisponible = maximoDisponible;
            else
                cantidadDisponible += velocidadRenovacion * fDeltaTimeSeconds;
        }
    }
}
