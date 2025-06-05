using System.Text;
using Microsoft.EntityFrameworkCore;

namespace FirstApi.Test;

public class Tests
{
    private ClinicContext _context;
    IRepository<int, Doctor> _doctorRepository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase("TestDb")
                            .Options;
        _context = new ClinicContext(options);
        _doctorRepository = new DoctorRepository(_context);
    }

    [Test]
    public async Task AddDoctorTest()
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
            Role = "Doctor"
        };
        _context.Add(user);
        await _context.SaveChangesAsync();

        var doctor = new Doctor
        {
            Name = "test2",
            Email = email,
            YearsOfExperience = 3
        };
        // Action
        var result = await _doctorRepository.Add(doctor);

        // Assert
        Assert.That(result, Is.Not.Null, "Doctor is not added.");
        Assert.That(result.Id, Is.EqualTo(1));
        // Assert.That(result.Id, Is.EqualTo(2));

    }

    [TestCase(1)]
    public async Task GetDoctorPassTest(int id)
    {
        // Arrange
        // var email = "test@gmail.com";
        // var password = Encoding.UTF8.GetBytes("test123");
        // var key = Guid.NewGuid().ToByteArray();

        // var user = new User
        // {
        //     UserName = email,
        //     Password = password,
        //     HashKey = key,
        //     Role = "Doctor"
        // };

        // _context.Add(user);
        // await _context.SaveChangesAsync();

        // var doctor = new Doctor
        // {
        //     Name = "test",
        //     YearsOfExperience = 2,
        //     Email = email
        // };
        // IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);

        // await _doctorRepository.Add(doctor);

        // Action
        var result = _doctorRepository.Get(id);

        // Assert
        Assert.That(result, Is.Not.Null, "Doctor not found.");
        Assert.That(result.Id, Is.EqualTo(id));
    }

    [TestCase(2)]
    public async Task GetDoctorExceptionTest(int id)
    {
        // Arrange
        // var email = "test@gmail.com";
        // var password = Encoding.UTF8.GetBytes("test123");
        // var key = Guid.NewGuid().ToByteArray();

        // var user = new User
        // {
        //     UserName = email,
        //     Password = password,
        //     HashKey = key,
        //     Role = "Doctor"
        // };

        // _context.Add(user);
        // await _context.SaveChangesAsync();

        // var doctor = new Doctor
        // {
        //     Name = "test",
        //     YearsOfExperience = 2,
        //     Email = email
        // };
        // IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);

        // await _doctorRepository.Add(doctor);

        // Action
        var ex = Assert.ThrowsAsync<Exception>(() => _doctorRepository.Get(id));

        // Assert
        Assert.That(ex.Message, Is.EqualTo("Doctor not found with the given ID."));
    }

    [Test]
    public async Task GetAllDoctorsTest()
    {
        // Action
        var doctors = await _doctorRepository.GetAll();

        // Assert
        Assert.That(doctors, Is.Not.Null);

    }

    [Test]
    public async Task GetAllDoctorsExceptionTest()
    {
        await _doctorRepository.Delete(1);
        var ex = Assert.ThrowsAsync<Exception>(() => _doctorRepository.GetAll());
        Assert.That(ex.Message, Is.EqualTo("No doctors in the database."));
    }
    

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}