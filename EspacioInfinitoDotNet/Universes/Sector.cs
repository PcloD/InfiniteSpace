using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using EspacioInfinitoDotNet.Things;
using System.Drawing.Drawing2D;
using System.Collections.ObjectModel;
using EspacioInfinitoDotNet.Maths;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Recursos;
using Tao.OpenGl;

namespace EspacioInfinitoDotNet.Universes
{
    public class Sector : ITieneRecursos
    {
        public const int TamanioSector = 2048;
        public const int CantidadMinimaEstrellas = 3;
        public const int CantidadMaximaEstrellas = 20;
        public const int CantidadMinimaNebulas = 5;
        public const int CantidadMaximaNebulas = 25;

        #region Atributos

        private class Estrella
        {
            public Vector2 posicion;
            public float radius;
            public Color color;
        }

        private class Nebula
        {
            public Vector2 posicion;
            public float radius;
            public Color color;
            public float rotacion;
        }
        
        private Galaxia galaxia;
        private List<Thing> things = new List<Thing>();
        private SectorID sectorID;
        private Vector2 tamanio;
        private Vector2 centro;
        private int thingsActivos = 0;
        private Faccion faccion;

        private Estrella[] estrellas;
        private Nebula[] nebulas;
        private RecursoRenovable[] recursos;

        static private Textura texturaNebula;
        static private Random rnd = new Random((int) DateTime.Now.Ticks);

        public Galaxia Galaxy
        {
            get { return galaxia; }
        }

        public SectorID SectorID
        {
            get { return sectorID; }
        }

        public List<Thing> Things
        {
            get { return things; }
        }

        public int ThingsCount
        {
            get { return things.Count; }
        }

        public Vector2 Tamanio
        {
            get { return tamanio; }
        }

        public Vector2 Centro
        {
            get { return centro; }
        }

        public BoundingRectangle Bounds
        {
            get { return new BoundingRectangle(Centro, Tamanio, 0); }
        }

        public int ThingsActivos
        {
            get { return thingsActivos; }
        }

        public bool TieneNebulosa
        {
            get { return nebulas != null; }
        }

        public Faccion Faccion
        {
            get { return faccion; }
        }

        #endregion

        #region Creacion

        public Sector(Galaxia galaxia, SectorID sectorID, Faccion faccion)
        {
            this.galaxia = galaxia;
            this.sectorID = sectorID;
            this.faccion = faccion;

            tamanio = new Vector2(TamanioSector, TamanioSector);
            centro = new Vector2(SectorID.X * TamanioSector, SectorID.Y * TamanioSector);

            CrearEstrellas();
        }

        public void CrearNebulosa()
        {
            float radioGalaxia = galaxia.Tamanio.Length;

            float densidadNebulas = (radioGalaxia - Centro.Length) / radioGalaxia;
            densidadNebulas = densidadNebulas * densidadNebulas;

            int cantNebulas = CantidadMinimaNebulas + (int)((CantidadMaximaNebulas - CantidadMinimaNebulas) * densidadNebulas);

            nebulas = new Nebula[cantNebulas];

            for (int i = 0; i < cantNebulas; i++)
            {
                Nebula e = new Nebula();

                e.posicion.X = rnd.Next((int)(centro.X - tamanio.X / 2.0f), (int)(centro.X + tamanio.X / 2.0f));
                e.posicion.Y = rnd.Next((int)(centro.Y - tamanio.Y / 2.0f), (int)(centro.Y + tamanio.Y / 2.0f));
                e.radius = rnd.Next(2000, 5000) / 10.0f;
                e.rotacion = rnd.Next(0, 360);

                /*switch (rnd.Next(4))
                {
                    case 0:*/
                        e.color = Color.RoyalBlue;
                        /*break;
                    case 1:
                        e.color = Color.Violet;
                        break;
                    case 2:
                        e.color = Color.LightGray;
                        break;
                    case 3:
                        e.color = Color.Yellow;
                        break;
                }*/

                nebulas[i] = e;
            }
        }

        private void CrearEstrellas()
        {
            float radioGalaxia = galaxia.Tamanio.Length;

            float densidadEstrellas = (radioGalaxia - Centro.Length) / radioGalaxia;
            densidadEstrellas = densidadEstrellas * densidadEstrellas;

            int cantEstrellas = CantidadMinimaEstrellas + (int) ((CantidadMaximaEstrellas - CantidadMinimaEstrellas) * densidadEstrellas);

            estrellas = new Estrella[cantEstrellas];

            for (int i = 0; i < cantEstrellas; i++)
            {
                Estrella e = new Estrella();

                e.posicion.X = rnd.Next((int)(centro.X - tamanio.X / 2.0f), (int)(centro.X + tamanio.X / 2.0f));
                e.posicion.Y = rnd.Next((int)(centro.Y - tamanio.Y / 2.0f), (int)(centro.Y + tamanio.Y / 2.0f));
                e.radius = rnd.Next(2000, 4000) / 1000.0f;

                switch (rnd.Next(4))
                {
                    case 0:
                        e.color = Color.RoyalBlue;
                        break;
                    case 1:
                        e.color = Color.Violet;
                        break;
                    case 2:
                        e.color = Color.LightGray;
                        break;
                    case 3:
                        e.color = Color.Yellow;
                        break;
                }

                estrellas[i] = e;
            }
        }

