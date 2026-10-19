namespace Entropa.Core
{
    public static class GameLayers
    {
        public const int Environment = 8;
        public const int Player = 9;
        public const int Enemy = 10;

        public const int EnvironmentMask = 1 << Environment;
        public const int PlayerMask = 1 << Player;
        public const int EnemyMask = 1 << Enemy;
        public const int CharacterMask = PlayerMask | EnemyMask;
        public const int CharacterCollisionMask = EnvironmentMask | PlayerMask | EnemyMask;
    }
}
