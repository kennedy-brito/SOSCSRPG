using System.Collections.Frozen;
using System.Collections.ObjectModel;

namespace Engine.Models;
public abstract class LivingEntity : BaseNotificationClass
{
    private string _name;
    private int _currentHitPoints;
    private int _maximumHitPoints;
    private int _gold;
    private int _level;

    #region Properties
    public string Name 
    { 
        get => _name; 
        private set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public int CurrentHitPoints
    {
        get => _currentHitPoints;
        private set
        {
            _currentHitPoints = value;
            OnPropertyChanged();
        }
    }

    public int MaximumHitPoints
    {
        get => _maximumHitPoints;
        protected set
        {
            _maximumHitPoints = value;
            OnPropertyChanged();
        }
    }

    public int Gold
    {
        get => _gold;
        private set
        {
            _gold = value;
            OnPropertyChanged();
        }
    }

    public int Level
    {
        get { return _level; }
        protected set
        {
            _level = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<GameItem> Inventory { get; }
    public ObservableCollection<GroupedInventoryItem> GroupedInventory { get; }
    public List<GameItem> Weapons =>
        Inventory.Where(i => i.Category is GameItem.ItemCategory.Weapon).ToList();

    public bool IsDead => CurrentHitPoints <= 0;

    #endregion

    public event EventHandler OnKilled;
    protected LivingEntity( string name, int maximumHitPoints, int currentHitPoints, int gold, int level = 1)
    {
        Name = name;
        MaximumHitPoints = maximumHitPoints;
        CurrentHitPoints = currentHitPoints;
        Gold = gold;
        Level = level;

        Inventory = new ObservableCollection<GameItem>();
        GroupedInventory = new ObservableCollection<GroupedInventoryItem>();
    }

    public void AddItemToInventory(GameItem item)
    {
        Inventory.Add(item);
        
        if(item.IsUnique) 
        {
            GroupedInventory.Add(new GroupedInventoryItem(item, 1));
        
        }
        else
        {
            if (! GroupedInventory.Any(gi => gi.Item.ItemTypeId == item.ItemTypeId)) 
            {
                GroupedInventory.Add(new GroupedInventoryItem(item, 0));
            }

            GroupedInventory.First(gi=> gi.Item.ItemTypeId == item.ItemTypeId).Quantity++;
        }
        OnPropertyChanged(nameof(Weapons));

    }

    public void RemoveItemFromInventory(GameItem item) 
    {
        Inventory.Remove(item);
            
        GroupedInventoryItem groupedInventoryItemToRemove = item.IsUnique ?
            GroupedInventory.FirstOrDefault(gi => gi.Item == item) :
            GroupedInventory.FirstOrDefault(gi => gi.Item.ItemTypeId == item.ItemTypeId);

        if(groupedInventoryItemToRemove is not null)
        {
            if(groupedInventoryItemToRemove.Quantity == 1)
            {
                GroupedInventory.Remove(groupedInventoryItemToRemove);
            }
            else
            {
                groupedInventoryItemToRemove.Quantity--;
            }
        }
        OnPropertyChanged(nameof(Weapons));
    }

    public void TakeDamage(int hitPointDamage)
    {
        CurrentHitPoints -= hitPointDamage;

        if (IsDead)
        {
            CurrentHitPoints = 0;
            RaiseOnKilledEvent();
        }

    }

    public void Heal(int hitPointHeal)
    {
        CurrentHitPoints += hitPointHeal;

        if (CurrentHitPoints > MaximumHitPoints)
        {
            CurrentHitPoints = MaximumHitPoints;
        }
    }

    public void CompletelyHeal() => CurrentHitPoints = MaximumHitPoints;

    public void ReceiveGold(int amountGold) => Gold += amountGold;

    public void SpendGold(int amountGold)
    {
        if(amountGold > Gold)
        {
            throw new ArgumentException($"{Name} only has {Gold} gold, and cannot spend {amountGold} gold");
        }

        Gold -= amountGold;
    }

    #region private functions

    private void RaiseOnKilledEvent() => OnKilled?.Invoke(this, new System.EventArgs());

    #endregion
}
