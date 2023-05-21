using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json.Nodes;

internal class Program {
  private static void Main(string[] args) {
    if (args.Length < 1) {
      Console.WriteLine("Usage: rater <path to .nclevel file>");
      return;
    }

    string nclevelPath = args[0];

    using ZipArchive archive = ZipFile.OpenRead(nclevelPath);
    foreach (ZipArchiveEntry entry in archive.Entries) {
      string entryPath = entry.FullName;
      string entryDirectory = Path.GetDirectoryName(entryPath);
      string entryExtension = Path.GetExtension(entryPath);

      // Only read files in /records/ directory.
      if (entryDirectory != "records") {
        continue;
      }

      // Only read .dat files.
      if (entryExtension != ".dat") {
        continue;
      }

      // Read /record.json in .dat archive file.
      using ZipArchive datArchive = new ZipArchive(entry.Open());
      ZipArchiveEntry recordEntry = datArchive.GetEntry("record.json");

      // Read record.json and deserialize it.
      using Stream recordStream = recordEntry.Open();
      using StreamReader recordReader = new StreamReader(recordStream);

      string recordJson = recordReader.ReadToEnd();
      JsonNode record = JsonNode.Parse(recordJson);
    }
  }
}
