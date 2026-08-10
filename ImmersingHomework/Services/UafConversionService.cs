using System;
using System.Collections.Generic;
using System.Linq;
using ImmersingHomework.Models;
using ImmersingHomework.Uaf.Core.Models;

namespace ImmersingHomework.Services;

public static class UafConversionService
{
    public static List<UafPayload> HomeworkToUafPayloads(Homework homework)
    {
        return homework.HomeworkItems
            .Select(item => HomeworkItemToUafPayload(item, homework.Date))
            .ToList();
    }

    public static UafPayload HomeworkItemToUafPayload(HomeworkItem item, DateOnly date)
    {
        return new UafPayload(
            subject: item.Subject,
            date: date.ToString("yyyy-MM-dd"),
            content: item.Content,
            tags: item.Tags?.AsReadOnly() ?? new List<string>().AsReadOnly()
        );
    }

    public static List<Homework> UafPayloadsToHomeworkList(List<UafPayload> payloads)
    {
        return payloads
            .GroupBy(p => p.Date)
            .Select(group =>
            {
                var date = DateOnly.Parse(group.Key);
                var items = group.Select(UafPayloadToHomeworkItem).ToList();
                return new Homework(date, items);
            })
            .ToList();
    }

    public static HomeworkItem UafPayloadToHomeworkItem(UafPayload payload)
    {
        return new HomeworkItem(
            subject: payload.Subject,
            content: payload.Content,
            tags: payload.Tags?.ToList() ?? new List<string>()
        );
    }

    public static List<UafPayload> MergeHomeworkListToUafPayloads(List<Homework> homeworkList)
    {
        return homeworkList.SelectMany(HomeworkToUafPayloads).ToList();
    }
}
