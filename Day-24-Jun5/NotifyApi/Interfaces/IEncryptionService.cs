public interface IEncryptionService
{
    Task<EncryptModel> EncryptData(EncryptModel data);
}
