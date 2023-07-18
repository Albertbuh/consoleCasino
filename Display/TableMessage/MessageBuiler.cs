using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MessageBuilder
{
    Message message = new Message();
    public void AddTextWithColor(string text, ConsoleColor color)
    {
        message.Add(text, color);
    }
    public void AddText(string text)
    {
        message.Add(text, Console.ForegroundColor);
    }
    public Message GetMessage()
    {
        return message;
    }
}


