namespace TDS_Coordinator_Application.TaskCoordinator.DB.Repositories
{
    // Generic methods used in all repositories
    public interface IRepository<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void DeleteByGuid(string guid);
    }
}
