using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Message
{
    List<MessagePart> parts = new List<MessagePart>();
    public void Render()
    {
        foreach (var part in parts)
            part.Render();
    }
    public void Add(string text, ConsoleColor color)
    {
        parts.Add(new MessagePart(text, color));
    }

    public struct MessagePart
    {
        public string text;
        public ConsoleColor color;
        public MessagePart(string text, ConsoleColor color)
        {
            this.text = text;
            this.color = color;
        }
        public void Render()
        {
            ConsoleColor temp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = temp;
        }
    }
}