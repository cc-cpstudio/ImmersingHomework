using System;
using System.Collections.Generic;

namespace ImmersingHomework.Models;

public class HomeworkItem
{
    public Guid Id { get; init; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public List<string> Tags { get; set; }

    public HomeworkItem(string subject, string content, List<string> tags)
    {
        Id = Guid.NewGuid();
        Subject = subject;
        Content = content;
        Tags = tags;
    }
}