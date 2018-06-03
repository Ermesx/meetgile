namespace IntranetCalendar.Infrastructure
{
    using System.Threading.Tasks;

    public interface IRepository<TAggregateRoot, TIdentity> where TAggregateRoot : class
    {
        Task<TAggregateRoot> GetAsync(TIdentity id);

        Task SaveAsync(TAggregateRoot entity);

        Task Delete(TAggregateRoot entity);
    }
}