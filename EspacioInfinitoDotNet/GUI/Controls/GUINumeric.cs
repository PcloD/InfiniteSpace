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
    public class GUINumeric : GUIWindow
    {
        #region Atributos

        Color textColor = Color.White;
        Color backColor = Color.FromArgb(0, Color.Black);
        float value;
        float minValue = int.MinValue;
        float maxValue = int.MaxValue;
        float stepValue = 1.0f;

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

        public float Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public float MaxValue
        {
            get { return this.maxValue; }
            set { this.maxValue = value; }
        }

        public float MinValue
        {
            get { return this.minValue; }
            set { this.minValue = value; }
        }

        public float StepValue
        {
            get { return this.stepValue; }
            set { this.stepValue = value; }
        }

        #endregion

        public GUINumeric(Size size)
            : base(size)
        {
            Focusable = true;
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            bool handled = false;

            if (guiEvent is GUIEventKeyPressed)
            {
                GUIEventKeyPressed guiEventKey = (GUIEventKeyPressed)guiEvent;

                if (guiEventKey.key == Sdl.SDLK_LEFT || guiEventKey.key == Sdl.SDLK_MINUS)
                {
                    value -= stepValue;

                    value = (float) (Math.Round(value * 1000.0f) / 1000.0f);

                    if (value < minValue)
                        value = minValue;

                    handled = true;
                }
                else if (guiEventKey.key == Sdl.SDLK_RIGHT || guiEventKey.key == Sdl.SDLK_PLUS)
                {
                    value += stepValue;

                    value = (float)(Math.Round(value * 1000.0f) / 1000.0f);

                    if (value > maxValue)
                        value = maxValue;

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

            guiGraphicEngine.DrawText(rectTexto, value.ToString(), TextColor);

            guiGraphicEngine.DrawText(new Point(4, 0), "<");
            guiGraphicEngine.DrawText(new Point(Size.Width - 10, 0), ">");

            if (Father.Focus == this)
                guiGraphicEngine.DrawRectangleFrame(rect, Color.White, 1);
        }
    }
}
