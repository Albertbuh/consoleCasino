using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TexasHoldem : Game, IPoker
{
    public const int animationDelay = 1500;
    public static Dictionary<IPoker.CardSuitEnum, char> deckSuits = new Dictionary<IPoker.CardSuitEnum, char>()
    {
        [IPoker.CardSuitEnum.Hearts] = '\u2665',
        [IPoker.CardSuitEnum.Diamonds] = '\u2666',
        [IPoker.CardSuitEnum.Clubs] = '\u2663',
        [IPoker.CardSuitEnum.Spades] = '\u2660',
    };
    public static Dictionary<IPoker.CardValuesEnum, string> deckValues = new Dictionary<IPoker.CardValuesEnum, string>()
    {
        [IPoker.CardValuesEnum.Two] = "2",
        [IPoker.CardValuesEnum.Three] = "3",
        [IPoker.CardValuesEnum.Four] = "4",
        [IPoker.CardValuesEnum.Five] = "5",
        [IPoker.CardValuesEnum.Six] = "6",
        [IPoker.CardValuesEnum.Seven] = "7",
        [IPoker.CardValuesEnum.Eight] = "8",
        [IPoker.CardValuesEnum.Nine] = "9",
        [IPoker.CardValuesEnum.Ten] = "10",
        [IPoker.CardValuesEnum.Valey] = "V",
        [IPoker.CardValuesEnum.Queen] = "Q",
        [IPoker.CardValuesEnum.King] = "K",
        [IPoker.CardValuesEnum.Ace] = "A",
    };

    //from Game
    //public List<IPlayer> Players = new List<IPlayer>();
    //isFinished
    public override string Name => "Texas Holdem";
    public Casino? Casino;
    public HoldemDealer dealer;
    public Deck Deck = new Deck(); 
    public Table Table = new Table(new PreflopTableState(), 5);
    public TexasHoldem()
    {
        Deck.PrintDeckEvent += PrintDeck;
    }    
    public void PrintDeck(List<IPoker.Card> Deck)
    {
        if (Deck != null)
        {
            foreach (var card in Deck)
            {
                Console.WriteLine($"{deckValues[card.Value]}{deckSuits[card.Suit]}");
            }
        }
    }
    public void AddCasino(Casino casino)
    {
        Casino = casino;
        dealer = new HoldemDealer(casino);
    }
    
    public void HandCards()
    {
        if (Deck == null)
            throw new ArgumentNullException("Deck is empty");
                
        foreach(Player player in Players)
        {
            IPoker.Card[] hand = new IPoker.Card[2];
            hand[0] = Deck[0];
            hand[1] = Deck[1];
            player.CreateHand(hand);
            Deck.RemoveCards(0, 2);
        }
    }
    public override void StartGame()
    {
        //deck manipulations
        Deck.Create(deckSuits.Keys.ToArray(), deckValues.Keys.ToArray());
        Random rand = new Random();
        int shuffleAmount = rand.Next(2, 5);
        for (int i = 0; i < shuffleAmount; i++)
        {
            Deck.Shuffle();
        }
        Console.WriteLine("Deck created");
        HandCards();
        Console.WriteLine("Cards on hands");

        //tell dealer about players
        foreach (Player player in Players)
            dealer.AddObserver(player);

        Console.WriteLine("Invite player \"{1}\" in {0:f2} seconds", (float)animationDelay / 1000, Players[0].Name);
        Thread.Sleep(animationDelay);
        //players make bets
        while (!Table.isOpen && dealer.Players.Count > 1)
        {
            dealer.NotifyObservers(this);
            if(dealer.FinishCircle)
            {
                Console.WriteLine("Open cards...");
                //tableState = OpenCards(table, tableState);
                Table.OpenCards(Deck);
            }
        }

        Console.WriteLine("Table has been opened, start counting combinations..."); 
        Thread.Sleep(animationDelay);
        Players = dealer.Players;
        if(Players.Count == 1)
        {
            while (!Table.isOpen)//(tableState != TableStateEnum.empty)
                Table.OpenCards(Deck);//tableState = OpenCards(table, tableState);
        }    
        //Count combinations        
        dealer.NotifyObservers(CountCombinations(Players));

        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
        FinishGame();
    }
    public Dictionary<IPlayer, CardCombinationEnum> CountCombinations(IEnumerable<IPlayer> players)
    {
        Dictionary<IPlayer, CardCombinationEnum> combinations = new Dictionary<IPlayer, CardCombinationEnum>();
        foreach (IPlayer player in players)
        {
            List<IPoker.Card> tablehand = new List<IPoker.Card>();
            tablehand.AddRange(Table.cards);
            if (player.Hand is IPoker.Card[] hand)
                tablehand.AddRange(hand);

            int[] cardValues = new int[tablehand.Count];
            int[] cardSuits = new int[tablehand.Count];
            for (int i = 0; i < tablehand.Count; i++)
            {
                cardValues[i] = (int)tablehand[i].Value;
                cardSuits[i] = (int)tablehand[i].Suit;
            }
            cardValues = cardValues.OrderByDescending(v => v).ToArray();
            List<int> newhand = new List<int>();
            CardCombinationEnum combination = GetCardCombination(cardValues, cardSuits, newhand);
            combinations.Add(player, combination);
            int counter = 0;
            Console.Write("{0,-10} (", player.Name);
            tablehand = tablehand.OrderBy((IPoker.Card c) => c).ToList();
            MessageBuilder builder = new MessageBuilder();
            foreach (var card in tablehand)
            {
                if (newhand.Contains((int)card.Value) && counter <= 5)
                {
                    //Console.Write($"{PrintCard(card)} ");
                    if (isRed(card))
                        builder.AddTextWithColor($"{PrintCard(card)} ", ConsoleColor.Red);
                    else
                        builder.AddTextWithColor($"{PrintCard(card)} ", ConsoleColor.White);
                    counter++;
                }
            }
            builder.GetMessage().Render();
            Console.WriteLine(") => {0}", combination);
        }
        return combinations;
    }

    public bool isRed(IPoker.Card card)
    {
        return card.Suit == IPoker.CardSuitEnum.Diamonds || card.Suit == IPoker.CardSuitEnum.Hearts;
    }

    public enum CardCombinationEnum
    {
        HighCard, Pair, TwoPair, Set, Straight, Flush, FullHaus, Quad, StraightFlush
    }
    public CardCombinationEnum GetCardCombination(int[] values, int[] suits,List<int> hand)
    {
        CardCombinationEnum result = CardCombinationEnum.HighCard;

        List<int> straightHand = new List<int>();
        List<int> flushHand = new List<int>();
        List<int> newHand = new List<int>();
        bool isStraight = CheckToStraight(values, straightHand, 5);
        bool isFlush = CheckToFlush(values, suits, flushHand, 5);

        if (isStraight && isFlush)                  //StraighFlush
            result = CardCombinationEnum.StraightFlush;
        else if (CheckToSame(values, newHand, 4))    //quad
            result = CardCombinationEnum.Quad;
        else if (CheckToTwoSame(values, newHand, 3, 2)) //fullhaus
            result = CardCombinationEnum.FullHaus;
        else if (isFlush)                                   //flush
            result = CardCombinationEnum.Flush;
        else if (isStraight)                                //straight
            result = CardCombinationEnum.Straight;
        else if (CheckToSame(values, newHand, 3))       //set
            result = CardCombinationEnum.Set;
        else if (CheckToTwoSame(values, newHand, 2, 2)) //two pairs
            result = CardCombinationEnum.TwoPair;
        else if (CheckToSame(values, newHand, 2)) //pair
            result = CardCombinationEnum.Pair;

        
        if (straightHand.Count >= 5)
            hand.AddRange(straightHand);
        else if(flushHand.Count >= 5)
            hand.AddRange(flushHand);
        else
        {
            int ind = 0;
            while (newHand.Count < 5)
            {
                if (!newHand.Contains(values[ind]))
                    newHand.Add(values[ind]);
                ind++;
            }
            hand.AddRange(newHand);
        }
        return result;
    }
    public bool CheckToTwoSame(int[] values, List<int> newhand, int amount1, int amount2)
    {
        if (CheckToSame(values, newhand, amount1))
        {
            int ind = 0;
            int exceptVal = newhand.First();
            int[] remainHand = new int[values.Length - newhand.Count];
            foreach (var val in values)
            {
                if (val != exceptVal)
                    remainHand[ind++] = val;
            }
            List<int> temp = new List<int>();
            bool result = CheckToSame(remainHand, temp, amount2);
            if (result)
                newhand.AddRange(temp);
        }
        return newhand.Count == amount1+amount2;
    }
    public bool CheckToSame(int[] values, List<int> newhand, int amount)
    {
        newhand.Clear();
        int curValue = values[0];
        newhand.Add(values[0]);
        for (int i = 1; i < values.Length && newhand.Count < amount; i++)
        {
            if (curValue != values[i])
                newhand.Clear();
            newhand.Add(values[i]);
            curValue = values[i];
        }
        return newhand.Count == amount;
    }
    public bool CheckToFlush(int[] values, int[] suits, List<int> newhand, int amount)
    {
        Dictionary<int, int> suitsDict = new Dictionary<int, int>();
        for (int i = 0; i < suits.Length; i++)
        {
            if (suitsDict.ContainsKey(suits[i]))
                suitsDict[suits[i]]++;
            else
                suitsDict.Add(suits[i], 1);
        }
        var flushSuit = suitsDict.Values.FirstOrDefault((int s) => s >= amount);
        newhand.Clear();
        if (flushSuit != default(int))
        {
            for(int i=0; i<values.Length && newhand.Count < amount; i++)
            {
                if (suits[i] == flushSuit)
                    newhand.Add(values[i]);
            }
        }
        return newhand.Count == amount;
    }
    public bool CheckToStraight(int[] values, List<int> newhand, int amount)
    {
        int curVal = values[0];
        newhand.Clear();
        newhand.Add(curVal);
        for(int i=1; i<values.Length && newhand.Count < amount; i++)
        {
            if (values[i] != curVal - 1)
                newhand.Clear();
            newhand.Add(values[i]);
            curVal = values[i];
        }
        return newhand.Count >= amount;
    }
    public Message ShowTable(Table table)//IPoker.Card[] cards, TableStateEnum tableState)
    {
        //string table = "";
        MessageBuilder tableMessage = new MessageBuilder();
        tableMessage.AddText("Table: ");
        int ind = 0;
        while (ind < table.cards.Length && table.cards[ind].Value != 0)
        {
            ConsoleColor clr;
            if (isRed(Table.cards[ind]))
                clr = ConsoleColor.Red;
            else
                clr = ConsoleColor.White;
            tableMessage.AddTextWithColor($"{PrintCard(Table.cards[ind])} ", clr);
            ind++;
        }
        while(ind < 5)
        {
            tableMessage.AddTextWithColor("XX ", ConsoleColor.Gray);
            ind++;
        }
        tableMessage.AddText("\n");
        return tableMessage.GetMessage();
    }
    public string PrintCard(IPoker.Card card)
    {
        return $"{deckValues[card.Value]}{deckSuits[card.Suit]}";
    }
    public override void FinishGame()
    {
        isFinished = true;
        foreach(Player player in Players)
        {
            player.Bet = Casino.minBet;
            player.Hand = null;
        }
    }
}

