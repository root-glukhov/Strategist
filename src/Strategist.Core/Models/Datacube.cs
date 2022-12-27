using Newtonsoft.Json;
using System.Text;

namespace Strategist.Core.Models;

internal class Datacube
{
    [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
    public string? Title { get; set; }
    [JsonProperty("chart")]
    public Chart Chart { get; init; } = new() { Type = "Candles" };
    [JsonProperty("onchart")]
    public List<Chart> OnChart { get; init; } = new();
    [JsonProperty("offchart")]
    public List<Chart> OffChart { get; init; } = new();
    [JsonProperty("settings")]
    public Settings Settings { get; set; }

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
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name;
    [JsonProperty("type")]
    public string Type;
    [JsonProperty("indexBased", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IndexBased;
    [JsonProperty("data")]
    public List<List<object>> Data;
    [JsonProperty("settings", NullValueHandling = NullValueHandling.Ignore)]
    public Settings Settings;

    public Chart()
    {
        Data = new();
        Settings = new Settings();
    }
}

internal class Settings
{
    [JsonProperty("z-index", NullValueHandling = NullValueHandling.Ignore)]
    public int? zIndex;
    [JsonProperty("schema", NullValueHandling = NullValueHandling.Ignore)]
    public List<string> Schema;
    [JsonProperty("colors", NullValueHandling = NullValueHandling.Ignore)]
    public List<string> Colors;
    [JsonProperty("rangeFrom", NullValueHandling = NullValueHandling.Ignore)]
    public int? RangeFrom;
    [JsonProperty("rangeTo", NullValueHandling = NullValueHandling.Ignore)]
    public int? RangeTo;
}
