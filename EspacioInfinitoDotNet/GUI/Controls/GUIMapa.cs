using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Universes;
using EspacioInfinitoDotNet.Things;
using EspacioInfinitoDotNet.Maths;
using System.Drawing;
using Tao.OpenGl;
using EspacioInfinitoDotNet.GUI;

namespace EspacioInfinitoDotNet.GUI.Controls
{
    public class GUIMapa : GUIWindow
    {
        public GUIMapa(System.Drawing.Size size)
            : base(size)
        {
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            return false;
        }

        public override void Draw(GUIGraphicEngine guiGraphicEngine)
        {
            Thing thingASeguir = Game.GameEngine.Instance.ThingASeguir;

            if (thingASeguir == null)
                return;

            Galaxia galaxia = thingASeguir.Galaxia;

            //Calculo la posición del mapa en la pantalla
            Vector2 inicioSuperiorIzquierdo = new Vector2(PrvGetAbsolutePosition().X, PrvGetAbsolutePosition().Y);
            Vector2 finInferiorDerecho = new Vector2(PrvGetAbsolutePosition().X + Size.Width, PrvGetAbsolutePosition().Y + Size.Height);

            //Calculo la cantidad óptima de sectores de igual tamaño (cuadrados) a dibujar
            int radioSectoresADibujarEnMapaX = Properties.Settings.Default.RadioSectoresCercanosADibujarEnMapa;
            int radioSectoresADibujarEnMapaY = Properties.Settings.Default.RadioSectoresCercanosADibujarEnMapa;

            Vector2 tamanioSectorEnMapa = new Vector2(
                (finInferiorDerecho.X - inicioSuperiorIzquierdo.X) / (radioSectoresADibujarEnMapaX * 2 + 1),
                (finInferiorDerecho.Y - inicioSuperiorIzquierdo.Y) / (radioSectoresADibujarEnMapaY * 2 + 1));

            float tamanioSectorAUsar;

            if (tamanioSectorEnMapa.X > tamanioSectorEnMapa.Y)
                tamanioSectorAUsar = tamanioSectorEnMapa.X;
            else
                tamanioSectorAUsar = tamanioSectorEnMapa.Y;

            tamanioSectorEnMapa.X = tamanioSectorAUsar;
            tamanioSectorEnMapa.Y = tamanioSectorAUsar;

            radioSectoresADibujarEnMapaX = (int)Math.Ceiling((finInferiorDerecho.X - inicioSuperiorIzquierdo.X) / tamanioSectorEnMapa.X / 2 - 1);
            radioSectoresADibujarEnMapaY = (int)Math.Ceiling((finInferiorDerecho.Y - inicioSuperiorIzquierdo.Y) / tamanioSectorEnMapa.Y / 2 - 1);

            SectorID sectorIDCentro = galaxia.GetSectorEnPosicion(thingASeguir.Centro).SectorID;

            Gl.glBegin(Gl.GL_QUADS);

            //Dibujo un fondo negro
            GraphicEngine.Instance.SetColor(Color.FromArgb(200, Color.Black));

            Gl.glVertex3f(inicioSuperiorIzquierdo.X, inicioSuperiorIzquierdo.Y, GraphicEngine.zValue);
            Gl.glVertex3f(inicioSuperiorIzquierdo.X, finInferiorDerecho.Y, GraphicEngine.zValue);
            Gl.glVertex3f(finInferiorDerecho.X, finInferiorDerecho.Y, GraphicEngine.zValue);
            Gl.glVertex3f(finInferiorDerecho.X, inicioSuperiorIzquierdo.Y, GraphicEngine.zValue);

            Gl.glEnd();

            //Dibujo el borde del mapa
            Gl.glBegin(Gl.GL_LINE_LOOP);

            GraphicEngine.Instance.SetColor(Color.FromArgb(100, Color.LightBlue));

            Gl.glVertex3f(inicioSuperiorIzquierdo.X, inicioSuperiorIzquierdo.Y, GraphicEngine.zValue);
            Gl.glVertex3f(inicioSuperiorIzquierdo.X, finInferiorDerecho.Y, GraphicEngine.zValue);
            Gl.glVertex3f(finInferiorDerecho.X, finInferiorDerecho.Y, GraphicEngine.zValue);
            Gl.glVertex3f(finInferiorDerecho.X, inicioSuperiorIzquierdo.Y, GraphicEngine.zValue);

            Gl.glEnd();

            //Dibujo las lineas de los sectores y su contenido

            //Calculo el offset dentro del mapa debido al desplazamiento del jugador en el sector
            Vector2 offset = new Vector2(
                (thingASeguir.Centro.X - galaxia.GetSector(sectorIDCentro).Centro.X) / Sector.TamanioSector * tamanioSectorEnMapa.X,
                (thingASeguir.Centro.Y - galaxia.GetSector(sectorIDCentro).Centro.Y) / Sector.TamanioSector * tamanioSectorEnMapa.Y);

            //Dibujo un sector menos y un sector mas en cada uno de los ejes (X,Y) para compensar los casos en los que
            //el offset es distinto a cero, en cuyo caso se deberian ver parcialmente algunos otros sectores

            Vector2 from = new Vector2(
                inicioSuperiorIzquierdo.X - offset.X - tamanioSectorEnMapa.X,
                finInferiorDerecho.Y + offset.Y + tamanioSectorEnMapa.Y);

            Gl.glBegin(Gl.GL_LINES);

            for (int x = sectorIDCentro.X - radioSectoresADibujarEnMapaX - 1; x <= sectorIDCentro.X + radioSectoresADibujarEnMapaX + 1; x++)
            {
                from.Y = finInferiorDerecho.Y + offset.Y + tamanioSectorEnMapa.Y;

                GraphicEngine.Instance.SetColor(Color.FromArgb(100, Color.LightBlue));

                //Linea vertical que abarca todos los sectores
                if (Properties.Settings.Default.DibujarLineasMapa)
                {
                    Gl.glVertex3f(from.X, inicioSuperiorIzquierdo.Y, GraphicEngine.zValue);
                    Gl.glVertex3f(from.X, finInferiorDerecho.Y, GraphicEngine.zValue);
                }

                Vector2 to = new Vector2(from.X + tamanioSectorEnMapa.X, from.Y - tamanioSectorEnMapa.Y);

                for (int y = sectorIDCentro.Y - radioSectoresADibujarEnMapaY - 1; y <= sectorIDCentro.Y + radioSectoresADibujarEnMapaY + 1; y++)
                {
                    Sector sector = galaxia.GetSector(new SectorID(x, y));

                    bool cambiarFondo = false;
                    Color nuevoColorFondo = Color.Black;

                    if (sector.TieneNebulosa)
                    {
                        //Relleno el sector de un color distinto si tiene nebulosa
                        cambiarFondo = true;
                        nuevoColorFondo = Color.FromArgb(50, Color.DarkViolet);
                    }

                    if (thingASeguir is ThingNave)
                    {
                        ThingNave thingNave = (ThingNave)thingASeguir;

                        if (sector.Faccion != null &&
                            sector.Faccion.GetRelacion(thingNave.Faccion) == Faccion.RelacionConOtraFaccionEnum.Agresiva)
                        {
                            cambiarFondo = true;
                            nuevoColorFondo = Color.FromArgb(50, Color.Red);
                        }
                    }

                    if (cambiarFondo)
                    {
                        Gl.glEnd();

                        Gl.glBegin(Gl.GL_QUADS);

                        GraphicEngine.Instance.SetColor(nuevoColorFondo);

                        Gl.glVertex3f(from.X, from.Y, GraphicEngine.zValue);
                        Gl.glVertex3f(from.X, to.Y, GraphicEngine.zValue);
                        Gl.glVertex3f(to.X, to.Y, GraphicEngine.zValue);
                        Gl.glVertex3f(to.X, from.Y, GraphicEngine.zValue);

                        Gl.glEnd();

                        Gl.glBegin(Gl.GL_LINES);
                    }

                    //Dibujo las naves que haya en el sector

                    if (sector.ThingsCount > 0)
                        Gl.glEnd();

                    foreach (Thing thing in sector.Things)
                    {
                        //Dibujo todos los elementos en el mapa, excepto la nave del jugador
                        //que la dibujo al final para que queda arriba de todo
                        if (!(thing is ThingNaveJugador))
                            DibujarThing(guiGraphicEngine, thing, sector, from, to, tamanioSectorAUsar);
                    }

                    if (sector.ThingsCount > 0)
                        Gl.glBegin(Gl.GL_LINES);

                    GraphicEngine.Instance.SetColor(Color.FromArgb(100, Color.LightBlue));
                    
                    //Linea horizontal que abarca solo este sector
                    if (Properties.Settings.Default.DibujarLineasMapa)
                    {
                        Gl.glVertex3f(from.X, to.Y, GraphicEngine.zValue);
                        Gl.glVertex3f(to.X, to.Y, GraphicEngine.zValue);
                    }

                    from.Y -= tamanioSectorEnMapa.Y;
                    to.Y -= tamanioSectorEnMapa.Y;
                }

                from.X += tamanioSectorEnMapa.X;
            }

            Gl.glEnd();

            //Dibujo la nave del jugador en el centro del mapa, lo cual siempre va a ser correcto 
            //porque el mapa esta centrado en el jugador

            //DibujarThing(guiGraphicEngine, naveJugador, galaxia.GetSectorEnPosicion(naveJugador.Centro), from, to, tamanioSectorAUsar);
            
            Vector2 centroMapa = new Vector2(
                (inicioSuperiorIzquierdo.X + finInferiorDerecho.X) / 2,
                (inicioSuperiorIzquierdo.Y + finInferiorDerecho.Y) / 2);

            centroMapa.X -= PrvGetAbsolutePosition().X;
            centroMapa.Y -= PrvGetAbsolutePosition().Y;

            thingASeguir.DibujarEnMapa(centroMapa, tamanioSectorAUsar / 3, guiGraphicEngine);
        }

        private void DibujarThing(GUIGraphicEngine guiGraphicEngine, Thing thing, Sector sector, Vector2 from, Vector2 to, float tamanioSectorAUsar)
        {
            Vector2 centro = new Vector2((from.X + to.X) / 2, (from.Y + to.Y) / 2);

            Vector2 offsetCentro = (sector.Centro - thing.Centro) * tamanioSectorAUsar / Sector.TamanioSector;

            centro.X -= offsetCentro.X;
            centro.Y += offsetCentro.Y;

            Point p = PrvGetAbsolutePosition();

            centro.X -= p.X;
            centro.Y -= p.Y;

            thing.DibujarEnMapa(centro, tamanioSectorAUsar / 4, guiGraphicEngine);
        }
    }
}
