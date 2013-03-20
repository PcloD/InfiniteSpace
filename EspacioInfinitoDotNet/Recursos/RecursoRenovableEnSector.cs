using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Maths;
using EspacioInfinitoDotNet.Universes;

namespace EspacioInfinitoDotNet.Recursos
{
    public class RecursoRenovableEnSector : RecursoRenovable
    {
        Sector sector;

        public RecursoRenovableEnSector(Sector sector, string descripcion, float cantidadDisponible, float maximaDistanciaConsumir, float velocidadRenovacion, float maximoDisponible)
            : base(descripcion, cantidadDisponible, maximaDistanciaConsumir, velocidadRenovacion, maximoDisponible)
        {
            this.sector = sector;
        }

        public override Vector2 Posicion
        {
            get { return sector.Centro; }
        }
    }
}
