using health_app_backend.Models;

namespace health_app_backend.Services;

public interface IDataTypeService
{
    Task<IEnumerable<DataType>> GetDataTypes();
}