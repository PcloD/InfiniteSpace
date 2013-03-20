using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace EspacioInfinitoDotNet.GUI.Controls
{
    class GUIRoot : GUIWindow
    {
        public GUIRoot(Size size)
            : base(size)
        {
        }

        public override void Draw(GUIGraphicEngine guiGraphicEngine)
        {
        }

        public override bool HandleEvent(GUIEvent guiEvent)
        {
            return false;
        }
    }
}
