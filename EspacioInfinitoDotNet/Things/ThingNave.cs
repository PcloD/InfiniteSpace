using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Maths;
using EspacioInfinitoDotNet.GUI;
using Tao.OpenGl;

namespace EspacioInfinitoDotNet.Things
{
    public class ThingNave : ThingDaniable
    {
        #region Atributos

        Textura Textura;
        static BoundingObject[] boundingObjects;
        string nombre = "Nave";
        protected float velocidad = 1500.0f;
        Faccion faccion;
        Color colorEnMapa = Color.FromArgb(200, Color.White);

        public float Velocidad
        {
            get { return velocidad; }
        }

        public string Nombre
        {
            get { return this.nombre; }
            set { this.nombre = value; }
        }

        public Faccion Faccion
        {
            get { return faccion; }
        }

        public Color ColorEnMapa
        {
            get { return colorEnMapa; }
        }

        #endregion

       
        public ThingNave(Galaxia galaxia, Vector2 size, Vector2 center, float rotation, Faccion faccion)
            : base(galaxia, size, center, rotation)
        {
            this.faccion = faccion;
        }

        protected void SetColorEnMapa(Color colorEnMapa)
        {
            this.colorEnMapa = colorEnMapa;
        }
        
        #region Metodos sobrecargados de Thing

        public override BoundingObject[] CrearBoundingObjects()
        {
            //Aproximo la nave con 2 rectangulos

            if (boundingObjects == null)
            {
                //Puntos
                // 0  1
                // *--*|
                // |   *--*2
                // |   *--*3
                // *--*|
                // 5  4 

                //Este es un modelo simplificado que se usa para las pruebas de colision
                //boundingObjects = new BoundingObject[] { new BoundingRectangle(new Vector2(0, 0), Tamanio, 0) };

                boundingObjects = new BoundingObject[2];

                boundingObjects[0] = new BoundingRectangle(new Vector2(-Tamanio.X / 4.0f, 0), new Vector2(Tamanio.X / 2, Tamanio.Y), 0);
                boundingObjects[1] = new BoundingRectangle(new Vector2(Tamanio.X / 4.0f, 0), new Vector2(Tamanio.X / 2, Tamanio.Y / 2), 0);
            }

            return boundingObjects;
        }

        public override void Dibujar()
        {
            if (Textura == null)
                Textura = TexturaManager.Instance.CargarTextura(Data.NombresTexturas.Nave);

            //Nave
            if (faccion == null)
                GraphicEngine.Instance.DrawRectangle(Centro, RotacionEnGrados, Tamanio, Textura);
            else
                GraphicEngine.Instance.DrawRectangle(Centro, RotacionEnGrados, Tamanio, Textura, Faccion.Color);

            //Indicador de energia
            float porcentajeEnergia = 100.0f * Vida / MaxVida;
            float fromX = -Tamanio.X / 2.0f;
            float toX = -Tamanio.X / 2.0f + Tamanio.X * porcentajeEnergia / 100.0f;

            Color colorEnergia;
            if (porcentajeEnergia > 50)
                colorEnergia = Color.Green;
            else if (porcentajeEnergia > 30)
                colorEnergia = Color.Yellow;
            else
                colorEnergia = Color.Red;

            GraphicEngine.Instance.DrawRectangle(new Vector2(Centro.X - Tamanio.X / 2.0f + (toX - fromX) / 2.0f , Centro.Y - Tamanio.Y / 2.0f - 20.0f), 0, new Vector2(toX - fromX, 10), colorEnergia);

            //Nombre de la nave
            Graphics.GraphicEngine.Instance.DrawTextStrokeCenteredX(Centro.X, Centro.Y + Tamanio.Y / 2.0f + 75.0f, Color.White, 50.0f, Nombre);
        }

        public override void Procesar(float fDeltaSegundos)
        {
            if (reloadTime > 0.0f)
                reloadTime -= fDeltaSegundos;
        }

        public override void OnImpacto(Thing thing)
        {
            if (thing.Eliminado)
                return;

            Vector2 vec = (this.Centro - thing.Centro).Normalized();

            MoverA(vec, true, false);
        }

        public override void DibujarEnMapa(Vector2 centro, float tamanio, GUIGraphicEngine guiGraphicEngine)
        {
            GraphicEngine.Instance.SetColor(colorEnMapa);

            //Tengo que sumar el origen de coordenadas del GUIGraphicEngine porque no estoy dibujando
            //con las funciones de GUI
            centro.X += guiGraphicEngine.Bounds.X;
            centro.Y += guiGraphicEngine.Bounds.Y;

            Gl.glBegin(Gl.GL_TRIANGLES);

            Vector2[] puntosNave = new Vector2[3];

            puntosNave[0] = new Vector2(tamanio, 0);
            puntosNave[1] = new Vector2(-tamanio, tamanio * 2 / 3);
            puntosNave[2] = new Vector2(-tamanio, -tamanio * 2 / 3);

            for (int i = 0; i < 3; i++)
            {
                puntosNave[i] = puntosNave[i].RotateByDegress(-RotacionEnGrados);
                Gl.glVertex3f(centro.X + puntosNave[i].X, centro.Y + puntosNave[i].Y, GraphicEngine.zValue);
            }

            Gl.glEnd();
        }

        #endregion

        float reloadTime = -1.0f;

        public void Shoot()
        {
            if (reloadTime <= 0.0f)
            {
                Vector2 laserPosition = Centro;

                float distance = Tamanio.X / 2.0f + 35.0f; /* 25.0f es la mitad del largo del laser */

                laserPosition.X += (float)Math.Cos(RotacionEnRadianes) * distance;
                laserPosition.Y += (float)Math.Sin(RotacionEnRadianes) * distance;

                ThingLaser laser = new ThingLaser(Galaxia, laserPosition, RotacionEnGrados, this);

                reloadTime = 0.1f;
            }
        }
    }
}
