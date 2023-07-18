using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Displayer
{
    MenuItem root;
    public Func<string, int> getUserInput;
    public Dictionary<string, MenuComponent> menuElements = new Dictionary<string, MenuComponent>();
    public Displayer(string message)
    {
        root = new MenuItem(message);
        menuElements.Add("root", root);
        getUserInput += GetUserInputInt;
    }
    public void Add(string parentKey, string key, MenuComponent component)
    {
        menuElements.Add(key, component);
        menuElements[parentKey].Add(component);
    }
    public void Remove(string parentKey, string key)
    {
        menuElements[parentKey].Remove(menuElements[key]);
        menuElements.Remove(key);
    }
    public void Display()
    {
        root.Display();
    }

    protected virtual int GetUserInputInt(string message = "Enter: ")
    {
        int result = 0;
        Console.Write(message);
        Int32.TryParse(Console.ReadLine(), out result);
        return result;
    }
}

public abstract class MenuComponent
{
    public static int spaceLen = 2;
    protected string message;
    public int space = -spaceLen;
    public MenuComponent(string message)
    {
        this.message = message;
    }
    public abstract void Display();
    public abstract void Add(MenuComponent c);
    public abstract void Remove(MenuComponent c);
}

public class MenuItem : MenuComponent
{
    List<MenuComponent> menuPoints = new List<MenuComponent>();
    public MenuItem(string message) : base(message) 
    {
    }
    public override void Add(MenuComponent c)
    {
        c.space = this.space + MenuComponent.spaceLen;
        menuPoints.Add(c);
    }
    public override void Remove(MenuComponent c)
    {
        menuPoints.Remove(c);
    }
    public override void Display()
    {
        for (int i = 0; i < space; i++)
            Console.Write(" ");
        Console.WriteLine(message);
        foreach(MenuComponent c in menuPoints)
        {
            c.Display();
        }
    }
}

public class MenuPoint : MenuComponent
{
    public MenuPoint(string message) : base(message) 
    { }
    public override void Display()
    {
        for(int i=0; i<space; i++)
            Console.Write(" ");
        Console.WriteLine(message);
    }

    public override void Add(MenuComponent c)
    {
        throw new InvalidOperationException("Trying to connect leaf to leaf");
    }

    public override void Remove(MenuComponent c)
    {
        throw new InvalidOperationException("Trying to remove any element from leaf element");
    }
}