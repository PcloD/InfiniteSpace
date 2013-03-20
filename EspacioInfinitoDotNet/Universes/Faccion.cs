using System;
using System.Collections.Generic;
using System.Text;

namespace EspacioInfinitoDotNet.Universes
{
    public class Faccion
    {
        public enum TipoFaccionEnum
        {
            Agresiva, //Atacan todo lo que se mueve
            Pavisa, //No atacan, y huyen al ser atacados
            Neutral, //Se defienden cuando son atacados
            Recolectora //Buscan recursos para llevar a una estación espacial o planeta
        }

        public enum RelacionConOtraFaccionEnum
        {
            Amigable, //No las atacan, y si una de sus naves es atacadas, la defienden
            Neutral, //No las atacan
            Agresiva //Los atacan si los ven
        }

        private string nombre;
        private byte identificador;
        private TipoFaccionEnum tipoFaccion;
        private Dictionary<Faccion, RelacionConOtraFaccionEnum> relaciones = new Dictionary<Faccion, RelacionConOtraFaccionEnum>();
        private System.Drawing.Color color = System.Drawing.Color.White;

        public string Nombre
        {
            get { return nombre; }
        }

        public byte Identificador
        {
            get { return identificador; }
        }

        public TipoFaccionEnum TipoFaccion
        {
            get { return tipoFaccion; }
        }

        public System.Drawing.Color Color
        {
            get { return color; }
        }

        public Faccion(string nombre, byte identificador, TipoFaccionEnum tipoFaccion, System.Drawing.Color color)
        {
            this.nombre = nombre;
            this.identificador = identificador;
            this.tipoFaccion = tipoFaccion;
            this.color = color;
        }

        public void SetRelacion(Faccion faccion, RelacionConOtraFaccionEnum relacion)
        {
            if (relaciones.ContainsKey(faccion))
                relaciones[faccion] = relacion;
            else
                relaciones.Add(faccion, relacion);
        }

        public void InformarAtaque(Faccion deFaccion)
        {
            
        }

        public void InformarDestruccionDePropiedad(Faccion deFaccion)
        {
            if (!relaciones.ContainsKey(deFaccion))
            {
                relaciones.Add(deFaccion, RelacionConOtraFaccionEnum.Agresiva);
            }
            else
            {
                if (relaciones[deFaccion] != RelacionConOtraFaccionEnum.Amigable)
                    relaciones[deFaccion] = RelacionConOtraFaccionEnum.Agresiva;
            }
        }

        public RelacionConOtraFaccionEnum GetRelacion(Faccion faccion)
        {
            if (faccion == this)
                return RelacionConOtraFaccionEnum.Amigable;

            if (relaciones.ContainsKey(faccion))
                return relaciones[faccion];

            switch (tipoFaccion)
            {
                case TipoFaccionEnum.Neutral:
                case TipoFaccionEnum.Pavisa:
                case TipoFaccionEnum.Recolectora:
                    return RelacionConOtraFaccionEnum.Neutral;

                case TipoFaccionEnum.Agresiva:
                default:
                    return RelacionConOtraFaccionEnum.Agresiva;
            }
        }
    }
}
