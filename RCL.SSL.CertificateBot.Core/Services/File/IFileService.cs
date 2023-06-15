namespace RCL.SSL.CertificateBot.Core
{
    internal interface IFileService
    {
       public void CreateDirectory(string path);
       public int GetNumberFilesInDirectory(string path);
       public void SaveFile(string fileName, string path, Stream contentStream);
       public void WriteTextToFile(string fileName, string path, string text);
    }
}
