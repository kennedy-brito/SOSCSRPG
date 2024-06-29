﻿using System.Collections.ObjectModel;

namespace Engine.Models;
public abstract class LivingEntity : BaseNotificationClass
{
    private string _name;
    private int _currentHitPoints;
    private int _maximumHitPoints;
    private int _gold;

    public string Name 
    { 
        get => _name; 
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public int CurrentHitPoints
    {
        get => _currentHitPoints;
        set
        {
            _currentHitPoints = value;
            OnPropertyChanged(nameof(CurrentHitPoints));
        }
    }

    public int MaximumHitPoints
    {
        get => _maximumHitPoints;
        set
        {
            _maximumHitPoints = value;
            OnPropertyChanged(nameof(MaximumHitPoints));
        }
    }

    public int Gold
    {
        get => _gold;
        set
        {
            _gold = value;
            OnPropertyChanged(nameof(Gold));
        }
    }
    public ObservableCollection<GameItem> Inventory { get; set; }
    protected LivingEntity()
    {
        Inventory = new ObservableCollection<GameItem>();
    }
    public List<GameItem> Weapons =>
        Inventory.Where(i => i is Weapon).ToList();

    public void AddItemToInventory(GameItem item)
    {
        Inventory.Add(item);
        OnPropertyChanged(nameof(Weapons));

    }

    public void RemoveItemFromInventory(GameItem item) 
    {
        Inventory.Remove(item);
        OnPropertyChanged(nameof(Weapons));
    }

}
