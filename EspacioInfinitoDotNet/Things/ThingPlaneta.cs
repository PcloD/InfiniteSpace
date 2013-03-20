using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    class ThingPlaneta : ThingRedondo
    {
        #region Atributos

        static Textura textura;

        #endregion

        public ThingPlaneta(Galaxia galaxia, Vector2 centro, float diametro)
            : base(galaxia, new Vector2(diametro, diametro), centro, 0)
        {
            SetMovible(false);
            SetVida(1000000);
            SetColorEnMapa(Color.DarkSalmon);
        }

        #region Metodos sobrecargados de Thing

        public override void Dibujar()
        {
            if (textura == null)
                textura = TexturaManager.Instance.CargarTextura(Data.NombresTexturas.Planeta);

            GraphicEngine.Instance.DrawRectangle(Centro, RotacionEnGrados, Tamanio, textura);
        }

        public override void Procesar(float fDeltaSegundos)
        {
            //TODO: Actualziar estado interno? recursos?
        }

        public override void OnImpacto(Thing thing)
        {
            //TODO: Dock ?
        }

        #endregion
    }
}
