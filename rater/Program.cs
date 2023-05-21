using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json.Linq;

internal class Program {
  private static Logger _logger = new Logger("rater");
  private static Rater _rater = new Rater();

  private static void Main(string[] args) {
    if (args.Length < 1) {
      Console.Error.WriteLine("Usage: rater <path to .nclevel file>");
      return;
    }

    string nclevelPath = args[0];

    try {
      using ZipArchive archive = ZipFile.OpenRead(nclevelPath);
      ParseNclevel(archive);

    } catch (Exception e) {
      _logger.Error($"Bad .nclevel file {nclevelPath}: {e.Message}");
    }

    Dictionary<int, Rating> result = _rater.GetUniqueIdList().ToDictionary(
      uid => uid,
      uid => _rater.GetRating(uid)
    );

    // Print out as JSON
    string json = JToken.FromObject(result).ToString();
    Console.WriteLine(json);
  }

  private static void ParseNclevel(ZipArchive archive) {
    foreach (ZipArchiveEntry entry in archive.Entries) {
      string entryPath = entry.FullName;
      string entryDirectory = Path.GetDirectoryName(entryPath) ?? "";
      string entryExtension = Path.GetExtension(entryPath) ?? "";

      // Only read files in /records/ directory.
      if (entryDirectory != "records") {
        continue;
      }

      // Only read .dat files.
      if (entryExtension != ".dat") {
        continue;
      }

      try {
        // Read /record.json in .dat archive file.
        using ZipArchive datArchive = new ZipArchive(entry.Open());
        ParseDat(datArchive);

      } catch (Exception e) {
        throw new Exception($"Bad .dat file {entryPath}: {e.Message}", e);
      }
    }
  }

  private static void ParseDat(ZipArchive datArchive) {
    ZipArchiveEntry recordEntry = datArchive.GetEntry("record.json") ?? throw new Exception("Missing record.json file.");

    try {
      using Stream recordStream = recordEntry.Open();
      using StreamReader recordReader = new StreamReader(recordStream);
      string recordJson = recordReader.ReadToEnd();
      JArray records = JObject.Parse(recordJson)?["records"] as JArray ??
        throw new Exception("Bad record.");

      foreach (JObject? record in records) {
        if (record is null) {
          continue;
        }

        _rater.AddRecord(record);
      }

    } catch (Exception e) {
      throw new Exception($"Bad record.json file: {e.Message}", e);
    }
  }
}
