using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Maths;
using System.Drawing;
using EspacioInfinitoDotNet.GUI;
using Tao.Sdl;

namespace EspacioInfinitoDotNet.GUI.Controls
{
    public class GUICheckbox : GUIWindow
    {
        #region Atributos

        Color backColor = Color.FromArgb(0, Color.Black);
        Color textColor = Color.White;
        String text = "";
        bool chk = false;

        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        public String Text
        {
            get { return text; }
            set { text = value; }
        }

        public bool Checked
        {
            get { return chk; }
            set { chk = value; }
        }

        #endregion

        public GUICheckbox(Size size)
            : base(size)
        {
            Focusable = true;
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            bool handled = false;

            if (guiEvent is GUIEventMouseButtonPressed)
            {
                chk = !chk;
                handled = true;
            }
            else if (guiEvent is GUIEventKeyPressed)
            {
                GUIEventKeyPressed guiEventKey = (GUIEventKeyPressed)guiEvent;

                if (guiEventKey.key == Sdl.SDLK_SPACE ||
                    guiEventKey.key == Sdl.SDLK_RETURN)
                {
                    chk = !chk;
                    handled = true;
                }
                else if (guiEventKey.key == Sdl.SDLK_DOWN)
                {
                    Father.FocusNextChild();
                    handled = true;
                }
                else if (guiEventKey.key == Sdl.SDLK_UP)
                {
                    Father.FocusPreviusChild();
                    handled = true;
                }
            }

            return handled;
        }

        public override void Draw(GUIGraphicEngine guiGraphicEngine)
        {
            Rectangle rect = new Rectangle(new Point(0, 0), Size);

            if (BackColor.A > 0)
                guiGraphicEngine.DrawRectangle(rect, BackColor);

            Rectangle rectTexto = new Rectangle(24, 0, Size.Width - 24, Size.Height);

            guiGraphicEngine.DrawText(rectTexto, text, TextColor);

            if (chk)
                guiGraphicEngine.DrawText(new Point(4, 0), "[X]");
            else
                guiGraphicEngine.DrawText(new Point(4, 0), "[  ]");

            if (Father.Focus == this)
                guiGraphicEngine.DrawRectangleFrame(rect, Color.White, 1);
        }
    }
}
