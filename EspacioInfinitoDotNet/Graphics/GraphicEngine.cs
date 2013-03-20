using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Tao.OpenGl;
using Tao.Sdl;
using EspacioInfinitoDotNet.Maths;

namespace EspacioInfinitoDotNet.Graphics
{
    public class GraphicEngine
    {
        public const float zValue = 1.0f;

        #region Manejo del singleton

        static GraphicEngine ge;

        private GraphicEngine()
        {
        }

        public static GraphicEngine Instance
        {
            get
            {
                if (ge == null)
                    ge = new GraphicEngine();

                return ge;
            }
        }

        #endregion

        #region Atributos

        private Size size;
        private IntPtr sdlSurfacePtr;

        public Size Size
        {
            get { return size; }
        }

        #endregion

        #region Inicialización y finalización

        public bool Inicializar(string nombreVentana, int ancho, int alto, int bpp, bool pantallaCompleta)
        {
            int flags = (Sdl.SDL_HWSURFACE | Sdl.SDL_DOUBLEBUF | Sdl.SDL_OPENGL);

            if (pantallaCompleta)
                flags |= Sdl.SDL_FULLSCREEN;

            Sdl.SDL_WM_SetCaption(nombreVentana, "");

            sdlSurfacePtr = Sdl.SDL_SetVideoMode(
                ancho,
                alto,
                bpp,
                flags);

            if (sdlSurfacePtr == IntPtr.Zero)
                return false;

            TexturaManager.Instance.Inicializar();

            size = new Size(ancho, alto);

            ReSizeGLScene(Size.Width, Size.Height);

            InitGL();

            Tao.FreeGlut.Glut.glutInit();

            return true;
        }

        #region Inicialización de OpenGL

        private bool InitGL()
        {
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glClearColor(0, 0, 0, 0.5f);
            //Gl.glClearDepth(1);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            //Gl.glDepthFunc(Gl.GL_LEQUAL);
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
            Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT, Gl.GL_NICEST);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);

            Gl.glEnable(Gl.GL_LINE_SMOOTH); //Activo el antialiasing de lineas para que los fonts salgan bien
            //Gl.glEnable(Gl.GL_POLYGON_SMOOTH);

            Gl.glEnable(Gl.GL_MULTISAMPLE_ARB);
            Gl.glHint(Gl.GL_MULTISAMPLE_FILTER_HINT_NV, Gl.GL_NICEST);

