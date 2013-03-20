using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using Tao.OpenGl;

namespace EspacioInfinitoDotNet.Graphics
{
    public class TexturaManager
    {
        #region Manejo del singleton

        static TexturaManager tm;

        private TexturaManager()
        {
        }

        public static TexturaManager Instance
        {
            get
            {
                if (tm == null)
                    tm = new TexturaManager();

                return tm;
            }
        }

        #endregion

        Dictionary<String, Textura> texturasCargadas = new Dictionary<string, Textura>();

        public void Inicializar()
        {
            System.Reflection.FieldInfo[] campos = typeof(Data.NombresTexturas).GetFields();

            foreach (System.Reflection.FieldInfo campo in campos)
            {
                String nombreTextura = (String) campo.GetValue(null);

                CargarTextura(nombreTextura);
            }

        }

        public Textura CargarTextura(string fileName)
        {
            fileName = fileName.ToUpper();

            if (!texturasCargadas.ContainsKey(fileName))
                texturasCargadas.Add(fileName, CargarTexturaDeArchivo(fileName));
            
            return texturasCargadas[fileName];
        }

        #region LoadTextureFromFile

        private Textura CargarTexturaDeArchivo(string nombreArchivo)
        {
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            if (!System.IO.File.Exists(nombreArchivo))
                throw new System.IO.FileNotFoundException("No se encontro el archivo", nombreArchivo);

            Bitmap image = new Bitmap(nombreArchivo);
            int[] textureId = new int[1];

            Gl.glGenTextures(1, textureId);

            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            
            Rectangle rectangle = new Rectangle(0, 0, image.Width, image.Height);

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureId[0]);

            BitmapData bitmapData;

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);

            //Gl.glTexParameterfv(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_BORDER_COLOR, new float[] { 0, 0, 0, 0 });

            if (image.PixelFormat == PixelFormat.Format32bppArgb)
            {
                bitmapData = image.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGBA8, image.Width, image.Height, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);

                //Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, image.Width, image.Height, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
            }
            else
            {
                bitmapData = image.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGB8, image.Width, image.Height, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);

                //Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB8, image.Width, image.Height, 0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
            }

            image.UnlockBits(bitmapData);

            return new Textura(textureId[0], nombreArchivo, image);;
        }

        #endregion
    }
}
