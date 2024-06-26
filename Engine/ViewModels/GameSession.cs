﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Factories;
using System.ComponentModel;
namespace Engine.ViewModels;

public class GameSession : BaseNotificationClass
{
    private Location _currentLocation;
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
        } 
    }

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

        CurrentWorld = WorldFactory.CreateWorld();

        CurrentLocation = CurrentWorld.LocationAt(0, 0);
    }

    public void MoveNorth()
    {
        CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1);
    }
    public void MoveWest()
    {
        CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate);
    }
    public void MoveSouth()
    {
        CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1);

    }
    public void MoveEast()
    {
        CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate);

    }

}
