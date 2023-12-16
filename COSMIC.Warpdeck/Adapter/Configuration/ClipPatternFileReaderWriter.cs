using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Domain.Configuration;

namespace COSMIC.Warpdeck.Adapter.Configuration
{
    public class ClipPatternFileReaderWriter : IClipPatternReader, IClipPatternWriter
    {
        private readonly string _configBaseDir;

        public ClipPatternFileReaderWriter(string configBaseDir)
        {
            _configBaseDir = configBaseDir;
        }

        public List<ClipPattern> ReadPatterns()
        {
            List<ClipPattern> patterns = new List<ClipPattern>();
  
            string clipPatternConfigPath = Path.Join(_configBaseDir, "clip-patterns");
            string[] patternConfigFiles = Directory.GetFiles(clipPatternConfigPath, "*.wdclippattern.json");
            
            foreach (string patternConfigFile in patternConfigFiles)
            {
                ClipPattern pattern = JsonSerializer.Deserialize<ClipPattern>(File.ReadAllText(patternConfigFile));
                patterns.Add(pattern);
            }
            
            return patterns;
        }

        public void WritePatterns(List<ClipPattern> patterns)
        {
            string clipPatternConfigDir = Path.Join(_configBaseDir, "clip-patterns");

            if (!Directory.Exists(_configBaseDir))
                Directory.CreateDirectory(_configBaseDir);
            if (!Directory.Exists(clipPatternConfigDir))
                Directory.CreateDirectory(clipPatternConfigDir);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            foreach (ClipPattern pattern in patterns)
            {
                string patternPath = Path.Join(clipPatternConfigDir, pattern.Name + ".wdclip.json");
                File.WriteAllText(patternPath, JsonSerializer.Serialize(pattern, options));
            }
        }
    }
}