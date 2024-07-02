using Engine.Actions;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Factories;

public static class ItemFactory
{
    private static readonly List<GameItem> _standardGameItems = new();

    static ItemFactory()
    {
        BuildWeapon(1001, "Pointy Stick", 1, 1, 2);
        BuildWeapon(1002, "Rusty Sword", 5, 1, 3);
        BuildWeapon(1501, "Snake fang", 0,0, 2);
        BuildWeapon(1502, "Rat claws", 0, 0, 2);
        BuildWeapon(1503, "Spider fangs", 0, 0, 1);

        BuildMiscellanousItem(9001, "Snake fang", 1);
        BuildMiscellanousItem(9002, "Snakeskin", 2);
        BuildMiscellanousItem(9003, "Rat tail", 1);
        BuildMiscellanousItem(9004, "Rat fur", 2);
        BuildMiscellanousItem(9005, "Spider fang", 1);
        BuildMiscellanousItem(9006, "Spider silk", 2);
    }

    private static void BuildWeapon(int id, string name, int price,
                                    int minimumDamage, int maximumDamage)
    {
        GameItem weapon = new GameItem(GameItem.ItemCategory.Weapon, id, name, price, true);

        weapon.Action = new AttackWithWeapon(weapon, minimumDamage, maximumDamage);
        _standardGameItems.Add(weapon);
    }

    private static void BuildMiscellanousItem(int id, string name, int price) 
    {
        _standardGameItems.Add(new GameItem(GameItem.ItemCategory.Miscellaneous, id, name, price)); 
        
    }

    public static GameItem? CreateGameItem(int itemTypeId) =>  _standardGameItems.FirstOrDefault(item => item.ItemTypeId == itemTypeId)?.Clone();
    
}
