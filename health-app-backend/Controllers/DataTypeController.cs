using health_app_backend.Models;
using health_app_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace health_app_backend.Controllers;

[ApiController]
[Route("api/datatype")]
public class DataTypeController : ControllerBase
{
    private readonly IDataTypeService _dataTypeService;

    public DataTypeController(IDataTypeService dataTypeService)
    {
        _dataTypeService = dataTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DataType>>> GetDataTypes()
    {
        try
        {
            var result = await _dataTypeService.GetDataTypes();
            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}