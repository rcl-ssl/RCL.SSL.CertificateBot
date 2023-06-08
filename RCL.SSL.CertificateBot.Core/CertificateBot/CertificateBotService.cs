﻿#nullable disable

using Microsoft.Extensions.Options;
using RCL.SSL.SDK;
using System.Security.Cryptography.X509Certificates;

namespace RCL.SSL.CertificateBot.Core
{
    internal class CertificateBotService : ICertificateBotService
    {
        private readonly IOptions<CertificateBotOptions> _options;
        private readonly ICertificateService _certificateService;
        private readonly IFileService _fileService;
        private readonly IIISService _iIISService;

        private static readonly HttpClient _httpClient;

        private const string _pfxCertificateFileName = "certificate.pfx";
        private const string _privateKeyFileName = "privateKey.key";
        private const string _primaryCertificateFileName = "primaryCertificate.crt";
        private const string _caBundleFileName = "caBundle.crt";
        private const string _fullChainCertificateFileName = "fullChainCertificate.crt";

        static CertificateBotService()
        {
            _httpClient = new HttpClient();
        }

        public CertificateBotService(IOptions<CertificateBotOptions> options,
            ICertificateService certificateService,
            IFileService fileService,
            IIISService iIISService)
        {
            _options = options;
            _certificateService = certificateService;
            _fileService = fileService;
            _iIISService = iIISService;
        }

