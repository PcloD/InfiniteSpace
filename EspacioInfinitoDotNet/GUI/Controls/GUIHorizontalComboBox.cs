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
    public class GUIHorizontalComboBox : GUIWindow
    {
        #region Atributos

        Color textColor = Color.White;
        Color backColor = Color.FromArgb(0, Color.Black);
        object[] items;
        int selectedIndex = 0;

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }

        public object[] Items
        {
            get { return items; }
        }

        public object SelectedItem
        {
            get 
            {
                if (selectedIndex >= 0 && selectedIndex < items.Length)
                    return items[selectedIndex];
                else
                    return null;
            }

            set 
            { 
                for (int i = 0; i < items.Length; i++)
                    if (items[i].Equals(value))
                    {
                        selectedIndex = i;
                        break;
                    }
            }
        }

        #endregion

        public GUIHorizontalComboBox(Size size, object[] items)
            : base(size)
        {
            Focusable = true;
            this.items = items;
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            bool handled = false;

            if (guiEvent is GUIEventKeyPressed)
            {
                GUIEventKeyPressed guiEventKey = (GUIEventKeyPressed)guiEvent;

                if (guiEventKey.key == Sdl.SDLK_LEFT)
                {
                    selectedIndex--;

                    if (selectedIndex < 0)
                        selectedIndex = items.Length - 1;

                    handled = true;
                }
                else if (guiEventKey.key == Sdl.SDLK_RIGHT)
                {
                    selectedIndex++;

                    if (selectedIndex >= items.Length)
                        selectedIndex = 0;

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

            Rectangle rectTexto = new Rectangle(14, 0, Size.Width - 20, Size.Height);

            string text = "";

            if (SelectedItem != null)
                text = SelectedItem.ToString();

            guiGraphicEngine.DrawText(rectTexto, text, TextColor);

            guiGraphicEngine.DrawText(new Point(4, 0), "<");
            guiGraphicEngine.DrawText(new Point(Size.Width - 10, 0), ">");

            if (Father.Focus == this)
                guiGraphicEngine.DrawRectangleFrame(rect, Color.White, 1);
        }
    }
}
