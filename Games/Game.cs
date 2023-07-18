using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class Game
{
    abstract public string Name { get; }
    public List<IPlayer> Players = new List<IPlayer>();
    public bool isFinished = false;
    virtual public void AddPlayer(IPlayer player)
    {
        Players.Add(player);
    }
    abstract public void StartGame();
    abstract public void FinishGame();
}