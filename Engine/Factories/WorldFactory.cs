using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
namespace Engine.Factories;

internal class WorldFactory
{

    internal World CreateWorld()
    {   
        var newWorld = new World();

        newWorld.AddLocation(
            0,
            -1,
            "Home",
            "This is your home",
            "pack://application:,,,/Engine;component/Images/Locations/Home.png");
        return newWorld;
    }
}
