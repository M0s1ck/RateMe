using RateMe.DataUtils.JsonModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace RateMe.DataUtils.InterfaceCollections
{
    public class Curriculums : ObservableCollection<string>
    {
        static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        static readonly string _path = "Data\\Curriculums.json";

        public Curriculums()
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), _path);
            string jsonContent = File.ReadAllText(fullPath);

            CurriculumsModel? curriculumsModel = JsonSerializer.Deserialize<CurriculumsModel>(jsonContent, Options);

            if (curriculumsModel == null)
            {
                throw new IOException("Couldn't desserialize Data\\Curriculums.json");
            }

            foreach (string op in curriculumsModel.Curriculums)
            {
                Add(op);
            }
        }
    }
}
