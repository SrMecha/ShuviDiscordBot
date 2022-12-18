﻿namespace Shuvi.Classes.Characteristics
{
    public static class CharacteristicSettings
    {
        private const int _statNeedToUpPoint = 10;
        private const int _healthPerUpPoint = 4;
        private const int _manaPerUpPoint = 4;

        public static int StatNeedToUpPoint => _statNeedToUpPoint;
        public static int HealthPerUpPoint => _healthPerUpPoint;
        public static int ManaPerUpPoint => _manaPerUpPoint;
    }
}
