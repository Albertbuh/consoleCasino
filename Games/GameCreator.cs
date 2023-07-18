using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class GameCreator
{
    public string Name { get; private set; }
    public GameCreator(string name)
    {
        Name = name;
    }
    abstract public Game Create();
}
