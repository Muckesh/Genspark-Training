using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FirstApi.Test;

public class DoctorServiceTest
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

    [TestCase("Cardiology")]
    public async Task TestGetDoctorBySpeciality(string speciality)
    {
        Mock<DoctorRepository> doctorRepositoryMock = new Mock<DoctorRepository>(_context);
        Mock<SpecialityRepository> specialityRepositoryMock = new(_context);
        Mock<DoctorSpecialityRepository> doctorSpecialityRepositoryMock = new(_context);
        Mock<UserRepository> userRepositoryMock = new(_context);
        Mock<OtherFunctionalityImplementation> otherFunctionalitiesImplementationMock = new(_context);
        Mock<EncryptionService> encryptionServiceMock = new();
        Mock<IMapper> mapperMock = new();

        otherFunctionalitiesImplementationMock.Setup(ocf => ocf.GetDoctorsBySpeciality(It.IsAny<string>()))
                                                .ReturnsAsync((string speciality) => new List<DoctorsBySpecialityResponseDto>
                                                {
                                                    new DoctorsBySpecialityResponseDto{
                                                        Id = 1,
                                                        DoctorName = "test",
                                                        Yoe = 2
                                                    }
                                                });

        IDoctorService doctorService = new DoctorService(doctorRepositoryMock.Object,
                                                            specialityRepositoryMock.Object,
                                                            doctorSpecialityRepositoryMock.Object,
                                                            userRepositoryMock.Object,
                                                            otherFunctionalitiesImplementationMock.Object,
                                                            encryptionServiceMock.Object,
                                                            mapperMock.Object);

        // Assert.That(doctorService,Is.Not.Null);
        // Action
        var result = await doctorService.GetDoctorsBySpeciality(speciality);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));   
    }   

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

}