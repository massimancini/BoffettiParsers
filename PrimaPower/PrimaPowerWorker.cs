using log4net;
using log4net.Config;
using Microsoft.VisualBasic.FileIO;
using ParserWorkerBase;

namespace PrimaPower
{
    public class PrimaPowerWorker : Worker
    {
        public PrimaPowerWorker(ILogger<PrimaPowerWorker> logger) : base(logger)
        {

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            
            Task t = base.StartAsync(cancellationToken);
            log2File.Info("dopo di base.StartAsync.");
            importFolder = Path.Combine(rootFolder, importFolder);
            log2File.Info("importFolder: " + importFolder);
            String[] filesPresent = Directory.GetFiles(importFolder);
            foreach (String pathfilename in filesPresent)
            {
                OutPrimaPower outInfo = readCsv(pathfilename);

                MoveParsedFile(pathfilename);
                //writeInfo(outInfo);
            }

            return t;
            //await base.StartAsync(cancellationToken);
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //        await Task.Delay(1000, stoppingToken);
        //    }
        //}

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
                    OutPrimaPower outInfo = await readExcelAsync(pathfilename);

                    MoveParsedFile(pathfilename);
                    //writeInfo(outInfo);

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

        private async Task<OutPrimaPower> readExcelAsync(string fileName)
        {
            OutPrimaPower outList = await Task.Run(() => { return readCsv(fileName); });
            return outList;
        }

        private bool writeInfo(OutPrimaPower tiesseInfo)
        {
            try
            {
               
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

               

                //log2File.Info("namespaceIndex: " + namespaceIndex);

                if (client != null)
                    client.write2OPCUAServer(infos, (UInt16)namespaceIndex);
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
        private OutPrimaPower readCsv(string pathfileName)
        {
            log2File.Info("readCsv " + pathfileName);

            OutPrimaPower outInfo = new OutPrimaPower();
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
                    //outInfo.dataOraInizio = GetDateTime((string)row[3]);
                    //outInfo.dataOraFine = GetDateTime((string)row[4]);
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

    }
}