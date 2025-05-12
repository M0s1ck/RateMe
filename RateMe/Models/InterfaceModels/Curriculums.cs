using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using RateMe.Models.JsonModels;

namespace RateMe.Models.InterfaceModels
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
