using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }
    [HttpPost("Register")]
    public async Task<ActionResult<Employee>> RegisterEmployee(EmployeeAddRequestDto employee)
    {
        try
        {
            var result = await _employeeService.AddEmployee(employee);
            return result;
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }
    }
}