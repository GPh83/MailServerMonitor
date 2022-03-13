using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MailServerMonitor.Models
{
    internal class DataManager
    {
        internal Config config = new();


        /// <summary>
        /// Load files 
        /// </summary>
        internal void Start()
        {
            LoadConfig();
        }

        internal void LoadConfig()
        {
            if (!File.Exists("config.json"))
            {
                SaveConfig();
            }
            String jsonString;
            jsonString = File.ReadAllText("config.json");
            config = JsonSerializer.Deserialize<Config>(jsonString);
        }

        internal void SaveConfig()
        {
            String jsonString = JsonSerializer.Serialize<Config>(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("config.json", jsonString);
        }

        internal void AddToCSV(Histo his)
        {
            if(!File.Exists(config.CSVName)) File.AppendAllText(config.CSVName, his.CSVHeader());
            File.AppendAllText(config.CSVName,his.CSVLine());
        }
    }
}
