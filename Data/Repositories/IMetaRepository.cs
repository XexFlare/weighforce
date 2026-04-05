namespace WeighForce.Data.Repositories
{
    public interface IMetaRepository
    {
        bool DeleteMeta(string Name);
        Meta GetMeta(string Name);
        Meta SetMeta(Meta meta);
    }
}