        public async Task<string> InstallAndRenewCertificateAsync()
        {
            string message = $"Message received at : {DateTime.Now}. ";

            try
            {
                List<string> certificateNames = _options.Value.IncludeCertificates;

                if (certificateNames?.Count > 0)
                {
                    List<Certificate> certificates = await GetIncludedCertificatesAsync(certificateNames);

                    if (certificates?.Count > 0)
                    {
                        message = $"Found {certificates.Count} certificate(s) to process locally. ";

                        foreach (Certificate cert in certificates)
                        {
                            bool b = await SaveCertificateAsync(cert);

                            if (b == true)
                            {
                                message = $"{message} Successfully saved : {cert.certificateName} in local machine. ";
                            }
                            else
                            {
                                message = $"{message} {cert.certificateName} is up-to-date on local machine. ";
                            }
                        }
                    }
                    else
                    {
                        message = $"{message} Did not find any certificates to process locally. ";
                    }

                    List<Certificate> certificatesToRenew = await GetCertificatesToRenewAsync(_options.Value.IncludeCertificates);

                    if (certificatesToRenew?.Count > 0)
                    {
                        message = $"{message} Found {certificatesToRenew?.Count} certificate(s) to renew. ";

                        foreach (Certificate cert in certificatesToRenew)
                        {
                            await _certificateService.RenewCertificateAsync(cert);

                            message = $"{message} Scheduled {cert.certificateName} for renewal. ";
                        }
                    }
                    else
                    {
                        message = $"{message} Did not find any certificates to renew. ";
                    }
                }
                else
                {
                    message = $"{message} Did not find any certificates to include in local machine. ";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{message} {ex.Message}");
            }

            return message;
        }

        public async Task<string> InstallAndRenewCertificateInIISAsync()
        {
            string message = $"Message received at : {DateTime.Now}. ";

            try
            {
                List<IISBindingInformation> bindings = _options.Value.IISBindings;
                List<string> certificateNamesSaved = new List<string>();
                List<Certificate> certificatesSaved = new List<Certificate>();
                List<string> certificatesProcessed = new List<string>();

                if (bindings?.Count > 0)
                {
                    foreach (var binding in bindings)
                    {
                        Certificate certificate = new Certificate
                        {
                            certificateName = binding.certificateName
                        };

                        if (!certificatesProcessed.Contains(certificate.certificateName))
                        {
                            Certificate certRetrieved = await _certificateService.GetCertificateAsync(certificate);

                            if (!string.IsNullOrEmpty(certRetrieved?.certificateName))
                            {
                                bool b = await SaveCertificateAsync(certRetrieved);

                                if (b == true)
                                {
                                    message = $"{message} Successfully saved : {certRetrieved.certificateName} in local machine. ";
                                    certificateNamesSaved.Add(certRetrieved.certificateName);
                                    certificatesSaved.Add(certRetrieved);
                                }
                                else
                                {
                                    message = $"{message} {certRetrieved.certificateName} is up-to-date on local machine. ";
                                }
                            }

                            certificatesProcessed.Add(certificate.certificateName);
                        }
                    }

                    foreach(var binding in bindings)
                    {
                        if(certificateNamesSaved.Contains(binding.certificateName))
                        {
                           RemoveIISBinding(binding);

                            Certificate certificate = certificatesSaved.Where(w => w.certificateName == binding.certificateName).FirstOrDefault();

                            string folderPath = FolderNameHelper.GetFolderPath(certificate.certificateName, _options.Value.SaveCertificatePath);

                            _iIISService.AddIISSiteBinding(binding.siteName, binding.GetBindingInformation(), $"{folderPath}/{_pfxCertificateFileName}", certificate.password, StoreLocation.LocalMachine);

                            message = $"{message} Successfully bound certificate : {certificate.certificateName} to IIS site: {binding.siteName} for hostname: {binding.host}. ";
                        }
                    }
                }
                else
                {
                    message = $"{message} Did not find any bindngs for IIS. ";
                }

                List<Certificate> certificatesToRenew = await GetCertificatesToRenewAsync(certificatesProcessed);

                if (certificatesToRenew?.Count > 0)
                {
                    message = $"{message} Found {certificatesToRenew?.Count} certificate(s) to renew. ";

                    foreach (Certificate cert in certificatesToRenew)
                    {
                        await _certificateService.RenewCertificateAsync(cert);

                        message = $"{message} Scheduled {cert.certificateName} for renewal. ";
                    }
                }
                else
                {
                    message = $"{message} Did not find any certificates to renew. ";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{message} {ex.Message}");
            }

            return message;
        }

        private async Task<List<Certificate>> GetIncludedCertificatesAsync(List<string> certificateNames)
        {
            List<Certificate> certificates = new List<Certificate>();

            try
            {
                if (certificateNames?.Count > 0)
                {
                    foreach (string certificateName in certificateNames)
                    {
                        Certificate certificateToRetrieve = new Certificate
                        {
                            certificateName = certificateName
                        };

                        Certificate certificateRetrieved = await _certificateService.GetCertificateAsync(certificateToRetrieve);

                        if (!string.IsNullOrEmpty(certificateRetrieved?.certificateName ?? string.Empty))
                        {
                            if (certificateRetrieved.renewal.ToLower() == "automatic")
                            {
                                certificates.Add(certificateRetrieved);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not get Included Certificates. {ex.Message}");
            }

            return certificates;
        }

        private async Task<bool> SaveCertificateAsync(Certificate certificate)
        {
            bool b = false;

            try
            {
                string folderPath = FolderNameHelper.GetFolderPath(certificate.certificateName, _options.Value.SaveCertificatePath);

                int files = _fileService.GetNumberFilesInDirectory(folderPath);
                int age = (DateTime.Now.Date - certificate.issueDate.Date).Days;

                if (files < 1 || age < 9)
                {
                    await SaveFileAsync(_pfxCertificateFileName, folderPath, certificate.certificateDownloadUrl.pfxUrl);
                    await SaveFileAsync(_privateKeyFileName, folderPath, certificate.certificateDownloadUrl.keyUrl);
                    await SaveFileAsync(_primaryCertificateFileName, folderPath, certificate.certificateDownloadUrl.certCrtUrl);
                    await SaveFileAsync(_caBundleFileName, folderPath, certificate.certificateDownloadUrl.cabundleCrtUrl);
                    await SaveFileAsync(_fullChainCertificateFileName, folderPath, certificate.certificateDownloadUrl.fullchainCrtUrl);

                    b = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return b;
        }

        private async Task SaveFileAsync(string fileName, string folderPath, string fileUri)
        {
            try
            {
                var response = await _httpClient.GetAsync(fileUri);

                using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                {
                    _fileService.SaveFile(fileName, folderPath, contentStream);
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Could not save file locally, {ex.Message}");
            }
        }

        private async Task<List<Certificate>> GetCertificatesToRenewAsync(List<string> certNames)
        {
            List<Certificate> certificates = new List<Certificate>();

            try
            {
                List<Certificate> certsToRenew = await _certificateService.GetCertificatesToRenewAsync();

                if (certsToRenew?.Count > 0)
                {
                    if (certNames?.Count > 0)
                    {
                        foreach (Certificate cert in certsToRenew)
                        {
                            if (certNames.Contains(cert.certificateName))
                            {
                                certificates.Add(cert);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return certificates;
        }

        private void RemoveIISBinding(IISBindingInformation binding)
        {
            try
            {
                _iIISService.RemoveIISSiteBinding(binding.siteName, binding.GetBindingInformation());
            }
            catch (Exception) { }
        }
    }
}
