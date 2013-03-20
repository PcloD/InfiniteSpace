using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Maths;
using System.Drawing;
using Tao.OpenGl;

namespace EspacioInfinitoDotNet.GUI
{
    public class GUIGraphicEngine
    {
        #region Atributos

        Stack<Rectangle> boundsStack = new Stack<Rectangle>();
        Size size;
        Rectangle bounds;
        GraphicEngine graphicEngine;
        Color foreColor = Color.White;
        Color backColor = Color.Black;
        Color textColor = Color.White;
        Size[] tamaniosLetras;

        private IntPtr bitmapFont = Tao.FreeGlut.Glut.GLUT_BITMAP_HELVETICA_12;
        int altoLetra;
        int altoLetraConEspacioAbajo;

        public Size Size
        {
            get { return size; }
            set { this.size = value; }
        }

        public Rectangle Bounds
        {
            get { return bounds; }
        }

        public Color ForeColor
        {
            get { return foreColor; }
            set { this.foreColor = value; }
        }

        public Color BackColor
        {
            get { return backColor; }
            set { this.backColor = value; }
        }

        public Color TextColor
        {
            get { return textColor; }
            set { this.textColor = value; }
        }

        #endregion

        public GUIGraphicEngine(GraphicEngine graphicEngine)
        {
            this.graphicEngine = graphicEngine;
        }

        public bool Init()
        {
            size = GraphicEngine.Instance.Size;

            bounds.Size = size;

            InicialiarFonts();

            return (true);
        }

        private void InicialiarFonts()
        {
            tamaniosLetras = new Size[256];

            altoLetra = Tao.FreeGlut.Glut.glutBitmapHeight(bitmapFont);
            altoLetraConEspacioAbajo = altoLetra * 12 / 10; //Compenso por los caracteres que van por debajo de la linea

            for (int i = 0; i < 256; i++)
                tamaniosLetras[i] = new Size(Tao.FreeGlut.Glut.glutBitmapWidth(bitmapFont, (char) i), altoLetra);
        }

        #region Metodos internos

        private void SetClipRectangle(Rectangle rectangle)
        {
            Gl.glScissor(rectangle.X, Size.Height - (rectangle.Y + rectangle.Height), rectangle.Width, rectangle.Height);
        }

        internal void PushBounds(Rectangle bounds)
        {
            boundsStack.Push(this.bounds);

            this.bounds = new Rectangle(
                this.bounds.X + bounds.X,
                this.bounds.Y + bounds.Y,
                bounds.Width,
                bounds.Height);

            SetClipRectangle(this.bounds);
        }

        internal void PopBounds()
        {
            this.bounds = boundsStack.Pop();

            SetClipRectangle(this.bounds);
        }

        private Point TranslatePoint(Point point)
        {
            return new Point(point.X + bounds.X, point.Y + bounds.Y);
        }

        private Rectangle TranslateRectangle(Rectangle rectangle)
        {
            return new Rectangle(TranslatePoint(rectangle.Location), rectangle.Size);
        }

        #endregion

        public void DrawRectangle(Rectangle rectangle)
        {
            DrawRectangle(rectangle, backColor);
        }

        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            rectangle = TranslateRectangle(rectangle);

            Gl.glBegin(Gl.GL_QUADS);

            Gl.glColor4ub(color.R, color.G, color.B, color.A);

            Gl.glVertex3i(rectangle.X, rectangle.Y, 0);
            Gl.glVertex3i(rectangle.X, rectangle.Y + rectangle.Height, 0);
            Gl.glVertex3i(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0);
            Gl.glVertex3i(rectangle.X + rectangle.Width, rectangle.Y, 0);

            Gl.glColor4ub(255, 255, 255, 255);

            Gl.glEnd();
        }

        public void DrawRectangleFrame(Rectangle rectangle, int width)
        {
            DrawRectangleFrame(rectangle, foreColor, width);
        }

        public void DrawRectangleFrame(Rectangle rectangle, Color color, int width)
        {
            Rectangle rectTemp = new Rectangle();

            //Dibujo las barras horizonaltes
            rectTemp.Location = rectangle.Location;
            rectTemp.X += width;
            rectTemp.Width = rectangle.Size.Width - width * 2;
            rectTemp.Height = width;

            DrawRectangle(rectTemp, color);

            rectTemp.Y = rectangle.Y + rectangle.Height - width;

            DrawRectangle(rectTemp, color);

            //Dibujo las barras verticales
            rectTemp.Location = rectangle.Location;
            rectTemp.Width = width;
            rectTemp.Height = rectangle.Height;

            DrawRectangle(rectTemp, color);

            rectTemp.X = rectangle.X + rectangle.Width - width;

            DrawRectangle(rectTemp, color);
        }

