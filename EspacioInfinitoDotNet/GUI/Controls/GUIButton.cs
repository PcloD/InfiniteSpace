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
    public class GUIButton : GUIWindow
    {
        #region Atributos

        Color textColor = Color.White;
        Color buttonColor = Color.Gray;
        Color buttonPressedColor = Color.Black;
        Color frameColor = Color.Black;
        String text = "";

        public Color ButtonColor
        {
            get { return buttonColor; }
            set { buttonColor = value; }
        }

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        public Color ButtonPressedColor
        {
            get { return buttonPressedColor; }
            set { buttonPressedColor = value; }
        }

        public Color FrameColor
        {
            get { return frameColor; }
            set { frameColor = value; }
        }

        public String Text
        {
            get { return text; }
            set { text = value; }
        }

        #endregion

        #region Eventos generados

        public delegate void ButtonPressedHandler(GUIButton button);
        public event ButtonPressedHandler ButtonPressed;

        #endregion

        public GUIButton(Size size) : base(size)
        {
            Focusable = true;
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            bool handled = false;
            bool pressed = false;

            if (guiEvent is GUIEventMouseButtonPressed)
            {
                pressed = true;
                handled = true;
            }
            else if (guiEvent is GUIEventMouseButtonReleased)
            {
                handled = true;
            }
            else if (guiEvent is GUIEventMouseMoved)
            {
                handled = true;

                GUIEventMouseMoved g = (GUIEventMouseMoved) guiEvent;

                if (pressed == true)
                {
                    if (g.position.X < 0 ||
                        g.position.Y < 0 ||
                        g.position.X >= Size.Width ||
                        g.position.Y >= Size.Height)
                    {
                        pressed = false;
                    }
                }
            }
            else if (guiEvent is GUIEventKeyPressed)
            {
                GUIEventKeyPressed guiEventKey = (GUIEventKeyPressed)guiEvent;

                if (guiEventKey.key == Sdl.SDLK_DOWN || guiEventKey.key == Sdl.SDLK_RIGHT)
                {
                    Father.FocusNextChild();
                    handled = true;
                }
                else if (guiEventKey.key == Sdl.SDLK_UP || guiEventKey.key == Sdl.SDLK_LEFT)
                {
                    Father.FocusPreviusChild();
                    handled = true;
                }
            }
            else if (guiEvent is GUIEventKeyReleased)
            {
                GUIEventKeyReleased guiEventKey = (GUIEventKeyReleased)guiEvent;

                if (guiEventKey.key == Sdl.SDLK_SPACE ||
                    guiEventKey.key == Sdl.SDLK_RETURN)
                {
                    pressed = true;
                    handled = true;
                }
            }

            if (pressed)
                if (ButtonPressed != null)
                    ButtonPressed(this);

            return handled;
        }

        public override void Draw(GUIGraphicEngine guiGraphicEngine)
        {
	        Rectangle rect = new Rectangle(new Point(0, 0), Size);

            guiGraphicEngine.DrawRectangle(rect, ButtonColor);

	        Size textSize = guiGraphicEngine.GetTextSizePixels(text);
	        Point textPosition = new Point(1, 1);

	        if (textSize.Width < Size.Width)
		        textPosition.X = (Size.Width - textSize.Width) / 2;

	        if (textSize.Height < Size.Height)
		        textPosition.Y = Size.Height / 2 - textSize.Height + 3;

	        guiGraphicEngine.DrawText(textPosition, Text, TextColor);

            guiGraphicEngine.DrawRectangleFrame(rect, FrameColor, 1);

            if (Father.Focus == this)
                guiGraphicEngine.DrawRectangleFrame(rect, Color.White, 1);
        }
    }
}
