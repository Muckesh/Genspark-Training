
using AutoMapper;

public class EmployeeService : IEmployeeService
{
    private readonly IRepository<int, Employee> _employeeRepository;
    private readonly IRepository<string, User> _userRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IMapper _mapper;

    public EmployeeService(IRepository<int, Employee> employeeRepository,
                            Repository<string, User> userRepository,
                            IEncryptionService encryptionService,
                            IMapper mapper
                            )
    {
        _employeeRepository = employeeRepository;
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _mapper = mapper;
    }

    public async Task<Employee> AddEmployee(EmployeeAddRequestDto employee)
    {
        try
        {
            var user = _mapper.Map<EmployeeAddRequestDto, User>(employee);
            var encryptedData = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = employee.Password
            });
            user.Password = encryptedData.EncryptedData;
            user.HashKey = encryptedData.HashKey;
            user.Role = "Staff";
            user = await _userRepository.Add(user);

            var newEmployee = _mapper.Map<EmployeeAddRequestDto,Employee>(employee);
            newEmployee = await _employeeRepository.Add(newEmployee);
            if (newEmployee == null)
            {
                throw new Exception("Could not add employee");
            }
            return newEmployee;
        }
        catch (Exception e)
        {

            throw new Exception(e.Message);
        }
    }

    // public Task<ICollection<Employee>> GetAllEmployees()
    // {
    //     throw new NotImplementedException();
    // }

    // public Task<Employee> GetEmployeeByName(string name)
    // {
    //     throw new NotImplementedException();
    // }
}