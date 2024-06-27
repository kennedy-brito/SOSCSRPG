using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models;

internal class Monster : BaseNotificationClass
{
    private int _hitPoints;
    
    public string Name { get; set; }
    public string ImageName { get; set; }
    public int MaximumHitPoints { get; set; }
    public int HitPoints
    {
        get { return _hitPoints; }
        set 
        { 
            _hitPoints = value;
            OnPropertyChanged(nameof(HitPoints));

        }
    }

    public int RewardExperiencePoints { get; set; }
    public int RewardGold {  get; set; }

    public ObservableCollection<ItemQuantity> Inventory{ get; set; }

    public Monster(
        string name, string imageName, 
        int maximumHitPoints, int hitPoints,
        int rewardExperiencePoints, int rewardGold)
    {
        HitPoints = hitPoints;
        Name = name;
        ImageName = $"pack://application:,,,/Engine/Images/Monsters/{imageName}";
        MaximumHitPoints = maximumHitPoints;
        RewardExperiencePoints = rewardExperiencePoints;
        RewardGold = rewardGold;

        Inventory = new ObservableCollection<ItemQuantity>();
    }
}