        public void DrawText(Point position, string text)
        {
            DrawText(position, text, textColor);
        }

        public void DrawText(Point position, string text, Color color)
        {
            position = TranslatePoint(position);

            Gl.glColor4ub(color.R, color.G, color.B, color.A);

            Gl.glRasterPos2i(position.X, position.Y + altoLetra);

            Tao.FreeGlut.Glut.glutBitmapString(bitmapFont, text);

            Gl.glColor4ub(255, 255, 255, 255);
        }

        public class DrawTextInfo
        {
            internal List<Point> location = new List<Point>();
            internal List<String> text = new List<string>();
        }

        public DrawTextInfo CalculateDrawTextInfo(Rectangle rectangle, string text)
        {
            DrawTextInfo dtInfo = new DrawTextInfo();

            Point position = new Point(0, 0);
            int altoLetra = tamaniosLetras[(int)' '].Height;

            if (position.Y + altoLetra <= rectangle.Height)
            {
                int ancho = 0;

                text = text.Replace("\n", " \n ");
                String[] palabras = text.Split(' ');
                int nPalabra = 0;

                StringBuilder sb = new StringBuilder(text.Length);
                string texto = "";
                bool imprimir = false;

                while (nPalabra < palabras.Length)
                {
                    String palabra = palabras[nPalabra];

                    int anchoPalabra = 0;

                    if (palabra != "\n")
                    {
                        foreach (Char c in palabra)
                        {
                            if (c >= 256)
                                anchoPalabra += tamaniosLetras[' '].Width;
                            else
                                anchoPalabra += tamaniosLetras[c].Width;

                        }

                        if (anchoPalabra + ancho > rectangle.Width)
                        {
                            if (ancho == 0)
                            {
                                texto = palabra;
                                nPalabra++;
                            }
                            else
                            {
                                texto = sb.ToString();
                            }

                            imprimir = true;
                        }
                        else
                        {
                            sb.Append(palabra);
                            sb.Append(' ');
                            ancho += anchoPalabra + tamaniosLetras[' '].Width;
                            nPalabra++;
                        }
                    }
                    else
                    {
                        imprimir = true;
                        texto = sb.ToString();
                        nPalabra++;
                    }

                    if (!imprimir && nPalabra == palabras.Length)
                    {
                        texto = sb.ToString();
                        imprimir = true;
                    }

                    if (imprimir)
                    {
                        dtInfo.location.Add(new Point(position.X + rectangle.X, position.Y + rectangle.Y));
                        dtInfo.text.Add(texto);

                        position.Y += altoLetra;

                        if (position.Y + altoLetra > rectangle.Height)
                            break;

                        ancho = 0;
                        sb.Length = 0;
                        imprimir = false;
                    }
                }
            }

            return dtInfo;
        }

        public void DrawText(Rectangle rectangle, string text, Color color)
        {
            DrawText(CalculateDrawTextInfo(rectangle, text), color);
        }

        public void DrawText(DrawTextInfo dtInfo, Color color)
        {
            for (int i = 0; i < dtInfo.location.Count; i++)
                DrawText(dtInfo.location[i], dtInfo.text[i], color);
        }

        /*public Size GetTextSizePixels(string text, int maxWidth)
        {
            int altoLetra = tamaniosLetras[(int) ' '].Height;

            int sizeX = 0;
            int sizeY = altoLetra;

            foreach (Char c in text.ToCharArray())
            {
                if (c < 256)
                {
                    if (c == '\n' || c == '\r')
                    {
                        sizeX = 0;
                        sizeY += altoLetra;
                    }
                    else
                    {
                        int anchoLetra = tamaniosLetras[(int)c].Width;

                        if (sizeX + anchoLetra > maxWidth)
                        {
                            sizeY += altoLetra;

                            sizeX = anchoLetra;
                        }
                        else
                        {
                            sizeX += anchoLetra;
                        }
                    }
                }
            }

            return new Size(maxWidth, sizeY);
        }*/

        public Size GetTextSizePixels(string text)
        {
            int alto = altoLetra;
            int ancho = Tao.FreeGlut.Glut.glutBitmapLength(bitmapFont, text);

            return new Size(ancho, alto);
        }
    }
}
