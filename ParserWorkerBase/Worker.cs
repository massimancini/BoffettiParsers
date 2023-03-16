using log4net;
using log4net.Config;
using System.Globalization;

namespace ParserWorkerBase
{
    public class Worker : BackgroundService
    {
        protected readonly ILogger<Worker> _logger;
        protected static readonly ILog log2File = LogManager.GetLogger(typeof(Worker));
        protected FileSystemWatcher watcher;
        protected string rootFolder;
        protected string importFolder;
        protected string parsedFolder;
        protected string dateTimeFormat;
        protected string lastFileParsedName;
        protected string opcUaUser;
        protected string opcUaPwd;
        protected string endpointUrl;
        protected string opcUaConfigFile;
        protected string tagCodiceArticolo;
        protected string tagOrderID;
        protected string tagQuantita;
        protected int namespaceIndex;
        //Session session;
        protected ClientOPCUA.clientOPCUA client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            rootFolder = (string)System.Configuration.ConfigurationManager.AppSettings["RootFolder"];
            importFolder = (string)System.Configuration.ConfigurationManager.AppSettings["ImportFolder"];
            dateTimeFormat = (string)System.Configuration.ConfigurationManager.AppSettings["DateTimeFormat"];
            parsedFolder = (string)System.Configuration.ConfigurationManager.AppSettings["ParsedFolder"];
            lastFileParsedName = (string)System.Configuration.ConfigurationManager.AppSettings["lastFileParsedName"];
            endpointUrl = (string)System.Configuration.ConfigurationManager.AppSettings["EndpointUrl"];
            opcUaUser = (string)System.Configuration.ConfigurationManager.AppSettings["OpcUaUser"];
            opcUaPwd = (string)System.Configuration.ConfigurationManager.AppSettings["OpcUaPassword"];
            opcUaConfigFile = (string)System.Configuration.ConfigurationManager.AppSettings["OpcUaConfigFile"];
            tagCodiceArticolo = (string)System.Configuration.ConfigurationManager.AppSettings["TagCodiceArticolo"];
            tagOrderID = (string)System.Configuration.ConfigurationManager.AppSettings["TagOrderID"];
            tagQuantita = (string)System.Configuration.ConfigurationManager.AppSettings["TagQuantita"];
            namespaceIndex = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["NamespaceIndex"]);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {

            XmlConfigurator.Configure();
            _logger.LogInformation("Service startup.");
            log2File.Info("Service startup");

            opcUaConfigFile = Path.Combine(rootFolder, opcUaConfigFile);
            client = new ClientOPCUA.clientOPCUA(opcUaConfigFile, endpointUrl);

            client.createSession(opcUaUser, opcUaPwd);

            importFolder = Path.Combine(rootFolder, importFolder);
            log2File.Info("importFolder: " + importFolder);
            
            watcher = new FileSystemWatcher();
            watcher.Path = importFolder;

            // Watch for all changes specified in the NotifyFilters  
            //enumeration.  
            watcher.NotifyFilter = NotifyFilters.Attributes |
            NotifyFilters.CreationTime |
            NotifyFilters.DirectoryName |
            NotifyFilters.FileName |
            NotifyFilters.LastAccess |
            NotifyFilters.LastWrite |
            NotifyFilters.Security |
            NotifyFilters.Size;

            watcher.Filter = "*.csv";
            // Add event handlers.  



            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;

            return base.StartAsync(cancellationToken);
            //await base.StartAsync(cancellationToken);
        }

        protected void MoveParsedFile(string pathfilename)
        {
            string filename = Path.GetFileName(pathfilename);
            log2File.Info("move " + pathfilename);
            parsedFolder = Path.Combine(rootFolder, parsedFolder);
            File.Move(pathfilename, Path.Combine(parsedFolder, filename));

            //string fullPath = Path.Combine(parsedFolder, lastFileParsedName);
            //using (StreamWriter writer = new StreamWriter(fullPath))
            //{
            //    writer.WriteLine(filename);
            //}
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Service");

            //session.Close();
            //session.Dispose();
            client.close();
            watcher.EnableRaisingEvents = false;
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing Service");
            if (watcher != null)
                watcher.Dispose();
            base.Dispose();
        }

        // Define the event handlers.  
        public virtual async void OnChanged(object source, FileSystemEventArgs e)
        {
            

        }

        protected DateTime? GetDateTime(string strDateTime)
        {
            string[] validformats = new[] { dateTimeFormat };

            CultureInfo provider = CultureInfo.InvariantCulture;
            string date = strDateTime;
            DateTime dateTime;

            if (!DateTime.TryParseExact(date, validformats, provider,
                                        DateTimeStyles.None, out dateTime))
            {
                log2File.Info("Unable to parse the  dateTime");
            }

            return dateTime;
        }
    }
}