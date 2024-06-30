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

    private Player _currentPlayer;
    private Location _currentLocation;

    private Monster _currentMonster;
    private Trader _currentTrader;

    public World CurrentWorld { get; }
    public Player CurrentPlayer { 
        get => _currentPlayer; 
        set
        {
            if (_currentPlayer is not null) 
            {
                _currentPlayer.OnActionPerformed -= OnCurrentPlayerPerformedAction;
                _currentPlayer.OnLeveledUp -= OnCurrentPlayerLeveledUp;
                _currentPlayer.OnKilled -= OnCurrentPlayerKilled;
            }

            _currentPlayer = value;

            if(_currentPlayer is not null)
            {
                _currentPlayer.OnActionPerformed += OnCurrentPlayerPerformedAction;
                _currentPlayer.OnLeveledUp += OnCurrentPlayerLeveledUp;
                _currentPlayer.OnKilled += OnCurrentPlayerKilled;
            }
        }
    }
    public Location CurrentLocation { 
        get { return _currentLocation; } 
        set
        {
            _currentLocation = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasLocationToNorth));
            OnPropertyChanged(nameof(HasLocationToSouth));
            OnPropertyChanged(nameof(HasLocationToWest));
            OnPropertyChanged(nameof(HasLocationToEast));

            CompleteQuestAtLocation();
            GivePlayerQuestsAtLocation();
            GetMonsterAtLocation();

            CurrentTrader = CurrentLocation.TraderHere;
        } 
    }

    public bool HasLocationToNorth  => 
        CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1 ) is not null;
        
    public bool HasLocationToSouth => 
        CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1 ) is not null;
        
    public bool HasLocationToWest => 
        CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate) is not null;
        
    public bool HasLocationToEast => 
        CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate) is not null;
        
    public bool HasMonster => _currentMonster is not null;

    public Monster CurrentMonster
    {
        get => _currentMonster;
        set
        {
            if (_currentMonster is not null) 
            {
                _currentMonster.OnKilled -= OnCurrentMonsterKilled;
            }
            _currentMonster = value;

            if (CurrentMonster is not null)
            {
                _currentMonster.OnKilled += OnCurrentMonsterKilled;
                RaiseMessage("");
                RaiseMessage($"You see a {CurrentMonster.Name} here!");
            }

            OnPropertyChanged(nameof(HasMonster));
            OnPropertyChanged();
        }
    }

    public Trader CurrentTrader { 
        get => _currentTrader;
        set 
        {
            _currentTrader = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(HasTrader));
        } 
    }

    #endregion
    private void GetMonsterAtLocation() => CurrentMonster = CurrentLocation.GetMonster();

    public bool HasTrader => CurrentTrader is not null;
    public GameSession()
    {
        CurrentPlayer = new Player("Scott", "Fighter", 0, 10, 10, 10000);

        if (CurrentPlayer.Weapons.Count is 0)
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
            if(!CurrentPlayer.Quests.Any(q => q.PlayerQuest.ID == quest.ID))
            {
                CurrentPlayer.Quests.Add(new QuestStatus(quest));
                RaiseMessage("");
                RaiseMessage($"You receive the '{quest.Name}' quest");
                
                RaiseMessage(quest.Description);
                
                RaiseMessage("Return with:");
                foreach (ItemQuantity itemQuantity in quest.ItemsToComplete)
                {
                    RaiseMessage($"   {itemQuantity.Quantity} {ItemFactory.CreateGameItem(itemQuantity.ItemID).Name}");
                }
                
                RaiseMessage("And you will receive:");
                RaiseMessage($"   {quest.RewardExperiencePoints} experience points");
                RaiseMessage($"   {quest.RewardGold} gold");
                
                foreach (ItemQuantity itemQuantity in quest.RewardItems)
                {
                    RaiseMessage($"   {itemQuantity.Quantity} {ItemFactory.CreateGameItem(itemQuantity.ItemID).Name}");
                }
            }
        }
    }


    public void AttackCurrentMonster()
    {
        if(CurrentPlayer.CurrentWeapon is null)
        {
            RaiseMessage("You must select a weapon, to attack.");
            return;
        }

        CurrentPlayer.UseCurrentWeaponOn(CurrentMonster);

        // if monster if killed, collect rewards and loot
        if (CurrentMonster.IsDead) 
        {
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
                RaiseMessage($"The {CurrentMonster.Name} hit you for {damageToPlayer} points");
                CurrentPlayer.TakeDamage(damageToPlayer);
            }

        }
    }

    private void CompleteQuestAtLocation()
    {
       foreach(Quest quest in CurrentLocation.QuestsAvailableHere) 
        {
            QuestStatus? questToComplete = 
                CurrentPlayer.Quests.FirstOrDefault(q=> q.PlayerQuest.ID == quest.ID && 
                                                        !q.IsCompleted);

            if(questToComplete is not null)
            {
                if(CurrentPlayer.HasAllTheseItems(quest.ItemsToComplete))
                {
                    //Remove the quest completion items from the player's inventory
                    foreach( ItemQuantity itemQuantity in quest.ItemsToComplete)
                    {
                        for(int i = 0; i < itemQuantity.Quantity; i++)
                        {
                            CurrentPlayer.RemoveItemFromInventory(CurrentPlayer.Inventory.
                                First(item => item.ItemTypeId == itemQuantity.ItemID));
                        }
                    }

                    RaiseMessage("");
                    RaiseMessage($"You completed the '{quest.Name}' quest");

                    //Give the player the quest rewards
                    CurrentPlayer.Quests.Add(new QuestStatus(quest));
                    RaiseMessage("");
                    RaiseMessage($"You receive the '{quest.Name}' quest");
                    
                    RaiseMessage(quest.Description);
                    RaiseMessage("Return with:");
                    
                    foreach (ItemQuantity itemQuantity in quest.ItemsToComplete)
                    {
                        RaiseMessage($"   {itemQuantity.Quantity} {ItemFactory.CreateGameItem(itemQuantity.ItemID).Name}");
                    }
                    
                    RaiseMessage("And you will receive:");
                    RaiseMessage($"   {quest.RewardExperiencePoints} experience points");
                    RaiseMessage($"   {quest.RewardGold} gold");

                    CurrentPlayer.ReceiveGold(quest.RewardGold);
                    CurrentPlayer.AddExperience(quest.RewardExperiencePoints);

                    foreach (ItemQuantity itemQuantity in quest.RewardItems)
                    {
                        GameItem rewardItem = ItemFactory.CreateGameItem(itemQuantity.ItemID);
                        CurrentPlayer.AddItemToInventory(rewardItem);
                        RaiseMessage($"You receive a {rewardItem.Name}");
                    }

                    // Mark the Quest as completed
                    questToComplete.IsCompleted = true;
                }
            }
          
        }
    }

    private void OnCurrentPlayerPerformedAction(object sender, string result)
    {
        RaiseMessage(result);
    }
    private void OnCurrentPlayerLeveledUp(object sender, System.EventArgs eventArgs)
    {
        RaiseMessage($"You are now level {CurrentPlayer.Level}");
    }
    private void OnCurrentPlayerKilled(object sender, System.EventArgs eventArgs)
    {

        // If player is killed, move them back to their home
            RaiseMessage("");
        RaiseMessage($"You have been killed.");

        CurrentLocation = CurrentWorld.LocationAt(0, -1); //Player's home
        CurrentPlayer.CompletelyHeal(); //Completely heal the player
    }

    private void OnCurrentMonsterKilled(object sender, System.EventArgs eventArgs)
    {
        
        RaiseMessage("");
        RaiseMessage($"You defeated the {CurrentMonster.Name}!");

        RaiseMessage($"You receive {CurrentMonster.RewardExperiencePoints} experience points.");
        CurrentPlayer.AddExperience(CurrentMonster.RewardExperiencePoints);

        RaiseMessage($"You receive {CurrentMonster.Gold} gold.");
        CurrentPlayer.ReceiveGold(CurrentMonster.Gold);

        foreach (GameItem gameItem in CurrentMonster.Inventory)
        {
            CurrentPlayer.AddItemToInventory(gameItem);
            RaiseMessage($"You receive one {gameItem.Name}.");

        }
    }
    private void RaiseMessage(string message)
    {
        OnMessageRaised?.Invoke(this, new GameEventMessageArgs(message));
    }
}
