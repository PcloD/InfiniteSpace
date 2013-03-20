using System;
using System.Collections.Generic;
using System.Text;
using EspacioInfinitoDotNet.Graphics;
using EspacioInfinitoDotNet.Maths;
using System.Drawing;
using EspacioInfinitoDotNet.GUI;

namespace EspacioInfinitoDotNet.GUI.Controls
{
    public class GUITextBox : GUIWindow
    {
        #region Atributos

        Color textColor = Color.White;
        Color backColor = Color.FromArgb(0, Color.Black);
        String text = "";
        bool multiLine = true;
        bool recalcularDibujado = true;

        GUIGraphicEngine.DrawTextInfo dtInfo;
        Size dtInfoSize;

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
            set { text = value; recalcularDibujado = true;  }
        }

        public bool MultiLine
        {
            get { return multiLine; }
            set { multiLine = value; recalcularDibujado = true; }
        }

        #endregion

        public GUITextBox(Size size)
            : base(size)
        {
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            return false;
        }

        public override void Draw(GUIGraphicEngine guiGraphicEngine)
        {
            if (BackColor.A > 0)
                guiGraphicEngine.DrawRectangle(new Rectangle(new Point(0, 0), Size), BackColor);

            if (multiLine)
            {
                if (recalcularDibujado || Size != dtInfoSize)
                {
                    dtInfo = guiGraphicEngine.CalculateDrawTextInfo(new Rectangle(new Point(0, 0), Size), text);
                    dtInfoSize = Size;
                    recalcularDibujado = false;
                }

                guiGraphicEngine.DrawText(dtInfo, TextColor);
            }
            else
            {
                guiGraphicEngine.DrawText(new Point(0, 0), Text, TextColor);
            }
        }
    }
}
