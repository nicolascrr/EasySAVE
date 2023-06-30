using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using CryptoSoft;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using System.Xml;

namespace XOREncryption
{
    class Program
    {
        public static List<string> TestExtensions { get; private set; }

        static void Main(string[] args)
        {
            TestExtensions = new List<string> { ".txt", ".docx", ".pptx" };
            string filePathxml = "C:\\EasySave\\backupJobInfoHistory.xml";
            string filePathjson = "C:\\EasySave\\backupJobInfoHistory.json";

            DateTime lastWriteTimexml = File.GetLastWriteTime(filePathxml);
            DateTime lastWriteTimejson = File.GetLastWriteTime(filePathjson);
            Stopwatch stopwatch = new Stopwatch();

            {
                if (lastWriteTimexml > lastWriteTimejson)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<BackupJob>));
                    StreamReader reader = new StreamReader(filePathxml);
                    var BackUpJobList = (List<BackupJob>)serializer.Deserialize(reader);
                    reader.Close();

                    var LastObj = BackUpJobList.LastOrDefault();
                    var destinationPath = LastObj.DestinationPath;
                    stopwatch.Start();
                    EncryptDirectory(destinationPath, TestExtensions);
                    stopwatch.Stop();
                    LastObj.EncryptionDuration = stopwatch.Elapsed;

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.Encoding = Encoding.UTF8;
                    using (XmlWriter writer = XmlWriter.Create(filePathxml, settings))
                    {
                        writer.WriteStartDocument();
                        serializer.Serialize(writer, BackUpJobList);
                    }
                }
                else
                {
                    var BackupJobList = JsonSerializer.Deserialize<List<BackupJob>>(File.ReadAllText(filePathjson));
                    var job = BackupJobList.Last();
                    var destinationPath = job.DestinationPath;
                    stopwatch.Start();
                    EncryptDirectory(destinationPath, TestExtensions);
                    stopwatch.Stop();
                    job.EncryptionDuration = stopwatch.Elapsed;
                    var options = new JsonSerializerOptions
                    { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
                    string newJson = JsonSerializer.Serialize(BackupJobList, options);
                    File.WriteAllText(filePathjson, newJson);
                }
            }
        }


    static void EncryptDirectory(string directoryPath, List<string> Extensions)
        {
            // Récupère la liste des fichiers et dossiers dans le chemin de dossier donné
            string[] files = Directory.GetFiles(directoryPath);
            string[] directories = Directory.GetDirectories(directoryPath);

            // Chiffre tous les fichiers du dossier sauf ceux avec des extensions spécifiées
            foreach (string file in files)
            {
                if (Extensions.Contains(Path.GetExtension(file)))
                {
                    EncryptFile(file);
                }
            }

            // Chiffre tous les fichiers dans les sous-dossiers
            foreach (string directory in directories)
            {
                EncryptDirectory(directory, Extensions);
            }
        }

        static void EncryptFile(string filePath)
        {
            byte key = 0x53; // La clé pour l'algorithme XOR
            byte[] buffer = new byte[1024];

            // Ouverture du fichier en lecture/écriture
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                // Lit le fichier par morceaux et effectue l'algorithme XOR sur chaque octet
                int bytesRead = 0;
                do
                {
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);

                    for (int i = 0; i < bytesRead; i++)
                    {
                        buffer[i] = (byte)(buffer[i] ^ key);
                    }

                    // Écrit les octets chiffrés dans le fichier
                    fileStream.Seek(-bytesRead, SeekOrigin.Current);
                    fileStream.Write(buffer, 0, bytesRead);
                }
                while (bytesRead == buffer.Length);
            }
        }
    }
}

