using System.Collections.Generic;
using System.IO;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Domain.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace COSMIC.Warpdeck.Adapter.Configuration
{
    public class YamlFileClipPatternReaderWriter : IClipPatternReader, IClipPatternWriter
    {
        private readonly string _configBaseDir;

        public YamlFileClipPatternReaderWriter(string configBaseDir)
        {
            _configBaseDir = configBaseDir;
        }

        public List<ClipPattern> ReadPatterns()
        {
            string devicesDir = Path.Join(_configBaseDir, "clip-patterns");
            if (!Directory.Exists(devicesDir))
                return new List<ClipPattern>() { };

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            List<ClipPattern> patterns = new List<ClipPattern>();

            string clipPatternConfigPath = Path.Join(_configBaseDir, "clip-patterns");
            string[] patternConfigFiles = Directory.GetFiles(clipPatternConfigPath, "*.clippattern.yaml");

            foreach (string patternConfigFile in patternConfigFiles)
            {
                ClipPattern pattern = deserializer.Deserialize<ClipPattern>(File.ReadAllText(patternConfigFile));
                patterns.Add(pattern);
            }

            return patterns;
        }

        public void WritePatterns(List<ClipPattern> patterns)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            string clipPatternConfigDir = Path.Join(_configBaseDir, "clip-patterns");

            if (!Directory.Exists(_configBaseDir))
                Directory.CreateDirectory(_configBaseDir);
            if (!Directory.Exists(clipPatternConfigDir))
                Directory.CreateDirectory(clipPatternConfigDir);

            foreach (ClipPattern pattern in patterns)
            {
                string patternPath = Path.Join(clipPatternConfigDir, pattern.Name + ".clippattern.yaml");
                File.WriteAllText(patternPath, serializer.Serialize(pattern));
            }
        }
    }
}