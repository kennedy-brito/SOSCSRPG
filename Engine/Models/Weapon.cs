﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models;

public class Weapon : GameItem
{
    public int MinimumDamage { get; }
    public int MaximumDamage { get; }
    public Weapon(int itemTypeId, string name, int price, int minimumDamage, int maximumDamage) 
        : base(itemTypeId, name, price, true) 
    { 
        MinimumDamage = minimumDamage;
        MaximumDamage = maximumDamage;
    }

    public new Weapon Clone() => new Weapon(ItemTypeId, Name, Price, MinimumDamage, MaximumDamage);
}
