using System;
using System.Collections.Generic;
using System.Text;
using Tao.Sdl;

namespace EspacioInfinitoDotNet.GUI
{
    public enum MouseButtonEnum
    {
        MouseButton1, 
        MouseButton2, 
        MouseButton3
    }

    public enum MouseButtonMask 
    { 
        MouseButton1Mask = 1, 
        MouseButton2Mask = 2, 
        MouseButton3Mask = 4 
    }

    public class GUIEvent
    {
    }

    public class GUIEventMouseButtonPressed : GUIEvent
    {
        public System.Drawing.Point position;
        public MouseButtonEnum button;
    }

    public class GUIEventMouseButtonReleased : GUIEvent
	{
        public System.Drawing.Point position;
        public MouseButtonEnum button;
    }

    public class GUIEventMouseMoved : GUIEvent
	{
        public System.Drawing.Point position;
        public int buttonMask;
	}

    public class GUIEventKeyPressed : GUIEvent
	{
        public int key;
	}

    public class GUIEventKeyReleased : GUIEvent
	{
        public int key;
    }

    public class GUIEventFocusLost : GUIEvent
    {
    }

    public class GUIEventFocusGain : GUIEvent
    {
    }
}
