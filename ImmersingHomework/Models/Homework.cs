using System;
using System.Collections.Generic;
using System.Linq;

namespace ImmersingHomework.Models;

public class Homework
{
    public DateOnly Date { get; init; }
    public List<HomeworkItem> HomeworkItems { get; set; }

    public Homework(DateOnly date, List<HomeworkItem> homeworkItems)
    {
        Date = date;
        HomeworkItems = homeworkItems;
    }

    public void AddHomeworkItem(HomeworkItem homeworkItem)
    {
        HomeworkItems.Add(homeworkItem);
    }

    public void RemoveHomeworkItem(HomeworkItem homeworkItem)
    {
        var itemToRemove = HomeworkItems.Find(x => x.Id == homeworkItem.Id);
        if (itemToRemove != null)
        {
            HomeworkItems.Remove(itemToRemove);
        }
    }

    public HomeworkItem? GetHomeworkItem(Guid id)
    {
        return HomeworkItems.Find(x => x.Id == id);
    }

    public List<HomeworkItem> GetHomeworkItemsBySubject(string subject)
    {
        return HomeworkItems.FindAll(x => x.Subject == subject);
    }

    public List<HomeworkItem> GetHomeworkItemsByTags(List<string> tags)
    {
        return HomeworkItems.Where(item => 
            item.Tags != null && 
            tags.Any(tag => item.Tags.Contains(tag))).ToList();
    }
}