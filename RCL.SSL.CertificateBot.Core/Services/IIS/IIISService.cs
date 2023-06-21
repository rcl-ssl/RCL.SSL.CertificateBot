using Microsoft.Web.Administration;
using System.Security.Cryptography.X509Certificates;

namespace RCL.SSL.CertificateBot.Core
{
    internal interface IIISService
    {
        Site GetIISSite(string siteName);

        void AddIISSiteBinding(string siteName,
            string bindingInformation,
            string certificateFilePath,
            string certificatePassword,
            StoreLocation storeLocation);

        void RemoveIISSiteBinding(string siteName, 
            string bindingInformation);
    }
}
