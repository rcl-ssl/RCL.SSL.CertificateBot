namespace RCL.SSL.CertificateBot.Core.Helpers
{
    internal static class IISBindingsHelper
    {
        public static List<IISBindingInformation> GetIISBindings(List<string> IISBindingsList)
        {
            List<IISBindingInformation> bindings = new List<IISBindingInformation>();

            try
            {
                if (IISBindingsList != null)
                {
                    if (IISBindingsList?.Count > 0)
                    {
                        foreach (string binding in IISBindingsList)
                        {
                            IISBindingInformation bindingInformation = new IISBindingInformation();

                            string[] arr = binding.Split(';');

                            if (arr.Length > 0)
                            {
                                foreach (string str in arr)
                                {
                                    if (str.Contains("siteName"))
                                    {
                                        bindingInformation.siteName = str.Split(':')[1];
                                    }
                                    if (str.Contains("ip"))
                                    {
                                        bindingInformation.ip = str.Split(':')[1];
                                    }
                                    if (str.Contains("port"))
                                    {
                                        bindingInformation.port = str.Split(':')[1];
                                    }
                                    if (str.Contains("host"))
                                    {
                                        bindingInformation.host = str.Split(':')[1];
                                    }
                                    if (str.Contains("certificateName"))
                                    {
                                        bindingInformation.certificateName = str.Split(':')[1];
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(bindingInformation?.siteName) &&
                               !string.IsNullOrEmpty(bindingInformation?.ip) &&
                               !string.IsNullOrEmpty(bindingInformation?.port) &&
                               !string.IsNullOrEmpty(bindingInformation?.host) &&
                               !string.IsNullOrEmpty(bindingInformation?.certificateName))
                            {
                                bindings.Add(bindingInformation);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return bindings;
        }
    }
}
