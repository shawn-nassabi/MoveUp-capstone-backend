using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace health_app_backend.Repositories;

public class DataTypeRepository : Repository<DataType>, IDataTypeRepository
{
    private readonly AppDbContext _context;
    public DataTypeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<DataType> GetByIdAsync(int id)
    {
        return await _context.DataTypes.FirstOrDefaultAsync(d => d.Id == id);
    }
    
}