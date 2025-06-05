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

    [Test]
        public async Task TestGetAllDoctors()
        {
        var doctors = new List<Doctor>

        {
            new Doctor { Id = 1, Name = "Doc 1" },
            new Doctor { Id = 2, Name = "Doc 2"}
        };
        var doctorRepositoryMock = new Mock<IRepository<int, Doctor>>();
        doctorRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(doctors);
        var service = new DoctorService(
            doctorRepositoryMock.Object,
            Mock.Of<IRepository<int, Speciality>>(),
            Mock.Of<IRepository<int, DoctorSpeciality>>(),
            Mock.Of<IRepository<string, User>>(),
            Mock.Of<IOtherContextFunctionalities>(),
            Mock.Of<IEncryptionService>(),
            Mock.Of<IMapper>()
        );
        var result = await service.GetAllDoctors();
        Assert.That(result.Count, Is.EqualTo(2));
    }  

    [Test]
    public void TestGetAllDoctorsException()
    {
        var doctorRepositoryMock = new Mock<IRepository<int, Doctor>>();
        doctorRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Doctor>());
        var service = new DoctorService(
            doctorRepositoryMock.Object,
            Mock.Of<IRepository<int, Speciality>>(),
            Mock.Of<IRepository<int, DoctorSpeciality>>(),
            Mock.Of<IRepository<string, User>>(),
            Mock.Of<IOtherContextFunctionalities>(),
            Mock.Of<IEncryptionService>(),
            Mock.Of<IMapper>()
        );
        Assert.ThrowsAsync<Exception>(async () => await service.GetAllDoctors());
    }

    [Test]
    public async Task TestGetDoctByName_ReturnsDoctors()
    {
        var doctors = new List<Doctor> { new Doctor { Id = 1, Name = "Tom" }, new Doctor { Id = 2, Name = "Tim" } };
        var doctorRepositoryMock = new Mock<IRepository<int, Doctor>>();
        doctorRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(doctors);
        var service = new DoctorService(
            doctorRepositoryMock.Object,
            Mock.Of<IRepository<int, Speciality>>(),
            Mock.Of<IRepository<int, DoctorSpeciality>>(),
            Mock.Of<IRepository<string, User>>(),
            Mock.Of<IOtherContextFunctionalities>(),
            Mock.Of<IEncryptionService>(),
            Mock.Of<IMapper>()
        );
        var result = await service.GetDoctorByName("Tom");
        Assert.That(result.Name, Is.EqualTo("Tom"));
    }



    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

}