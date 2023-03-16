using log4net;
using log4net.Config;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System;
using ParserWorkerBase;
//using Opc.Ua.Client;
//using Opc.Ua.Configuration;
//using Opc.Ua;
//using Session = Opc.Ua.Client.Session;
//using ClientOPCUA.clientOPCUA;

namespace TiesseRobot
{
    public class TiesseWorker : Worker
    {
        private string tagStatoOrdine;
        private string tagDataOraInizio;
        private string tagDataOraFine;
        //private readonly ILogger<TiesseWorker> _logger;
        //private static readonly ILog log2File = LogManager.GetLogger(typeof(TiesseWorker));
        //private FileSystemWatcher watcher;
        //private string rootFolder;
        //private string importFolder;
        //private string parsedFolder;
        //private string dateTimeFormat;
        //private string lastFileParsedName;
        //private string opcUaUser;
        //private string opcUaPwd;
        //private string endpointUrl;
        //private string opcUaConfigFile;
        //private string tagCodiceArticolo;
        //private string tagOrderID;
        //private string tagQuantita;
        //private int namespaceIndex;
        ////Session session;
        //ClientOPCUA.clientOPCUA client;
        //public Worker(ILogger<Worker> logger)
        //{
        //    _logger = logger;
        //    rootFolder = (string)System.Configuration.ConfigurationManager.AppSettings["RootFolder"];
        //    importFolder = (string)System.Configuration.ConfigurationManager.AppSettings["ImportFolder"];
        //    dateTimeFormat = (string)System.Configuration.ConfigurationManager.AppSettings["DateTimeFormat"];
        //    parsedFolder = (string)System.Configuration.ConfigurationManager.AppSettings["ParsedFolder"];
        //    lastFileParsedName = (string)System.Configuration.ConfigurationManager.AppSettings["lastFileParsedName"];
        //    endpointUrl = (string)System.Configuration.ConfigurationManager.AppSettings["EndpointUrl"];
        //    opcUaUser = (string)System.Configuration.ConfigurationManager.AppSettings["OpcUaUser"];
        //    opcUaPwd = (string)System.Configuration.ConfigurationManager.AppSettings["OpcUaPassword"];
        //    opcUaConfigFile = (string)System.Configuration.ConfigurationManager.AppSettings["OpcUaConfigFile"];
        //    tagCodiceArticolo = (string)System.Configuration.ConfigurationManager.AppSettings["TagCodiceArticolo"];
        //    tagOrderID = (string)System.Configuration.ConfigurationManager.AppSettings["TagOrderID"];
        //    tagQuantita = (string)System.Configuration.ConfigurationManager.AppSettings["TagQuantita"];
        //    namespaceIndex = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["NamespaceIndex"]);

        //}

