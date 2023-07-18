using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class HoldemDealer : Dealer, IObservable
{
    //Dealer.cs
    //protected decimal[] Bets;
    //protected decimal MaxBet;
    //protected decimal Bank;
    public List<Func<IPlayer, decimal, bool>> BetEvents;
    public List<IPlayer> Players;
    public bool FinishCircle = true;
    public HoldemDealer(Casino casino) : base(casino)
    {
        BetEvents = new List<Func<IPlayer, decimal, bool>>()
        {
            Check, Call, Raise, Fold
        };
        Players = new List<IPlayer>();
    }    
    public void AddPlayerEvent(Func<IPlayer, decimal, bool> newEvent)
    {
        BetEvents.Add(newEvent);
    }

    public void AddObserver(IObserver o)
    {
        Players.Add((IPlayer)o);
    }
    public void RemoveObserver(IObserver o)
    {
        Players.Remove((IPlayer)o);
    }

    private List<IPlayer> folds = new List<IPlayer>();
    public void NotifyObservers(object o)
    {
        if (o is TexasHoldem holdem)
        {
            int checkCount = 0;
            FinishCircle = true;
            foreach (Player player in Players)
            {
                MaxBet = Players.Max((IPlayer player) => player.Bet);
                Bank = Players.Sum((IPlayer player) => player.Bet);

                BetInfo info = new(holdem, Bank, MaxBet,
                                        holdem.ShowTable(holdem.Table));
                player.Update(info);
                checkCount = MakeBet(player, checkCount);

                Console.Clear();
                Console.WriteLine("New player invited in {0:f2} seconds", (float)TexasHoldem.animationDelay / 1000);
                Thread.Sleep(TexasHoldem.animationDelay);
                Console.WriteLine("=====================================================");
                Console.Clear();
            }
            int ind = 0;
            while (ind < folds.Count && Players.Count > 1)
            {
                RemoveObserver((IObserver)folds[ind++]);
            }
        }
        else if (o is Dictionary<IPlayer, TexasHoldem.CardCombinationEnum> combinations)
        {
            int max = 0;
            List<IPlayer> winners = new List<IPlayer>();
            foreach (var comb in combinations)
            {
                if (max < (int)comb.Value)
                {
                    max = (int)comb.Value;
                    winners.Clear();
                    winners.Add(comb.Key);
                }
                else if (max == (int)comb.Value)
                    winners.Add(comb.Key);
            }
            if(winners.Count > 1)
                Console.WriteLine("NOBODY WON, NO CLEAR WINNER");
            else
                Console.WriteLine($"Player {winners[0].Name} WON THIS ROUND!!! He takes bank {Bank:C2}");
            foreach(Player player in Players)
            {
                if (!winners.Any((IPlayer pl) => pl.GetHashCode == player.GetHashCode)
                    || winners.Count > 1)//player.GetHashCode() != winners.GetHashCode())
                    player.Update(new WinnerInfo($"You lose {player.Name}:(", false, 0));
                else
                    player.Update(new WinnerInfo($"My congratulations {player.Name}!!!", true, Bank, winners.Count));
            }
        }
    }
    public int MakeBet(IPlayer player, int CheckCount)
    {
        int actionId;
        bool betFinished = false;
        while (!betFinished)
        {
            Console.WriteLine("Choose action: \n1.Check\t 2.Call\t 3.Raise\t 4.Fold");
            Console.Write("Enter: ");
            Int32.TryParse(Console.ReadLine(), out actionId);
            if (actionId > 0 && actionId - 1 < BetEvents.Count)
            {
                decimal playerPrevBet = player.Bet;
                betFinished = BetEvents[actionId - 1].Invoke(player, MaxBet);
                MaxBet = Math.Max(MaxBet, player.Bet);
                Bank += MaxBet - playerPrevBet;
                if (actionId > 2 || !betFinished)
                    FinishCircle = false;
            }
            else
                Console.WriteLine("Please, enter another action");
        }
        return CheckCount;
    }
   
    protected bool Check(IPlayer player, decimal maxBet)
    {
        if (player.Bet < maxBet)
        {
            if (maxBet <= player.Cash)
            {
                Console.WriteLine("Your bet is less than table bet.\nPlease, use Call action to up your bet or Fold to quit this round");
                return false;
            } // else player.Bet == player.Cash and we couldn't take more 
        }
        return true;
    }
    protected bool Call(IPlayer player, decimal maxBet)
    {
        if (maxBet < player.Cash)
            player.Bet = maxBet;
        else
            player.Bet = player.Cash;
        return true;
    }
    protected bool Raise(IPlayer player, decimal maxBet)
    {
        bool betSucceed = true;
        Console.Write("Enter your bet raise: ");
        decimal raise = 0;
        Decimal.TryParse(Console.ReadLine(), out raise);
        if (raise < player.Cash)
            player.Bet = maxBet + raise;
        else
        {
            Console.WriteLine("You want to set more than you have, would you like to set All-In? (y/N)");
            string ans = Console.ReadLine() ?? "n";
            if (ans.Equals("y"))
                player.Bet = player.Cash;
            else
                betSucceed = false;
        }
        return betSucceed;
    }
    protected bool Fold(IPlayer player, decimal maxBet)
    {
        if(player is Player pl)
        {
            pl.Update(new WinnerInfo($"{player.Name} Folded", false, 0));
        }
        folds.Add(player);
        return true;
    }
    
    public struct WinnerInfo
    {
        public string message;
        public bool winner;
        public decimal bank;
        public int winnerAmount;
        public WinnerInfo(string message, bool win, decimal bank, int winnerAmount = 1)
        {
            this.message = message;
            this.winner = win;
            this.bank = bank;
            this.winnerAmount = winnerAmount;
        }
    }
    public struct BetInfo
    {
        public decimal maxBet;
        public decimal bank;
        public Message tableProjection;
        public TexasHoldem lobby;
        public BetInfo(TexasHoldem lobby, decimal bank, decimal maxBet, Message table)
        {
            this.lobby = lobby;
            this.bank = bank;
            this.maxBet = maxBet;
            this.tableProjection = table;
        }
    }
}
