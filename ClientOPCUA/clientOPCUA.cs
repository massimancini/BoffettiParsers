using Opc.Ua.Client;
using Opc.Ua.Configuration;
using Opc.Ua;
using Microsoft.Extensions.Configuration;
using Opc.Ua.Server;
using Session = Opc.Ua.Client.Session;
using Microsoft.Extensions.Logging;
using log4net;
using log4net.Repository.Hierarchy;
using log4net.Config;
using log4net.Core;

namespace ClientOPCUA
{
    public class clientOPCUA
    {
        
        private static readonly ILog log2File = LogManager.GetLogger(typeof(clientOPCUA));
        ApplicationConfiguration m_configuration;
        ConfiguredEndpoint endpoint;
        string _opcUaUser = null;
        string _opcUaPwd = null;
        Session session;
        public clientOPCUA(string opcUaConfigFilePath, string endpointUrl)
        {
            try 
            { 
                XmlConfigurator.Configure();
                
                log2File.Info("clientOPCUA Inizio");
                // Generate a client application
                ApplicationInstance application = new ApplicationInstance();
                application.ApplicationType = ApplicationType.Client;

                // Load the configuration file
                //application.LoadApplicationConfiguration(@"./ConsoleReferenceClient.Config.xml", false).Wait();
                //application.LoadApplicationConfiguration(opcUaConfigFile, false).Wait();

                application.LoadApplicationConfiguration(opcUaConfigFilePath, false).Wait();

                m_configuration = application.ApplicationConfiguration;

                // check the application certificate.
                bool certOK = application.CheckApplicationInstanceCertificate(false, 0).Result;
                if (!certOK)
                {
                    throw new Exception("Application instance certificate invalid!");
                }

                m_configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates = true;
                //m_configuration.CertificateValidator = new CertificateValidator();
                m_configuration.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = true; };

                // Connect to a server

                // Get the endpoint by connecting to server's discovery endpoint.
                // Try to find the first endopint without security.
                //EndpointDescription endpointDescription = CoreClientUtils.SelectEndpoint("opc.tcp://atsitnb005:26543/OPCUAServer", false);
                EndpointDescription endpointDescription = CoreClientUtils.SelectEndpoint(endpointUrl, false);

                EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(m_configuration);
                endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
            }
            catch (Exception ex)
            {
                log2File.Info(ex.Message);
                log2File.Info(ex.StackTrace);
                

            }
            

        }

        public  bool createSession(string opcUaUser, string opcUaPwd)
        {
            try
            {
                _opcUaUser = opcUaUser;
                _opcUaPwd = opcUaPwd;
                UserIdentity user = new UserIdentity(opcUaUser, opcUaPwd);
                // Create the session
                session = Session.Create(
                    m_configuration,
                endpoint,
                false,
                    false,
                m_configuration.ApplicationName,
                    (uint)m_configuration.ClientConfiguration.DefaultSessionTimeout,
                    user,
                    null).Result;
            }
            catch (Exception ex)
            {
                log2File.Info(ex.Message);
                log2File.Info(ex.StackTrace);
                return false;

            }
            return true;
        }

