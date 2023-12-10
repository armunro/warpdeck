using System.Collections.Generic;
using COSMIC.Warpdeck.Domain.Clipboard;

namespace COSMIC.Warpdeck.Windows{

public class MyPatterns
{
    public static readonly List<ClipboardPattern> Patterns = new()
    {
        ClipboardPattern.Create(
            "JIRA-Identifier", @"((?<!([A-Z]{1,10})-?)[A-Z]+-\d+)",
            (pattern, match) => new []{ new ClipboardSuggestion
            {
                Type = "BROWSE",
                Value = "https://jira.ysi.yardi.com/browse/" + match.Value
            }}),
        ClipboardPattern.Create(
            "STELLAR-CaseID", @"\d{6,9}",
            (pattern, match) => new [] {new ClipboardSuggestion
            {
                Type = "BROWSE",
                Value = "https://stellar.yardiapp.com/prod/Pages/stellar_caseaction.aspx?CaseId=" + match.Value
            }}),
        ClipboardPattern.Create(
            "YCRM-TRID", @"TR-\d{6,9}",
            (pattern, match) => new[]{ new ClipboardSuggestion
            {
                Type = "BROWSE",
                Value = "https://ycrm.yardiapp.com/prod/?trid=" + match.Value
            }})
    };
}
}