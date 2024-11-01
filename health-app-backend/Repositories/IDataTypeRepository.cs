using health_app_backend.Models;

namespace health_app_backend.Repositories;

public interface IDataTypeRepository : IRepository<DataType>
{
    Task<DataType> GetByIdAsync(int id);
}