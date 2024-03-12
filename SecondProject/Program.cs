using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Renci.SshNet;
using static System.Net.WebRequestMethods;

namespace SecondProject
{
    public class Program
    {
        const string HOST = "127.0.0.1"; //test.rebex.net:22  demo/password
        const int PORT = 21;
        const string USERNAME = "Laylo";
        const string PASSWORD = "Laylo25Mir02";
        const string REMOTE_DIRECTORY = "/Archives/test";

       
        const string LOCAL_DIRECTORY = @"C:\Users\Laylo\Desktop\Demo";

        
        const string XML_FILE = "C:\\Users\\Laylo\\Desktop\\Eskhata\\Archives\\example.xml";

        static bool IsFileClosed(string path)
        {
            try
            {
                using (var file = System.IO.File.Open(path, FileMode.Open, FileAccess.ReadWrite))
                {
                }
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        static void SendFileToSftpServer(string path)
        {
            try
            {
                
                using (var client = new SftpClient(HOST, PORT, USERNAME, PASSWORD))
                {
                    client.Connect();
                    Console.WriteLine($"Uploading file to: {path}");

                    // Загрузка файла
                    //client.UploadFile(path, Path.Combine(REMOTE_DIRECTORY, Path.GetFileName(path)));
                  
                    using (FileStream fileStream = new FileStream(path, FileMode.Open))
                    {
                      
                        client.UploadFile(fileStream, Path.Combine(REMOTE_DIRECTORY, Path.GetFileName(path)));
                    }

                   
                    using (FileStream fileStream = new FileStream("/tmp/" + Path.GetFileName(path), FileMode.Create))
                    {
                        client.DownloadFile(Path.Combine(REMOTE_DIRECTORY, Path.GetFileName(path)), fileStream);
                    }

                }

                Console.WriteLine($"Файл {path} успешно отправлен на сервер SFTP");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при отправке файла {path}: {e}");
            }
        }

        static void Main(string[] args)
        {
            
            var watcher = new FileSystemWatcher(LOCAL_DIRECTORY);
            watcher.Filter = "*.xml";
            watcher.Created += OnCreated;
            watcher.EnableRaisingEvents = true;

           
            while (true)
            {
                Thread.Sleep(1000);

               
                if (System.IO.File.Exists(XML_FILE))
                {
                    
                    if (IsFileClosed(XML_FILE))
                    {
                       
                        SendFileToSftpServer(XML_FILE);
                    }
                }
            }
        }

        static void OnCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Новый файл создан: {e.FullPath}");
        }
    }
}