        public void close()
        {
            log2File.Info("close");
            session.Close();
            session.Dispose();
        }
        public bool write2OPCUAServer(List<Information> infos, UInt16 nameSpaceIndex)
        {
            try
            {
                WriteValueCollection nodesToWrite = new WriteValueCollection();
                
                foreach (Information info in infos)
                {
                    NodeId nodeId = new NodeId(info.tag, nameSpaceIndex);
                    WriteValue wv = new WriteValue();
                    //log2File.Info("Info type " + info.infoValue.GetType().ToString());
                    log2File.Info(info.tag + " type: " + info.type.ToString());
                    switch (info.type)
                    //switch (info.infoValue.GetType())
                    {
                        case Type int16Type when int16Type == typeof(Int16):
                            //log2File.Info("Int16: " + info.infoValue.ToString());
                            wv = writeShort(nodeId, (Int16) info.infoValue);
                            break;
                        case Type StringType when StringType == typeof(String):
                            //log2File.Info("String: " + info.infoValue.ToString());
                            wv = writeString(nodeId, (string)info.infoValue);
                            break;
                        case Type DoubleType when DoubleType == typeof(Double):
                            //log2File.Info("Double: " + info.infoValue.ToString());
                            wv = writeDouble(nodeId, (double)info.infoValue);
                            break;
                        case Type WordType when WordType == typeof(UInt16):
                            wv = writeWord(nodeId, (UInt16)info.infoValue);
                            break;
                        case Type LongType when LongType == typeof(Int32):
                            wv = writeLong(nodeId, (Int32)info.infoValue);
                            break;
                        case Type DWordType when DWordType == typeof(UInt32):
                            wv = writeDWord(nodeId, (UInt32)info.infoValue);
                            break;
                    }
                    nodesToWrite.Add(wv);

                }
               

                StatusCodeCollection results = null;
                DiagnosticInfoCollection diagnosticInfos;
                if(!(session != null && session.Connected))
                {
                    if (_opcUaUser != null && _opcUaPwd != null)
                        createSession(_opcUaUser, _opcUaPwd);
                }
                log2File.Info("session: " + session.SessionName + " connected: " + session.Connected.ToString());
                // Call Write Service
                session.Write(null,
                              nodesToWrite,
                              out results,
                              out diagnosticInfos);
                foreach (StatusCode res in results)
                    log2File.Info("write result code " + res.Code);
                
            }
            catch (Exception ex)
            {
                log2File.Info(ex.Message);
                log2File.Info(ex.StackTrace);
                close();
                return false;

            }
            return true;

        }

        private static WriteValue writeShort(NodeId nodeId, Int16 intValue)
        {
            WriteValue intWriteVal = new WriteValue();
            intWriteVal.NodeId = nodeId;
            intWriteVal.AttributeId = Attributes.Value;
            intWriteVal.Value = new DataValue();
            intWriteVal.Value.Value = (Int16)intValue;
            return intWriteVal;
        }
        private static WriteValue writeString(NodeId nodeId, string strValue)
        {
            WriteValue strWriteVal = new WriteValue();
            strWriteVal.NodeId = nodeId;
            strWriteVal.AttributeId = Attributes.Value;
            strWriteVal.Value = new DataValue();
            strWriteVal.Value.Value = (string)strValue;
            return strWriteVal;
        }

        private static WriteValue writeDouble(NodeId nodeId, double dValue)
        {
            WriteValue dWriteVal = new WriteValue();
            dWriteVal.NodeId = nodeId;
            dWriteVal.AttributeId = Attributes.Value;
            dWriteVal.Value = new DataValue();
            dWriteVal.Value.Value = (double)dValue;
            return dWriteVal;
        }
        private static WriteValue writeWord(NodeId nodeId, UInt16 intValue)
        {
            WriteValue intWriteVal = new WriteValue();
            intWriteVal.NodeId = nodeId;
            intWriteVal.AttributeId = Attributes.Value;
            intWriteVal.Value = new DataValue();
            intWriteVal.Value.Value = (UInt16)intValue;
            return intWriteVal;
        }
        private static WriteValue writeLong(NodeId nodeId, Int32 intValue)
        {
            WriteValue intWriteVal = new WriteValue();
            intWriteVal.NodeId = nodeId;
            intWriteVal.AttributeId = Attributes.Value;
            intWriteVal.Value = new DataValue();
            intWriteVal.Value.Value = (Int32)intValue;
            return intWriteVal;
        }
        private static WriteValue writeDWord(NodeId nodeId, UInt32 intValue)
        {
            WriteValue intWriteVal = new WriteValue();
            intWriteVal.NodeId = nodeId;
            intWriteVal.AttributeId = Attributes.Value;
            intWriteVal.Value = new DataValue();
            intWriteVal.Value.Value = (UInt32)intValue;
            return intWriteVal;
        }
    }
}