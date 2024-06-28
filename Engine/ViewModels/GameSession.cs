﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Factories;
using System.ComponentModel;
using Engine.EventArgs;
namespace Engine.ViewModels;

public class GameSession : BaseNotificationClass
{
    public event EventHandler<GameEventMessageArgs> OnMessageRaised;

    #region Properties 
    private Location _currentLocation;

    private Monster _currentMonster;
    public World CurrentWorld { get; set; }
    public Player CurrentPlayer { get; set; }
    public Location CurrentLocation { 
        get { return _currentLocation; } 
        set
        {
            _currentLocation = value;
            OnPropertyChanged(nameof(CurrentLocation));
            OnPropertyChanged(nameof(HasLocationToNorth));
            OnPropertyChanged(nameof(HasLocationToSouth));
            OnPropertyChanged(nameof(HasLocationToWest));
            OnPropertyChanged(nameof(HasLocationToEast));

            GivePlayerQuestsAtLocation();
            GetMonsterAtLocation();
        } 
    }
    public Weapon CurrentWeapon { get; set; }

    public bool HasLocationToNorth 
    { 
        get
        {
            return CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1 ) is not null;
        }
    }
    public bool HasLocationToSouth
    { 
        get
        {
            return CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1 ) is not null;
        }
    }
    public bool HasLocationToWest
    { 
        get
        {
            return CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate) is not null;
        }
    }
    public bool HasLocationToEast
    { 
        get
        {
            return CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate) is not null;
        }
    }
    public bool HasMonster => _currentMonster is not null;

    public Monster CurrentMonster
    {
        get => _currentMonster;
        set
        {
            _currentMonster = value;

            OnPropertyChanged(nameof(CurrentMonster));
            OnPropertyChanged(nameof(HasMonster));

            if (CurrentMonster is not null)
            {
                RaiseMessage("");
                RaiseMessage($"You see a {CurrentMonster.Name} here!");
            }
        }
    }
    #endregion
    private void GetMonsterAtLocation() => CurrentMonster = CurrentLocation.GetMonster();

    public GameSession()
    {
        CurrentPlayer = new Player
            {
                Name = "Scott",
                CharacterClass = "Fighter",
                Gold = 1000,
                HitPoints = 10,
                ExperiencePoints = 0,
                Level = 1
            };

        if( CurrentPlayer.Weapons.Count is 0)
        {
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(1001));
        }

        CurrentWorld = WorldFactory.CreateWorld();

        CurrentLocation = CurrentWorld.LocationAt(0, 0);
    }

    public void MoveNorth()
    {
        if(HasLocationToNorth)
        {

            CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1);

        }
    }
    public void MoveWest()
    {
        if(HasLocationToWest)
        {
            CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate);
        }
    }
    public void MoveSouth()
    {
        if(HasLocationToSouth)
        {
        
            CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1);

        }

    }
    public void MoveEast()
    {
        if(HasLocationToEast)
        {
        
            CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate);

        }

    }

    private void GivePlayerQuestsAtLocation()
    {
        foreach(Quest quest in CurrentLocation.QuestsAvailableHere)
        {
            if(! CurrentPlayer.Quests.Any(q => q.PlayerQuest.ID == quest.ID))
            {
                CurrentPlayer.Quests.Add(new QuestStatus(quest));
            }
        }
    }

    private void RaiseMessage(string message)
    {
        OnMessageRaised?.Invoke(this, new GameEventMessageArgs(message));
    }

    public void AttackCurrentMonster()
    {
        if(CurrentWeapon is null)
        {
            RaiseMessage("You must select a weapon, to attack.");
            return;
        }

        // determine damage to monster
        int damageToMonster = RandomNumberGenerator.NumberBetween(CurrentWeapon.MinimumDamage, CurrentWeapon.MaximumDamage);

        if(damageToMonster is 0) 
        {
            RaiseMessage($"You missed the {CurrentMonster.Name}");
        }
        else
        {
            CurrentMonster.HitPoints -= damageToMonster;
            RaiseMessage($"You hit the {CurrentMonster.Name} for {damageToMonster} points");
        }

        // if monster if killed, collect rewards and loot
        if (CurrentMonster.HitPoints <= 0) 
        {
            RaiseMessage("");
            RaiseMessage($"You defeated the {CurrentMonster.Name}!");

            CurrentPlayer.ExperiencePoints += CurrentMonster.RewardExperiencePoints;
            RaiseMessage($"You receive {CurrentMonster.RewardExperiencePoints} experience points.");

            CurrentPlayer.Gold += CurrentMonster.RewardGold;
            RaiseMessage($"You receive {CurrentMonster.RewardGold} gold.");

            foreach(ItemQuantity itemQuantity in CurrentMonster.Inventory)
            {
                GameItem item = ItemFactory.CreateGameItem(itemQuantity.ItemID);
                CurrentPlayer.AddItemToInventory(item);
                RaiseMessage($"You receive {itemQuantity.Quantity} {item.Name}.");

            }

            // get another monster to fight
            GetMonsterAtLocation();
        }
        else
        {
            // If monster is alive, let the monster attack
            int damageToPlayer = RandomNumberGenerator.NumberBetween(CurrentMonster.MinimumDamage, CurrentMonster.MaximumDamage);
        
            if (damageToPlayer is 0) 
            {
                RaiseMessage("The monster attacks, but misses you");
            }
            else
            {
                CurrentPlayer.HitPoints -= damageToPlayer;
                RaiseMessage($"The {CurrentMonster.Name} hit you for {damageToPlayer} points");
            }

            // If player is killed, move them back to their home
            if (CurrentPlayer.HitPoints <= 0) 
            {
                RaiseMessage("");
                RaiseMessage($"The {CurrentMonster.Name} killed you;");

                CurrentLocation = CurrentWorld.LocationAt(0, -1); //Player's home
                CurrentPlayer.HitPoints = CurrentPlayer.Level * 10; //Completely heal the player
            }
        }
    }
}
