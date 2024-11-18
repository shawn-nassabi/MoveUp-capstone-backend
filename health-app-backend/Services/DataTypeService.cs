using AutoMapper;
using health_app_backend.Models;
using health_app_backend.Repositories;

namespace health_app_backend.Services;

public class DataTypeService : IDataTypeService
{
    private readonly IDataTypeRepository _dataTypeRepository;
    private readonly IMapper _mapper;

    public DataTypeService(IDataTypeRepository dataTypeRepository, IMapper mapper)
    {
        _dataTypeRepository = dataTypeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DataType>> GetDataTypes()
    {
        return await _dataTypeRepository.GetAllAsync();
    }
}