        #endregion

        #region Manejo de Things

        public void AgregarThing(Thing thing)
        {
            if (thing.Centro.X < Centro.X - Tamanio.X / 2.0f ||
                thing.Centro.Y < Centro.Y - Tamanio.Y / 2.0f ||
                thing.Centro.X > Centro.X + Tamanio.X / 2.0f ||
                thing.Centro.Y > Centro.Y + Tamanio.Y / 2.0f)
            {
                throw new Exception("El sector no puede contener el objeto pasado");
            }

            if (things.Contains(thing))
                throw new Exception("El sector ya contiene el objeto pasado");

            things.Add(thing);

            if (thing.Activo)
                thingsActivos++;
        }

        public void EliminarThing(Thing thing)
        {
            if (!things.Contains(thing))
                throw new Exception("El sector no contenia el objeto que se quiso eliminar!!");

            things.Remove(thing);

            if (thing.Activo)
                thingsActivos--;
        }

        public List<Thing> GetThingsVisibles(Frustum frustum)
        {
            List<Thing> thingsVisibles = new List<Thing>();

            foreach (Thing thing in things)
                if (frustum.RectangleInside(thing.Centro, thing.Tamanio))
                    thingsVisibles.Add(thing);

            return thingsVisibles;
        }

        public void Procesar(float elapsedSeconds, long identificadorProcesado)
        {
            RenovarRecursos(elapsedSeconds);

            foreach (Thing thing in things.ToArray()) //Hago un .ToArray() para trabajar sobre una copia del vector
                if (!thing.Eliminado && 
                    thing.identificadorProcesado != identificadorProcesado)
                {
                    thing.identificadorProcesado = identificadorProcesado;
                    thing.Procesar(elapsedSeconds);

                    if (thing is ITieneRecursos)
                        ((ITieneRecursos)thing).RenovarRecursos(elapsedSeconds);
                }
        }

        public void ThingActivado(Thing thing)
        {
            if (!things.Contains(thing))
                throw new Exception("El sector no contenia el objeto que se quiso activar!!");

            thingsActivos++;
        }

        public void ThingDesactivado(Thing thing)
        {
            if (!things.Contains(thing))
                throw new Exception("El sector no contenia el objeto que se quiso desactivar!!");

            thingsActivos--;
        }

        #endregion

        #region Manejo de colisiones entre Things

        public Thing CollidesWithThing(Thing thingToEvaluate, Vector2 Center, Vector2 Size, float rotationInDegress)
        {
            BoundingObject[] boundingObjects = null;

            //Utilizo el centro que me pasan, no el del thing a evaluar porque puede ser un chequeo
            //por cambio de posición
            BoundingCircle bc = new BoundingCircle(Center, thingToEvaluate.BoundingCircle.Radius);

            foreach(Thing thing in things)
            {
                if (thing != thingToEvaluate &&
                    thing.Solido &&
                    !thing.Eliminado)
                {
                    if (thing.BoundingCircle.IntersectsWith(bc))
                    {
                        if (boundingObjects == null)
                            boundingObjects = CreateBoundingObjects(thingToEvaluate, Center, Size, rotationInDegress);

                        BoundingObject[] boundingObjects2 = CreateBoundingObjects(thing, thing.Centro, thing.Tamanio, thing.RotacionEnGrados);

                        foreach (BoundingObject b in boundingObjects)
                            foreach (BoundingObject b2 in boundingObjects2)
                                if (b.IntersectsWith(b2))
                                    return thing;                                    
                    }
                }
            }

            return null;
        }

        static private BoundingObject[] CreateBoundingObjects(Thing thing, Vector2 Center, Vector2 Size, float rotationInDegress)
        {
            BoundingObject[] boundingObjects;

            if (thing != null)
                boundingObjects = (BoundingObject[]) thing.CrearBoundingObjects().Clone();
            else
                boundingObjects = new BoundingObject[] { new BoundingRectangle(new Vector2(0, 0), Size, 0) };

            for (int i = 0; i < boundingObjects.Length; i++)
                boundingObjects[i] = boundingObjects[i].RotateAndTranslate(rotationInDegress, Center);

            return boundingObjects;
        }

