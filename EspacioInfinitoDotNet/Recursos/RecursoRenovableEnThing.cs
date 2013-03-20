using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;
using EspacioInfinitoDotNet.Things;

namespace EspacioInfinitoDotNet.Recursos
{
    public class RecursoRenovableEnThing : RecursoRenovable
    {
        Thing thing;

        public RecursoRenovableEnThing(Thing thing, string descripcion, float cantidadDisponible, float maximaDistanciaConsumir, float velocidadRenovacion, float maximoDisponible)
            : base(descripcion, cantidadDisponible, maximaDistanciaConsumir, velocidadRenovacion, maximoDisponible)
        {
            this.thing = thing;
        }

        public override Vector2 Posicion
        {
            get { return thing.Centro; }
        }
    }
}
