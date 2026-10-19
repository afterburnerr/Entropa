namespace Entropa.Combat
{
    public interface IDamageable
    {
        bool CanReceiveDamage { get; }

        void ReceiveDamage(in DamageInfo damage);
    }
}
