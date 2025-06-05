// using Microsoft.EntityFrameworkCore;

// public class DocumentRepository : Repository<Guid, Document>
// {
//     public DocumentRepository(NotifyDbContext context) : base(context)
//     {
//     }

//     public override async Task<Document> Get(Guid key)
//     {
//         return await _context.Documents
//             .Include(d => d.UploadedBy)
//             .FirstOrDefaultAsync(d => d.Id == key)
//             ?? throw new KeyNotFoundException("Document not found");
//     }

//     public override async Task<IEnumerable<Document>> GetAll()
//     {
//         return await _context.Documents
//             .Include(d => d.UploadedBy)
//             .OrderByDescending(d => d.UploadedAt)
//             .ToListAsync();
//     }

//     // Optional: Get all documents uploaded by a specific user
//     public async Task<IEnumerable<Document>> GetDocumentsByUser(string email)
//     {
//         return await _context.Documents
//             .Where(d => d.UploadedByEmail == email)
//             .OrderByDescending(d => d.UploadedAt)
//             .ToListAsync();
//     }
// }
