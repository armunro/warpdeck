using System.Collections.Generic;
using COSMIC.Warpdeck.Windows.Domain;

namespace COSMIC.Warpdeck.Windows{

public class MyPatterns
{
    public static readonly List<Pattern> Patterns = new()
    {
        Pattern.Create(
            "JIRA-Identifier", @"((?<!([A-Z]{1,10})-?)[A-Z]+-\d+)",
            (pattern, match) => new []{ new Suggestion
            {
                Type = "BROWSE",
                Value = "https://jira.ysi.yardi.com/browse/" + match.Value
            }}),
        Pattern.Create(
            "STELLAR-CaseID", @"\d{6,9}",
            (pattern, match) => new [] {new Suggestion
            {
                Type = "BROWSE",
                Value = "https://stellar.yardiapp.com/prod/Pages/stellar_caseaction.aspx?CaseId=" + match.Value
            }}),
        Pattern.Create(
            "YCRM-TRID", @"TR-\d{6,9}",
            (pattern, match) => new[]{ new Suggestion
            {
                Type = "BROWSE",
                Value = "https://ycrm.yardiapp.com/prod/?trid=" + match.Value
            }})
    };
}
}