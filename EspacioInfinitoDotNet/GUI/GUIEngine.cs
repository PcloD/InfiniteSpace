using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Maths;
using Tao.OpenGl;

namespace EspacioInfinitoDotNet.GUI
{
    class GUIEngine
    {
        GUIGraphicEngine guiGraphicEngine;
        Controls.GUIRoot root;

        public Controls.GUIRoot Root
        {
            get { return root; }
        }

	    public GUIEngine()
        {
        }

	    public Boolean Init(GUIGraphicEngine guiGraphicEngine)
        {
            this.guiGraphicEngine = guiGraphicEngine;

            if (guiGraphicEngine.Init() == false)
                return false;

            root = new GUI.Controls.GUIRoot(guiGraphicEngine.Size);

            return true;
        }

        public void Draw()
        {
            //Preparo para dibujar

            root.Size = GraphicEngine.Instance.Size;
            guiGraphicEngine.Size = root.Size;

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();

            Gl.glOrtho(0, root.Size.Width, root.Size.Height, 0, -1.0, 1.0);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();

            Gl.glDisable(Gl.GL_TEXTURE_2D);

            Gl.glEnable(Gl.GL_SCISSOR_TEST);

                root.PrvDraw(guiGraphicEngine);

            Gl.glDisable(Gl.GL_SCISSOR_TEST);

            Gl.glEnable(Gl.GL_TEXTURE_2D);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();
        }

        public void AddEvent(GUIEvent guiEvent)
        {
            root.PrvHandleEvent(guiEvent);
        }
    }

}
