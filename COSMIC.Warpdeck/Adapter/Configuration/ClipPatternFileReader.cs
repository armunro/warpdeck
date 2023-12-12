using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Domain.Configuration;

namespace COSMIC.Warpdeck.Adapter.Configuration
{
    public class ClipPatternFileReader : IClipPatternReader,IClipPatternWriter
    {
        private readonly string _configBaseDir;

        public ClipPatternFileReader(string configBaseDir)
        {
            _configBaseDir = configBaseDir;
        }

        public List<ClipPattern> ReadPatterns()
        {
            
            
            return new List<ClipPattern> ()
            {
                ClipPattern.Create(
                    "JIRA-Identifier", @"((?<!([A-Z]{1,10})-?)[A-Z]+-\d+)",
                    (pattern, match) => new []{ new ClipSuggestion
                    {
                        Type = "BROWSE",
                        Value = "https://jira.ysi.yardi.com/browse/" + match.Value
                    }}),
                ClipPattern.Create(
                    "STELLAR-CaseID", @"\d{6,9}",
                    (pattern, match) => new [] {new ClipSuggestion
                    {
                        Type = "BROWSE",
                        Value = "https://stellar.yardiapp.com/prod/Pages/stellar_caseaction.aspx?CaseId=" + match.Value
                    }}),
                ClipPattern.Create(
                    "YCRM-TRID", @"TR-\d{6,9}",
                    (pattern, match) => new[]{ new ClipSuggestion
                    {
                        Type = "BROWSE",
                        Value = "https://ycrm.yardiapp.com/prod/?trid=" + match.Value
                    }})
            };
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