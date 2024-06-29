namespace Engine.Models;

public class Monster : LivingEntity
{
    public string ImageName { get; set; }

    public int MinimumDamage { get; set; }
    public int MaximumDamage { get; set; }
    public int RewardExperiencePoints { get; set; }

    public Monster(
        string name, string imageName, 
        int maximumHitPoints, int hitPoints,
        int minimumDamage, int maximumDamage,
        int rewardExperiencePoints, int rewardGold)
    {

        CurrentHitPoints = hitPoints;
        Name = name;
        ImageName = $"pack://application:,,,/Engine;component/Images/Monsters/{imageName}";
        MaximumHitPoints = maximumHitPoints;
        RewardExperiencePoints = rewardExperiencePoints;
        Gold = rewardGold;
        MinimumDamage = minimumDamage;
        MaximumDamage = maximumDamage;
    }
}
