namespace Shuvi.Classes.User
{
    public static class UserSettings
    {
        private const int _ratingPerUpdgradePoint = 10;
        private const int _healthPointRegenTime = 5;
        private const int _energyPointRegenTime = 180;
        private const int _manaPointRegenTime = 10;
        private const int _endurancePerEnergy = 10;
        private const int _standartEnergy = 10;
        private const int _standartMana = 10;
        private const int _standartHealth = 100;
        private const int _manaDisplayMax = 5;
        private const int _energyDisplayMax = 5;
        private const int _healthDisplayMax = 5;
        private const int _healthMax = 100;

        public static int RatingPerUpdgradePoint => _ratingPerUpdgradePoint;
        public static int HealthPointRegenTime => _healthPointRegenTime;
        public static int EnergyPointRegenTime => _energyPointRegenTime;
        public static int ManaPointRegenTime => _manaPointRegenTime;
        public static int ManaDisplayMax => _manaDisplayMax;
        public static int EndurancePerEnergy => _endurancePerEnergy;
        public static int StandartEnergy => _standartEnergy;
        public static int StandartMana => _standartMana;
        public static int StandartHealth => _standartHealth;
        public static int HealthDisplayMax => _healthDisplayMax;
        public static int EnergyDisplayMax => _energyDisplayMax;
        public static int HealthMax => _healthMax;
    }
}
