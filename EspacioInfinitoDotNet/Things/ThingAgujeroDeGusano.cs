using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    class ThingAgujeroDeGusano : ThingRedondo
    {
        #region Atributos

        static Textura Textura;
        float velocidadRotacion = 180.0f;
        private ThingAgujeroDeGusano agujeroDestino;

        public float VelocidadRotacion
        {
            get { return velocidadRotacion; }
            set { this.velocidadRotacion = value; }
        }

        public ThingAgujeroDeGusano AgujeroDestino
        {
            get { return agujeroDestino; }
            set { agujeroDestino = value; }
        }

        #endregion

        public ThingAgujeroDeGusano(Galaxia galaxia, Vector2 centro, float diametro)
            : base(galaxia, new Vector2(diametro, diametro), centro, 0)
        {
            SetMovible(false);
            SetMaxVida(1000000);
            SetVida(1000000);
            SetColorEnMapa(Color.Blue);
        }

        protected override void OnDestruido()
        {
            if (!Eliminado)
            {
                base.OnDestruido();

                AgujeroDestino.OnOtroExtremoDestruido();
            }
        }

        private void OnOtroExtremoDestruido()
        {
            OnDestruido();
        }

        #region Metodos sobrecargados de Thing

        public override void Dibujar()
        {
            if (Textura == null)
                Textura = TexturaManager.Instance.CargarTextura(Data.NombresTexturas.AgujeroDeGusano);

            GraphicEngine.Instance.DrawRectangle(Centro, RotacionEnGrados, Tamanio * 2.0f, Textura);
        }

        public override void Procesar(float fDeltaSegundos)
        {
            Rotar(velocidadRotacion * fDeltaSegundos, false, false);
        }

        public override void OnImpacto(Thing thing)
        {
            if (thing.Eliminado)
                return;

            if (agujeroDestino != null)
            {
                Vector2 vectorEntrada = (thing.Centro - this.Centro).Normalized();
                Vector2 posicionFinal = agujeroDestino.Centro - vectorEntrada * (agujeroDestino.Tamanio.X / 2.0f * 1.1f + thing.Tamanio.Length / 2.0f);

                if (Galaxia.ColisionaConThing(thing, posicionFinal, thing.Tamanio * 1.5f, thing.RotacionEnGrados) == null)
                    Galaxia.ActualizarCentroThing(thing, posicionFinal);
            }
        }

        #endregion
    }
}
