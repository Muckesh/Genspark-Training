using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FirstApi.Test;

public class PatientServiceTest
{
    private ClinicContext _context;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase("TestDb")
                            .Options;
        _context = new ClinicContext(options);
    }

    [Test]
    public async Task AddPatient()
    {
        // Arrange
        var patientRepo = new Mock<PatientRepository>(_context);
        var userRepoMock = new Mock<UserRepository>(_context);
        var encryptionServiceMock = new Mock<EncryptionService>();
        var mapperMock = new Mock<IMapper>();

        var patientDto = new PatientAddRequestDto
        {
            Age = 25,
            Phone = "1234567890",
            Name = "test",
            Password = "test123"
        };

        var username = "test@gmail.com";
        var password = Encoding.UTF8.GetBytes("test123");

        var user = new User
        {
            UserName = username,
            Password = password,
            HashKey = Guid.NewGuid().ToByteArray(),
            Role = "Patient"
        };

        var patient = new Patient { Id = 1, Name = "Test" };
        var encryptedModel = new EncryptModel();


        mapperMock.Setup(m => m.Map<PatientAddRequestDto, User>(It.IsAny<PatientAddRequestDto>())).Returns(user);
        mapperMock.Setup(m => m.Map<PatientAddRequestDto, Patient>(It.IsAny<PatientAddRequestDto>())).Returns(patient);
        encryptionServiceMock.Setup(en => en.EncryptData(It.IsAny<EncryptModel>())).ReturnsAsync(encryptedModel);
        userRepoMock.Setup(us => us.Add(It.IsAny<User>())).ReturnsAsync(user);
        patientRepo.Setup(ad => ad.Add(It.IsAny<Patient>())).ReturnsAsync(patient);


        var service = new PatientService(
            patientRepo.Object,
            userRepoMock.Object,
            encryptionServiceMock.Object,
            mapperMock.Object
        );

        // Act
        var result = await service.AddPatient(patientDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("test"));

    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

}