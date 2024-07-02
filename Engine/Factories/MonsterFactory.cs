using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Factories;

public static class MonsterFactory
{
    public static Monster GetMonster(int monsterID)
    {
        switch(monsterID)
        {
            case 1:
                Monster snake = new Monster("Snake", "Snake.png", 4, 4, 5, 1);

                snake.CurrentWeapon = ItemFactory.CreateGameItem(1501);

                AddLootItem(snake, 9001, 25);
                AddLootItem(snake, 9002, 75);
                
                return snake;
            
            case 2:
                Monster rat = new Monster("Rat", "Rat.png", 5, 5, 5, 1);

                rat.CurrentWeapon = ItemFactory.CreateGameItem(1502);

                AddLootItem(rat, 9001, 25);
                AddLootItem(rat, 9002, 75);
                
                return rat;            
            
            case 3:
                Monster giantSpider = new Monster("GiantSpider", "GiantSpider.png", 10, 10, 4, 1);

                giantSpider.CurrentWeapon = ItemFactory.CreateGameItem(1503);

                AddLootItem(giantSpider, 9001, 25);
                AddLootItem(giantSpider, 9002, 75);
                
                return giantSpider;
            default:
                throw new ArgumentException(string.Format("Monster Type '{0}' does not exist", monsterID));
        }
    }

    private static void AddLootItem(Monster monster, int itemId, int percentage)
    {
        if (RandomNumberGenerator.NumberBetween(1,100) <= percentage)
        {
            monster.AddItemToInventory(ItemFactory.CreateGameItem(itemId));
        }
    }
}
