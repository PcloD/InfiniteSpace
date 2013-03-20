using System;
using System.Collections.Generic;
using System.Text;

namespace EspacioInfinitoDotNet.Recursos
{
    interface ITieneRecursos
    {
        Recurso[] GetRecursos();
        void RenovarRecursos(float fDeltaTime);
    }
}
