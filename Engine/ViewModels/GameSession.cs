using System;
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
}
