public interface IEncryptionService
{
    public Task<EncryptModel> EncryptData(EncryptModel data);
}