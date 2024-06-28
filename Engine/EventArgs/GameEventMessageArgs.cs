using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.EventArgs;
public class GameEventMessageArgs : System.EventArgs
{
    public string Message { get; set; }

    public GameEventMessageArgs(string message)
    { this.Message = message; }

}
