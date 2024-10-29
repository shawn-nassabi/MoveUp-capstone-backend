using health_app_backend.Models;

namespace health_app_backend.Repositories;

public class DataTypeRepository : Repository<DataType>
{
    public DataTypeRepository(AppDbContext context) : base(context) { }
}