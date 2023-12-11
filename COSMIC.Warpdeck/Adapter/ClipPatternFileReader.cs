using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Domain.Configuration;

namespace COSMIC.Warpdeck.Adapter
{
    public class ClipPatternFileReader : IClipPatternReader
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
    }
}