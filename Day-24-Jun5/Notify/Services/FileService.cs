// using Microsoft.AspNetCore.Hosting;
// using Microsoft.EntityFrameworkCore;

// public class FileService : IFileService
// {
//     private readonly IWebHostEnvironment _env;
//     private readonly IRepository<Guid, Document> _documentRepository;

//     public FileService(IWebHostEnvironment env, IRepository<Guid, Document> documentRepository)
//     {
//         _env = env;
//         _documentRepository = documentRepository;
//     }

//     public async Task<Document> SaveFile(IFormFile file, string uploadedByEmail)
//     {
//         if (file == null || file.Length == 0)
//             throw new ArgumentException("Invalid file");

//         var uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads");

//         if (!Directory.Exists(uploadsFolder))
//             Directory.CreateDirectory(uploadsFolder);

//         var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
//         var fullPath = Path.Combine(uploadsFolder, uniqueFileName);
//         var relativePath = Path.Combine("Uploads", uniqueFileName);

//         using (var stream = new FileStream(fullPath, FileMode.Create))
//         {
//             await file.CopyToAsync(stream);
//         }

//         var document = new Document
//         {
//             Id = Guid.NewGuid(),
//             FileName = file.FileName,
//             FilePath = relativePath,
//             UploadedByEmail = uploadedByEmail,
//             UploadedAt = DateTime.UtcNow
//         };

//         await _documentRepository.Add(document);
//         return document;
//     }

//     public async Task<Document?> GetFile(Guid id)
//     {
//         try
//         {
//             return await _documentRepository.Get(id);
//         }
//         catch
//         {
//             return null;
//         }
//     }
// }
