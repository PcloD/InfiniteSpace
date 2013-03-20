using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Things
{
    public abstract class ThingRedondo : ThingDaniable
    {
        protected BoundingObject[] boundingObjects;

        private Color colorEnMapa = Color.FromArgb(200, Color.White);

        public Color ColorEnMapa
        {
            get { return colorEnMapa; }
        }

        protected void SetColorEnMapa(Color colorEnMapa)
        {
            this.colorEnMapa = colorEnMapa;
        }

        protected ThingRedondo(Galaxia galaxia, Vector2 size, Vector2 position, float rotation)
            : base(galaxia, size, position, rotation)
        {
        }

        public override BoundingObject[] CrearBoundingObjects()
        {
            if (boundingObjects == null)
            {
                boundingObjects = new BoundingObject[1];

                boundingObjects[0] = new BoundingCircle(new Vector2(0, 0), Tamanio.X / 2.0f);
            }

            return boundingObjects;
        }

        public override void DibujarEnMapa(Vector2 centro, float tamanio, EspacioInfinitoDotNet.GUI.GUIGraphicEngine guiGraphicEngine)
        {
            guiGraphicEngine.DrawRectangle(
                new Rectangle(
                    (int)(centro.X - tamanio / 3.0f),
                    (int)(centro.Y - tamanio / 3.0f),
                    (int)(tamanio / 1.5f),
                    (int)(tamanio / 1.5f)),
                    colorEnMapa);
        }
    }
}
