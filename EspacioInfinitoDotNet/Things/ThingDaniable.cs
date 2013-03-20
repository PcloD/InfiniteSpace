using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    public abstract class ThingDaniable : Thing
    {
        #region Atributos

        private float maxVida = 100.0f;
        private float vida = 100.0f;

        public float Vida
        {
            get { return vida; }
        }

        public float MaxVida
        {
            get { return maxVida; }
        }

        protected void SetVida(float vida)
        {
            this.vida = vida;
        }

        protected void SetMaxVida(float maxVida)
        {
            this.maxVida = maxVida;
        }

        #endregion

        protected ThingDaniable(Galaxia galaxia, Vector2 size, Vector2 position, float rotation)
            : base (galaxia, size, position, rotation)
        {
        }

        public virtual void ProcesarDanio(Thing delThing, float danio)
        {
            vida -= danio;

            if (vida <= 0.001f)
                OnDestruido();
        }

        protected virtual void OnDestruido()
        {
            Explotar();
            Eliminar();
        }

        protected virtual void Explotar()
        {
            int fragmentos = (int) (Tamanio.Length / 5.0f);

            for (int i = 0; i < fragmentos; i++)
                new ThingResto(Galaxia, Centro);
        }
    }
}
