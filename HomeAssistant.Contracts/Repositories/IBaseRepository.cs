namespace HomeAssistant.Contracts.Repositories;

public interface IBaseRepository<T>
{
    Task<IEnumerable<T>> GetAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> AddAsync(T item);
    Task<T> UpdateAsync(T item);
    Task<bool> RemoveAsync(T item);
}