        #endregion

        #region Dibujado del fondos y frentes

        public void DibujarFondo()
        {
            DibujarFondoEstrellas();
        }

        public void DibujarFrente()
        {
            DibujarFrenteNebulosa();
        }

        private void DibujarFondoEstrellas()
        {
            Gl.glDisable(Gl.GL_TEXTURE_2D);

            Gl.glBegin(Gl.GL_QUADS);

            foreach (Estrella e in estrellas)
            {
                Graphics.GraphicEngine.Instance.SetColor(e.color);

                Gl.glVertex3f(e.posicion.X - e.radius, e.posicion.Y - e.radius, 1);
                Gl.glVertex3f(e.posicion.X + e.radius, e.posicion.Y - e.radius, 1);
                Gl.glVertex3f(e.posicion.X + e.radius, e.posicion.Y + e.radius, 1);
                Gl.glVertex3f(e.posicion.X - e.radius, e.posicion.Y + e.radius, 1);
            }

            Gl.glEnd();

            Gl.glEnable(Gl.GL_TEXTURE_2D);

            Gl.glColor3f(1.0f, 1.0f, 1.0f);
        }

        private void DibujarFrenteNebulosa()
        {
            if (nebulas != null)
            {
                InicializarRecursos();

                if (texturaNebula == null)
                    texturaNebula = TexturaManager.Instance.CargarTextura(Data.NombresTexturas.Nebulosa);

                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texturaNebula.Id);
                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_COLOR);

                float porcentajeRecursosDisponibles = recursos[0].CantidadDisponible / recursos[0].MaximoDisponible;

                int transparencia = (int) (255.0f * porcentajeRecursosDisponibles);

                foreach (Nebula e in nebulas)
                {
                    Graphics.GraphicEngine.Instance.SetColor(Color.FromArgb(transparencia, e.color));

                    Gl.glTranslatef(e.posicion.X, e.posicion.Y, GraphicEngine.zValue);
                    Gl.glRotatef(e.rotacion, 0, 0, 1);

                    Gl.glBegin(Gl.GL_QUADS);
                    Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-e.radius, -e.radius, 1);
                    Gl.glTexCoord2f(1, 0); Gl.glVertex3f(e.radius, -e.radius, 1);
                    Gl.glTexCoord2f(1, 1); Gl.glVertex3f(e.radius, e.radius, 1);
                    Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-e.radius, e.radius, 1);
                    Gl.glEnd();

                    Gl.glRotatef(-e.rotacion, 0, 0, 1);
                    Gl.glTranslatef(-e.posicion.X, -e.posicion.Y, -GraphicEngine.zValue);
                }

                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

                Gl.glColor3f(1.0f, 1.0f, 1.0f);
            }
        }

        #endregion

        #region Funciones Auxiliares

        public Vector2 BuscarEspacioVacio(float radio)
        {
            Vector2 espacioVacio = Centro;

            for (int i = 0; i < 100; i++)
            {
                espacioVacio = new Vector2(
                    (float) rnd.Next(-TamanioSector / 2, TamanioSector / 2),
                    (float) rnd.Next(-TamanioSector / 2, TamanioSector / 2));

                espacioVacio += Centro;

                if (galaxia.HayThingEnRadio(espacioVacio, radio) == false)
                    break;
            }

            return espacioVacio;
        }

        public Recurso[] BuscarRecursos(Vector2 centro, float radioDeBusqueda)
        {
            InicializarRecursos();

            List<Recurso> recursosEncontrados = new List<EspacioInfinitoDotNet.Recursos.Recurso>();

            recursosEncontrados.AddRange(recursos);

            foreach (Thing thing in things)
            {
                if (thing is ITieneRecursos)
                    if ((thing.Centro - centro).Length < radioDeBusqueda)
                        recursosEncontrados.AddRange(((ITieneRecursos)thing).GetRecursos());
            }

            return recursosEncontrados.ToArray();
        }

        #endregion

        private void InicializarRecursos()
        {
            if (recursos == null)
            {
                if (TieneNebulosa)
                {
                    recursos = new RecursoRenovable[1];
                    recursos[0] = new RecursoRenovableEnSector(this, Recurso.RecursoPlasma, 1000, Sector.TamanioSector / 2, 50, 1000);
                }
                else
                    recursos = new RecursoRenovable[0];
            }
        }

        #region Metodos de ITieneRecursos

        public Recurso[] GetRecursos()
        {
            InicializarRecursos();
            return recursos;
        }

        public void RenovarRecursos(float fDeltaTime)
        {
            if (TieneNebulosa)
            {
                InicializarRecursos();
                recursos[0].Renovar(fDeltaTime);
            }
        }

        #endregion
    }
}
