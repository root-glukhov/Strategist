using Newtonsoft.Json;
using System.Text;

namespace StrategistCore.Models;

internal class Datacube
{
    [JsonProperty("chart")]
    public Chart Chart { get; init; } = new();
    [JsonProperty("onchart")]
    public List<Chart> OnChart { get; init; } = new();
    [JsonProperty("offchart")]
    public List<Chart> OffChart { get; init; } = new();

    public async Task Save()
    {
        string fileName = @"wwwroot\data.json";
        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
        using FileStream fstream = new(fileName, FileMode.Create);

        string json = JsonConvert.SerializeObject(this);
        byte[] buffer = Encoding.Default.GetBytes(json);
        await fstream.WriteAsync(buffer, 0, buffer.Length);
        Console.WriteLine("Текст записан в файл");

        fstream.Dispose();
    }
}

internal class Chart
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("data")]
    public List<decimal[]> Data { get; set; }
    [JsonProperty("settings")]
    public IDictionary<string, string> Settings { get; set; }

    public Chart()
    {
        Data = new();
        Settings = new Dictionary<string, string>();
    }
}