        public TiesseWorker(ILogger<TiesseWorker> logger) : base(logger)
        {
            tagStatoOrdine = (string)System.Configuration.ConfigurationManager.AppSettings["TagStatoOrdine"];
            tagDataOraInizio = (string)System.Configuration.ConfigurationManager.AppSettings["TagDataOraInizio"];
            tagDataOraFine = (string)System.Configuration.ConfigurationManager.AppSettings["TagDataOraFine"];

        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {

            //XmlConfigurator.Configure();
            //_logger.LogInformation("Service startup.");
            //log2File.Info("Service startup");



            //opcUaConfigFile = Path.Combine(rootFolder, opcUaConfigFile);
            //client = new ClientOPCUA.clientOPCUA(opcUaConfigFile, endpointUrl);



            //client.createSession(opcUaUser, opcUaPwd);

            Task t = base.StartAsync(cancellationToken);
            log2File.Info("dopo di base.StartAsync.");
            importFolder = Path.Combine(rootFolder, importFolder);
            log2File.Info("importFolder: " + importFolder);
            String[] filesPresent = Directory.GetFiles(importFolder);
            foreach (String pathfilename in filesPresent)
            {
                OutTiesse outInfo = readCsv(pathfilename);

                MoveParsedFile(pathfilename);
                //writeInfo(outInfo);
            }

            return t;

            //importFolder = Path.Combine(rootFolder, importFolder);
            //log2File.Info("importFolder: " + importFolder);
            //String[] filesPresent = Directory.GetFiles(importFolder);
            //foreach (String pathfilename in filesPresent)
            //{
            //    OutTiesse outInfo = readCsv(pathfilename);

            //    MoveParsedFile(pathfilename);
            //    writeInfo(outInfo);
            //}

            //watcher = new FileSystemWatcher();
            //watcher.Path = importFolder;

            //// Watch for all changes specified in the NotifyFilters  
            ////enumeration.  
            //watcher.NotifyFilter = NotifyFilters.Attributes |
            //NotifyFilters.CreationTime |
            //NotifyFilters.DirectoryName |
            //NotifyFilters.FileName |
            //NotifyFilters.LastAccess |
            //NotifyFilters.LastWrite |
            //NotifyFilters.Security |
            //NotifyFilters.Size;

            //watcher.Filter = "*.csv";
            //// Add event handlers.  



            //watcher.Created += new FileSystemEventHandler(OnChanged);
            //watcher.EnableRaisingEvents = true;

            //return base.StartAsync(cancellationToken);
            ////await base.StartAsync(cancellationToken);
        }

        //private void MoveParsedFile(string pathfilename)
        //{
        //    string filename = Path.GetFileName(pathfilename);
        //    log2File.Info("move " + pathfilename);
        //    parsedFolder = Path.Combine(rootFolder, parsedFolder);
        //    File.Move(pathfilename, Path.Combine(parsedFolder, filename));

        //    //string fullPath = Path.Combine(parsedFolder, lastFileParsedName);
        //    //using (StreamWriter writer = new StreamWriter(fullPath))
        //    //{
        //    //    writer.WriteLine(filename);
        //    //}
        //}

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //        await Task.Delay(1000, stoppingToken);
        //    }
        //}
        //public override async Task StopAsync(CancellationToken cancellationToken)
        //{
        //    _logger.LogInformation("Stopping Service");

        //    //session.Close();
        //    //session.Dispose();
        //    client.close();
        //    watcher.EnableRaisingEvents = false;
        //    await base.StopAsync(cancellationToken);
        //}

        //public override void Dispose()
        //{
        //    _logger.LogInformation("Disposing Service");
        //    if(watcher != null)
        //        watcher.Dispose();
        //    base.Dispose();
        //}

        // Define the event handlers.  
        public override async void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                log2File.Info("Onchanged: " + e.FullPath);
                _logger.LogInformation($"Onchanged Triggered by [{e.FullPath}]");
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    string pathfilename = e.FullPath;
                    _logger.LogInformation($"InBound Change Event Triggered by [{pathfilename}]");
                    log2File.Info("Start parsing " + e.FullPath);
                    OutTiesse outInfo = await readExcelAsync(pathfilename);

                    MoveParsedFile(pathfilename);
                    writeInfo(outInfo);

                    //string filename = Path.GetFileNameWithoutExtension(pathfilename);
                    //log2File.Info("holeinfo num elements: " + holeInfoList.Count);
                    //if (holeInfoList != null)
                    //{
                    //    importInInspect(filename, holeInfoList);
                    //}
                    //log2File.Info("move " + pathfilename);
                    //string outputFile = Path.Combine(outputFolder, Path.GetFileName(pathfilename));
                    //if (File.Exists(outputFile))
                    //    File.Delete(outputFile);
                    //File.Move(pathfilename, outputFile);
                    //_logger.LogInformation("Done with Inbound Change Event");
                }
            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex.Message);
                log2File.Info(ex.Message);

            }

        }
        private async Task<OutTiesse> readExcelAsync(string fileName)
        {
            OutTiesse outList = await Task.Run(() => { return readCsv(fileName); });
            return outList;
        }

        private bool writeInfo(OutTiesse tiesseInfo)
        {
            try
            {
                //WriteValueCollection nodesToWrite = new WriteValueCollection();
                //log2File.Info("writeInfo.");
                //NodeId nodeId = new NodeId(tagCodiceArticolo, 2);
                //WriteValue wvCodiceArticolo = writeString(nodeId, info.codiceArticolo);
                //nodesToWrite.Add(wvCodiceArticolo);
                //log2File.Info("writeInfo.");
                //nodeId = new NodeId(tagQuantita, 2);
                //WriteValue wvquantita = writeShort(nodeId, info.quantita);
                //nodesToWrite.Add(wvquantita);

                //nodeId = new NodeId(tagOrderID, 2);
                //WriteValue wvOrderID = writeString(nodeId, info.ordineID);
                //nodesToWrite.Add(wvOrderID);

                //StatusCodeCollection results = null;
                //DiagnosticInfoCollection diagnosticInfos;

                //// Call Write Service
                //session.Write(null,
                //              nodesToWrite,
                //              out results,
                //              out diagnosticInfos);
                ////log2File.Info("results.Count " + results.Count);
                ////log2File.Info(results[0].Code);
                ////log2File.Info(results[1].Code);
                ////log2File.Info(results[2].Code);
                ///
                List<ClientOPCUA.Information> infos = new List<ClientOPCUA.Information>();
                ClientOPCUA.Information info = new ClientOPCUA.Information();
                info.tag = tagCodiceArticolo;
                info.infoValue = tiesseInfo.codiceArticolo;
                
                info.type = typeof(string);
                infos.Add(info);

                info = new ClientOPCUA.Information();
                info.tag = tagQuantita;
                info.infoValue = tiesseInfo.quantita;
                info.type = typeof(short);
                infos.Add(info);

                info = new ClientOPCUA.Information();
                info.tag = tagOrderID;
                info.infoValue = tiesseInfo.ordineID;
                info.type = typeof(string); 
                infos.Add(info);

                info = new ClientOPCUA.Information();
                info.tag = tagStatoOrdine;
                info.infoValue = tiesseInfo.statoOrdine;
                info.type = typeof(short);
                infos.Add(info);

                if (tiesseInfo.dataOraInizio != null)
                {
                    DateTime dtInizio = (DateTime)tiesseInfo.dataOraInizio;
                    info = new ClientOPCUA.Information();
                    info.tag = tagDataOraInizio;

                    info.infoValue = dtInizio.ToString(dateTimeFormat);
                    info.type = typeof(string);
                    infos.Add(info);
                }
                if (tiesseInfo.dataOraFine != null)
                {
                    DateTime dtFine = (DateTime)tiesseInfo.dataOraFine;
                    info = new ClientOPCUA.Information();
                    info.tag = tagDataOraFine;

                    info.infoValue = dtFine.ToString(dateTimeFormat);
                    info.type = typeof(string);
                    infos.Add(info);
                }

                //log2File.Info("namespaceIndex: " + namespaceIndex);

                if (client != null)
                    client.write2OPCUAServer(infos,(UInt16)namespaceIndex);
                else
                    log2File.Info("client null");

            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex.Message);
                log2File.Info(ex.Message);
                log2File.Info(ex.StackTrace);

            }
            return true;

        }
        private OutTiesse readCsv(string pathfileName)
        {
            log2File.Info("readCsv " + pathfileName);

            OutTiesse outInfo = new OutTiesse();
            int rowNumber = 0;
            try
            {


                using (TextFieldParser textFieldParser = new TextFieldParser(pathfileName))
                {
                    textFieldParser.TextFieldType = FieldType.Delimited;
                    textFieldParser.SetDelimiters(";");
                    string[] row = textFieldParser.ReadFields();
                    
                    
                    outInfo.ordineID = (string)row[0];
                    outInfo.ordineDescr = (string)row[1];
                    outInfo.codiceArticolo = (string)row[2];
                    outInfo.dataOraInizio = GetDateTime((string)row[3]);
                    outInfo.dataOraFine = GetDateTime((string)row[4]);
                    outInfo.quantita = Convert.ToInt16(row[5]);
                    outInfo.statoOrdine = Convert.ToInt16(row[6]);
                   
                    return outInfo;
                }
            }
            catch (Exception ex)
            {

                //AddOutputLine(ex.Message);

                _logger.LogInformation(ex.Message);
                log2File.Info(ex.Message);
                return null;
            }


        }

       
        //private DateTime? GetDateTime( string strDateTime)
        //{
        //    string[] validformats = new[] { dateTimeFormat };

        //    CultureInfo provider = CultureInfo.InvariantCulture;
        //    string date = strDateTime;
        //    DateTime dateTime;

        //    if (!DateTime.TryParseExact(date, validformats, provider,
        //                                DateTimeStyles.None, out dateTime))
        //    {
        //        log2File.Info("Unable to parse the  dateTime");
        //    }

        //    return dateTime;
        //}

     
    }
}