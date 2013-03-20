using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;
using EspacioInfinitoDotNet.Recursos;

namespace EspacioInfinitoDotNet.Things
{
    class ThingEstrella : ThingRedondo, ITieneRecursos
    {
        #region Atributos

        static Textura Textura;
        float velocidadAtraccion = 10.0f;
        float radioAccion = 1000.0f;
        private RecursoRenovable[] recursos;

        public float VelocidadAtraccion
        {
            get { return velocidadAtraccion; }
            set { this.velocidadAtraccion = value; }
        }

        public float RadioAccion
        {
            get { return radioAccion; }
            set { this.radioAccion = value; }
        }

        #endregion

        public ThingEstrella(Galaxia galaxia, Vector2 centro, float diametro)
            : base(galaxia, new Vector2(diametro, diametro), centro, 0)
        {
            SetMovible(false);
            SetVida(100);
            SetColorEnMapa(Color.Red);
            
            recursos = new RecursoRenovable[1];
            recursos[0] = new RecursoRenovableEnThing(this, Recurso.RecursoHelio, 1000, diametro * 5.0f, 50, 1000);
        }

        #region Metodos sobrecargados de Thing

        public override void Dibujar()
        {
            if (Textura == null)
                Textura = TexturaManager.Instance.CargarTextura(Data.NombresTexturas.Estrella);

            GraphicEngine.Instance.DrawRectangle(Centro, RotacionEnGrados, Tamanio, Textura);
        }

        protected override void Explotar()
        {
            ThingOndaExpansiva oe = new ThingOndaExpansiva(Galaxia, Centro);

            oe.Danio = 3000000;
            oe.DuracionSegundos = 1.5f;
            oe.RadioMaximo = Tamanio.X / 2 + 1500.0f;
            oe.RadioInicial = Tamanio.X / 2;
        }

        public override void Procesar(float fDeltaSegundos)
        {
            Thing[] thingsAfectados = Galaxia.GetThingsEnRadio(Centro, radioAccion);

            foreach (Thing thing in thingsAfectados)
                if (thing.Movible)
                {
                    Vector2 distancia = thing.Centro - this.Centro;

                    float velocidad = -velocidadAtraccion * (radioAccion - distancia.Length) / radioAccion;

                    thing.MoverA(distancia.Normalized() * velocidad * fDeltaSegundos, true, true);
                }
        }

        public override void OnImpacto(Thing thing)
        {
            if (thing.Eliminado)
                return;

            if (thing is ThingDaniable)
                ((ThingDaniable) thing).ProcesarDanio(this, 1000.0f);
        }

        #endregion

        #region Metodos de ITieneRecursos

        public Recurso[] GetRecursos()
        {
            return recursos;
        }

        public void RenovarRecursos(float fDeltaTime)
        {
            recursos[0].Renovar(fDeltaTime);
        }

        #endregion
    }
}
