using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace EspacioInfinitoDotNet.Graphics
{
    public class Textura
    {
        private int id;
        public int Id
        {
            get { return id; }
        }

        private string nombreArchivo;
        public string FileName
        {
            get { return nombreArchivo; }
        }

        private Bitmap bitmap;
        public Bitmap Bitmap
        {
            get { return bitmap; }
        }

        public Textura(int id, string nombreArchivo, Bitmap bitmap)
        {
            this.id = id;
            this.nombreArchivo = nombreArchivo;
            this.bitmap = bitmap;
        }
    }
}
