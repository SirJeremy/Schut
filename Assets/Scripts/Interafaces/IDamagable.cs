public interface IDamagable
{
    void Damage(float value);
    void Kill();
    public float Health { get; }
}
