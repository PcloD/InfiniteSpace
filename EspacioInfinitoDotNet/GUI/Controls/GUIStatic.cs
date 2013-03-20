using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Maths;
using System.Drawing;
using EspacioInfinitoDotNet.GUI;

namespace EspacioInfinitoDotNet.GUI.Controls
{
    public class GUIStatic : GUIWindow
    {
        #region Atributos

        Color textColor = Color.White;
        Color backColor = Color.FromArgb(0, Color.Black);
        String text = "";
        bool textChanged = true;
        bool autoFit = true;
        bool centerHorizontally = false;
        bool centerVertically = false;
        Point drawPosition = new Point(0, 0);

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

        public String Text
        {
            get { return text; }
            set { text = value; textChanged = true; }
        }

        public bool AutoFit
        {
            get { return autoFit; }
            set { autoFit = value; }
        }

        public bool CenterHorizontally
        {
            get { return centerHorizontally; }
            set { centerHorizontally = value; }
        }

        public bool CenterVertically
        {
            get { return centerVertically; }
            set { centerVertically = value; }
        }

        #endregion

        public GUIStatic(Size size)
            : base(size)
        {
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            return false;
        }

        public override void Draw(GUIGraphicEngine guiGraphicEngine)
        {
            Rectangle rect = new Rectangle(new Point(0, 0), Size);

            if (textChanged)
            {
                textChanged = false;

                Size textSize = guiGraphicEngine.GetTextSizePixels(text);

                if (autoFit)
                {
                    Size = textSize;

                    rect = new Rectangle(new Point(0, 0), Size);
                }
                else
                {
                    if (centerHorizontally)
                        drawPosition.X = (Size.Width - textSize.Width) / 2;

                    if (centerVertically)
                        drawPosition.Y = Size.Height / 2 - textSize.Height;
                }
            }

            if (BackColor.A > 0)
                guiGraphicEngine.DrawRectangle(rect, BackColor);

            guiGraphicEngine.DrawText(drawPosition, Text, TextColor);
        }
    }
}
