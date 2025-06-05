using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class DocumentService : IDocumentService
{
    private readonly NotifyDbContext _context;
    private readonly IRepository<int, Employee> _employeeRepository;
    private readonly IHubContext<NotificationHub> _hub;

    public DocumentService(NotifyDbContext context, IRepository<int, Employee> employeeRepository, IHubContext<NotificationHub> hub)
    {
        _context = context;
        _employeeRepository = employeeRepository;
        _hub = hub;
    }
    public async Task<DocumentResponseDto> GetDocumentsAsync(int id)
    {
        try
        {
            var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);

            if (doc == null)
                throw new FileNotFoundException("File is not found in the Database.");

            var content = Encoding.UTF8.GetString(doc.FileContent);

            return new DocumentResponseDto
            {
                FileContent = content,
                Id = doc.Id
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<string> UploadDocumentAsync(DocumentUploadRequestDto file, string uploader)
    {
        try
        {
            var employees = await _employeeRepository.GetAll();

            var employee = employees.FirstOrDefault(e => e.Email == uploader);
            if (employee == null)
                throw new Exception("No such user found.");
            var fileData = file.File;
            var doc = new Document
            {
                FileName = fileData.FileName,
                UploadedById = employee.Id,
            };

            using (var stream = new MemoryStream())
            {
                fileData.CopyTo(stream);
                doc.FileContent = stream.ToArray();
            }

            var result = await _context.AddAsync(doc);
            await _context.SaveChangesAsync();

            var message = "New file has been uploaded.";
            var uploadedBy = employee.Name;

            await _hub.Clients.Group("Staffs").SendAsync("ReceiveMessage", message, uploadedBy);

            return "File uploaded.";
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
