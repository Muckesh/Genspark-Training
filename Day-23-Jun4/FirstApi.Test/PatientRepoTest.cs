using System.Text;
using Microsoft.EntityFrameworkCore;

namespace FirstApi.Test;

public class PatientRepoTest
{
    private ClinicContext _context;
    IRepository<int, Patient> _patientRepository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase("TestPatientDb")
                            .Options;
        _context = new ClinicContext(options);
        _patientRepository = new PatientRepository(_context);
    }
    
    [Test]
    public async Task AddPatientTest()
    {
        // Arrange
        var email = "test2@gmail.com";
        var password = Encoding.UTF8.GetBytes("test2123");
        var key = Guid.NewGuid().ToByteArray();
        var user = new User
        {
            UserName = email,
            Password = password,
            HashKey = key,
            Role = "Patient"
        };
        _context.Add(user);
        await _context.SaveChangesAsync();

        var patient = new Patient
        {
            Name = "test2",
            Email = email,
            Age = 31,
            Phone = "1234567890"
            
        };
        // Action
        var result = await _patientRepository.Add(patient);

        // Assert
        Assert.That(result, Is.Not.Null, "Patient is not added.");
        Assert.That(result.Id, Is.EqualTo(1));
        // Assert.That(result.Id, Is.EqualTo(2));

    }

    [TestCase(1)]
    //[TestCase(2)]
    public async Task TestGetPatientById(int id)
    {
        var result = await _patientRepository.Get(id);

        Assert.That(result, Is.Not.Null, $"No Patient with id: {id}");
    }

    [TestCase(2)]
    public async Task TestGetPatientByIdException(int id)
    {
        var ex = Assert.ThrowsAsync<Exception>(() => _patientRepository.Get(id));
        Assert.That(ex.Message, Is.EqualTo("No patient with the given ID."));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}