            return true;
        }

        private void ReSizeGLScene(int width, int height)
        {
            if (height == 0) // Prevent A Divide By Zero...
            {
                height = 1;                                                     // By Making Height Equal To One
            }

            Gl.glViewport(0, 0, width, height);                                 // Reset The Current Viewport
            Gl.glMatrixMode(Gl.GL_PROJECTION);                                  // Select The Projection Matrix
            Gl.glLoadIdentity();                                                // Reset The Projection Matrix
            Glu.gluPerspective(45, (float)width / (float)height, 0.1, -Game.GameEngine.ZoomMaximo + 100.0f);          // Calculate The Aspect Ratio Of The Window
            Gl.glMatrixMode(Gl.GL_MODELVIEW);                                   // Select The Modelview Matrix
            Gl.glLoadIdentity();                                                // Reset The Modelview Matrix
        }

        #endregion

        public void Finalizar()
        {
        }

        #endregion

        #region Comienzo y Fin de dibujado

        public void IniciarDibujado()
        {
            Gl.glClearColor(0, 0, 0, 0);

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Gl.glLoadIdentity();

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
        }

        public void FinalizarDibujado()
        {
            Sdl.SDL_GL_SwapBuffers();
        }

        #endregion

        #region Frustum

        #region GetFrustum - Sacado del articulo http://www.artlum.com/pub/frustumculling.html

        public Frustum GetFrustum()
        {
            float[][] frustum;

            frustum = new float[6][];
            for (int i = 0; i < 6; i++)
                frustum[i] = new float[4];

            float[] proj = new float[16];
            float[] modl = new float[16];
            float[] clip = new float[16];
            float   t;

            /* Get the current PROJECTION matrix from OpenGL */
            Gl.glGetFloatv(Gl.GL_PROJECTION_MATRIX, proj);

            /* Get the current MODELVIEW matrix from OpenGL */
            Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, modl);

            /* Combine the two matrices (multiply projection by modelview) */
            clip[ 0] = modl[ 0] * proj[ 0] + modl[ 1] * proj[ 4] + modl[ 2] * proj[ 8] + modl[ 3] * proj[12];
            clip[ 1] = modl[ 0] * proj[ 1] + modl[ 1] * proj[ 5] + modl[ 2] * proj[ 9] + modl[ 3] * proj[13];
            clip[ 2] = modl[ 0] * proj[ 2] + modl[ 1] * proj[ 6] + modl[ 2] * proj[10] + modl[ 3] * proj[14];
            clip[ 3] = modl[ 0] * proj[ 3] + modl[ 1] * proj[ 7] + modl[ 2] * proj[11] + modl[ 3] * proj[15];

            clip[ 4] = modl[ 4] * proj[ 0] + modl[ 5] * proj[ 4] + modl[ 6] * proj[ 8] + modl[ 7] * proj[12];
            clip[ 5] = modl[ 4] * proj[ 1] + modl[ 5] * proj[ 5] + modl[ 6] * proj[ 9] + modl[ 7] * proj[13];
            clip[ 6] = modl[ 4] * proj[ 2] + modl[ 5] * proj[ 6] + modl[ 6] * proj[10] + modl[ 7] * proj[14];
            clip[ 7] = modl[ 4] * proj[ 3] + modl[ 5] * proj[ 7] + modl[ 6] * proj[11] + modl[ 7] * proj[15];

            clip[ 8] = modl[ 8] * proj[ 0] + modl[ 9] * proj[ 4] + modl[10] * proj[ 8] + modl[11] * proj[12];
            clip[ 9] = modl[ 8] * proj[ 1] + modl[ 9] * proj[ 5] + modl[10] * proj[ 9] + modl[11] * proj[13];
            clip[10] = modl[ 8] * proj[ 2] + modl[ 9] * proj[ 6] + modl[10] * proj[10] + modl[11] * proj[14];
            clip[11] = modl[ 8] * proj[ 3] + modl[ 9] * proj[ 7] + modl[10] * proj[11] + modl[11] * proj[15];

            clip[12] = modl[12] * proj[ 0] + modl[13] * proj[ 4] + modl[14] * proj[ 8] + modl[15] * proj[12];
            clip[13] = modl[12] * proj[ 1] + modl[13] * proj[ 5] + modl[14] * proj[ 9] + modl[15] * proj[13];
            clip[14] = modl[12] * proj[ 2] + modl[13] * proj[ 6] + modl[14] * proj[10] + modl[15] * proj[14];
            clip[15] = modl[12] * proj[ 3] + modl[13] * proj[ 7] + modl[14] * proj[11] + modl[15] * proj[15];

            /* Extract the numbers for the RIGHT plane */
            frustum[0][0] = clip[ 3] - clip[ 0];
            frustum[0][1] = clip[ 7] - clip[ 4];
            frustum[0][2] = clip[11] - clip[ 8];
            frustum[0][3] = clip[15] - clip[12];

            /* Normalize the result */
            t = (float) Math.Sqrt(frustum[0][0] * frustum[0][0] + frustum[0][1] * frustum[0][1] + frustum[0][2] * frustum[0][2]);
            frustum[0][0] /= t;
            frustum[0][1] /= t;
            frustum[0][2] /= t;
            frustum[0][3] /= t;

            /* Extract the numbers for the LEFT plane */
            frustum[1][0] = clip[ 3] + clip[ 0];
            frustum[1][1] = clip[ 7] + clip[ 4];
            frustum[1][2] = clip[11] + clip[ 8];
            frustum[1][3] = clip[15] + clip[12];

            /* Normalize the result */
            t = (float) Math.Sqrt( frustum[1][0] * frustum[1][0] + frustum[1][1] * frustum[1][1] + frustum[1][2] * frustum[1][2] );
            frustum[1][0] /= t;
            frustum[1][1] /= t;
            frustum[1][2] /= t;
            frustum[1][3] /= t;

            /* Extract the BOTTOM plane */
            frustum[2][0] = clip[ 3] + clip[ 1];
            frustum[2][1] = clip[ 7] + clip[ 5];
            frustum[2][2] = clip[11] + clip[ 9];
            frustum[2][3] = clip[15] + clip[13];

            /* Normalize the result */
            t = (float) Math.Sqrt(frustum[2][0] * frustum[2][0] + frustum[2][1] * frustum[2][1] + frustum[2][2] * frustum[2][2]);
            frustum[2][0] /= t;
            frustum[2][1] /= t;
            frustum[2][2] /= t;
            frustum[2][3] /= t;

            /* Extract the TOP plane */
            frustum[3][0] = clip[ 3] - clip[ 1];
            frustum[3][1] = clip[ 7] - clip[ 5];
            frustum[3][2] = clip[11] - clip[ 9];
            frustum[3][3] = clip[15] - clip[13];

            /* Normalize the result */
            t = (float) Math.Sqrt(frustum[3][0] * frustum[3][0] + frustum[3][1] * frustum[3][1] + frustum[3][2] * frustum[3][2]);
            frustum[3][0] /= t;
            frustum[3][1] /= t;
            frustum[3][2] /= t;
            frustum[3][3] /= t;

            /* Extract the FAR plane */
            frustum[4][0] = clip[ 3] - clip[ 2];
            frustum[4][1] = clip[ 7] - clip[ 6];
            frustum[4][2] = clip[11] - clip[10];
            frustum[4][3] = clip[15] - clip[14];

            /* Normalize the result */
            t = (float) Math.Sqrt(frustum[4][0] * frustum[4][0] + frustum[4][1] * frustum[4][1] + frustum[4][2] * frustum[4][2]);
            frustum[4][0] /= t;
            frustum[4][1] /= t;
            frustum[4][2] /= t;
            frustum[4][3] /= t;

            /* Extract the NEAR plane */
            frustum[5][0] = clip[ 3] + clip[ 2];
            frustum[5][1] = clip[ 7] + clip[ 6];
            frustum[5][2] = clip[11] + clip[10];
            frustum[5][3] = clip[15] + clip[14];

            /* Normalize the result */
            t = (float) Math.Sqrt(frustum[5][0] * frustum[5][0] + frustum[5][1] * frustum[5][1] + frustum[5][2] * frustum[5][2]);
            frustum[5][0] /= t;
            frustum[5][1] /= t;
            frustum[5][2] /= t;
            frustum[5][3] /= t;

            return new Frustum(frustum);
        }

        #endregion

        #endregion

        #region Colores

        public void SetColor(Color color)
        {
            Gl.glColor4f(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
        }

        #endregion

        #region Dibujar Texto

        private float strokeFontScale = 0.006562498f; /*Sale de dividir 1.0f por el alto devuelto por la funcion Tao.FreeGlut.Glut.glutStrokeHeight(strokeFont)*/
        private IntPtr strokeFont = Tao.FreeGlut.Glut.GLUT_STROKE_ROMAN;
        private IntPtr bitmapFont = Tao.FreeGlut.Glut.GLUT_BITMAP_HELVETICA_12;

        public void DrawTextStroke(PointF center, Color color, float fontSize, string text)
        {
            Gl.glPushMatrix();

                Gl.glDisable(Gl.GL_TEXTURE_2D);

                Gl.glTranslatef(center.X, center.Y, -1.0f);

                Gl.glScalef(fontSize * strokeFontScale, fontSize * strokeFontScale, 0);

                Gl.glColor3f(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);

                Tao.FreeGlut.Glut.glutStrokeString(strokeFont, text);

                Gl.glColor3f(1, 1, 1);
                
                Gl.glEnable(Gl.GL_TEXTURE_2D);

            Gl.glPopMatrix();
        }

        public void DrawTextStrokeCenteredX(float centerX, float topLeftY, Color color, float fontSize, string text)
        {
            float ancho = Tao.FreeGlut.Glut.glutStrokeLength(strokeFont, text);
            float alto = Tao.FreeGlut.Glut.glutStrokeHeight(strokeFont);

            ancho *= fontSize * strokeFontScale;
            alto *= fontSize * strokeFontScale;

            DrawTextStroke(new PointF(centerX - ancho / 2, topLeftY - alto), color, fontSize, text);
        }

        public void DrawTextBitmap(Point position, Color color, string text)
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            Gl.glOrtho(0, Size.Width, Size.Height, 0, -1.0, 1.0);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();

                Gl.glDisable(Gl.GL_TEXTURE_2D);

                    int alto = Tao.FreeGlut.Glut.glutBitmapHeight(bitmapFont);

                    SetColor(color);

                    Gl.glRasterPos2i(position.X, position.Y + alto);

                    Tao.FreeGlut.Glut.glutBitmapString(bitmapFont, text);

                    SetColor(Color.White);

                Gl.glEnable(Gl.GL_TEXTURE_2D);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();
        }

        #endregion

        #region Dibujar Rectangulos

        public void DrawRectangle(Vector2 center, float rotationInDegrees, Vector2 size, Textura Textura)
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Textura.Id);
            SetColor(Color.White);

            float halfX = size.X / 2.0f;
            float halfY = size.Y / 2.0f;

            Gl.glTranslatef(center.X, center.Y, 0.0f);
            Gl.glRotatef(rotationInDegrees, 0, 0, 1);
                Gl.glBegin(Gl.GL_QUADS);
                    Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-halfX, -halfY, zValue);
                    Gl.glTexCoord2f(1, 0); Gl.glVertex3f(halfX, -halfY, zValue);
                    Gl.glTexCoord2f(1, 1); Gl.glVertex3f(halfX, halfY, zValue);
                    Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-halfX, halfY, zValue);
                Gl.glEnd();
            Gl.glRotatef(-rotationInDegrees, 0, 0, 1);
            Gl.glTranslatef(-center.X, -center.Y, 0.0f);
        }

        public void DrawRectangle(Vector2 center, float rotationInDegrees, Vector2 size, Textura Textura, Color color)
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Textura.Id);
            SetColor(color);

            float halfX = size.X / 2.0f;
            float halfY = size.Y / 2.0f;
            
            Gl.glTranslatef(center.X, center.Y, 0.0f);
            Gl.glRotatef(rotationInDegrees, 0, 0, 1);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(0, 0); Gl.glVertex3f(-halfX, -halfY, zValue);
            Gl.glTexCoord2f(1, 0); Gl.glVertex3f(halfX, -halfY, zValue);
            Gl.glTexCoord2f(1, 1); Gl.glVertex3f(halfX, halfY, zValue);
            Gl.glTexCoord2f(0, 1); Gl.glVertex3f(-halfX, halfY, zValue);
            Gl.glEnd();
            Gl.glRotatef(-rotationInDegrees, 0, 0, 1);
            Gl.glTranslatef(-center.X, -center.Y, 0.0f);

            SetColor(Color.White);
        }


        public void DrawRectangle(Vector2 center, float rotationInDegrees, Vector2 size, Color color)
        {
            float halfX = size.X / 2.0f;
            float halfY = size.Y / 2.0f;

            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glTranslatef(center.X, center.Y, 0.0f);
            Gl.glRotatef(rotationInDegrees, 0, 0, 1);
                Gl.glBegin(Gl.GL_QUADS);
                SetColor(color);
                Gl.glVertex3f(-halfX, -halfY, zValue);
                Gl.glVertex3f(halfX, -halfY, zValue);
                Gl.glVertex3f(halfX, halfY, zValue);
                Gl.glVertex3f(-halfX, halfY, zValue);
                SetColor(Color.White);
                Gl.glEnd();
            Gl.glRotatef(-rotationInDegrees, 0, 0, 1);
            Gl.glTranslatef(-center.X, -center.Y, 0.0f);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
        }

        public void DrawRectangleBorder(Vector2 center, float rotationInDegrees, Vector2 size, Color color)
        {
            float halfX = size.X / 2.0f;
            float halfY = size.Y / 2.0f;

            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glTranslatef(center.X, center.Y, 0.0f);
            Gl.glRotatef(rotationInDegrees, 0, 0, 1);
                Gl.glBegin(Gl.GL_LINE_LOOP);
                SetColor(color);
                Gl.glVertex3f(-halfX, -halfY, zValue);
                Gl.glVertex3f(halfX, -halfY, zValue);
                Gl.glVertex3f(halfX, halfY, zValue);
                Gl.glVertex3f(-halfX, halfY, zValue);
                SetColor(Color.White);
            Gl.glEnd();
            Gl.glRotatef(-rotationInDegrees, 0, 0, 1);
            Gl.glTranslatef(-center.X, -center.Y, 0.0f);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
        }

        #endregion
    }
}
