﻿using System.Collections.ObjectModel;

namespace Engine.Models;

public class Player : LivingEntity
{
    private string _characterClass;
    private int _experiencePoints;

    #region Properties
  
    public string CharacterClass
    {
        get { return _characterClass; }
        set
        {
            _characterClass = value;
            OnPropertyChanged();
        }
    }
    

    public int ExperiencePoints
    {
        get { return _experiencePoints; }
        private set
        {
            _experiencePoints = value;
            OnPropertyChanged();

            SetLevelAndMaximumHitPoints();
        }
    }
   

    public ObservableCollection<QuestStatus> Quests { get; }

    #endregion
    public event EventHandler OnLeveledUp;
    public Player(string name, string characterClass, int experiencePoints,
                  int maximumHitPoints, int currentHitPoints, int gold) : 
        base(name, maximumHitPoints, currentHitPoints, gold)
    {
        CharacterClass = characterClass;
        ExperiencePoints = experiencePoints;
        Quests = new ObservableCollection<QuestStatus>();
    }

    public bool HasAllTheseItems(List<ItemQuantity> items)
    {
        foreach(ItemQuantity item in items)
        {
            if( Inventory.Count(i => i.ItemTypeId == item.ItemID) < item.Quantity)
            {
                return false;
            }
        }

        return true;
    }

    public void AddExperience(int experiencePoints) => ExperiencePoints += experiencePoints;

    private void SetLevelAndMaximumHitPoints()
    {
        int originalLevel = Level;

        Level = (ExperiencePoints / 100) + 1;

        if(Level != originalLevel)
        {
            MaximumHitPoints = Level * 10;

            OnLeveledUp?.Invoke(this, System.EventArgs.Empty);
        }
    }

}
