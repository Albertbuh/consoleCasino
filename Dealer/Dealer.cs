using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Dealer
{
    protected Dictionary<string, decimal> Bets = new Dictionary<string, decimal>();
    protected decimal MaxBet;
    protected decimal Bank;

    public Casino casino;
    public Dealer(Casino casino)
    {
        this.casino = casino;
    }

}