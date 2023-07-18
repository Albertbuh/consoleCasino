using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Deck : IPoker
{
    public List<IPoker.Card> cards = new List<IPoker.Card>();

    public event Action<List<IPoker.Card>> PrintDeckEvent;
    public Deck()
    {
        PrintDeckEvent = PrintDeckConsole;
    }
    public List<IPoker.Card> Create(IPoker.CardSuitEnum[] suits, IPoker.CardValuesEnum[] values)
    {
        cards = new List<IPoker.Card>();
        foreach (var cardSuit in suits)
        {
            foreach (var cardValue in values)
            {
                cards.Add(new IPoker.Card(cardSuit, cardValue));
            }
        }
        return cards;
    }

    public bool Shuffle()
    {
        if (cards == null)
            return false;

        Random rand = new Random();
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int k = rand.Next(i + 1);
            var temp = cards[k];
            cards[k] = cards[i];
            cards[i] = temp;
        }
        return true;
    }
    private void PrintDeckConsole(List<IPoker.Card> cards)
    {
        /*foreach(var card in cards)
            Console.WriteLine(card);*/
    }
    public void RemoveCards(int startIndex, int count)
    {
        cards.RemoveRange(startIndex, count);
    }
    public IPoker.Card this[int index]
    {
        get
        {
            if (index >= 0 && index < cards.Count)
                return cards[index];
            else
                throw new ArgumentOutOfRangeException("Deck list doesnt contains such index");
        }
        set
        {
            if (index > 0 && index < cards.Count)
                cards[index] = value;
        }
    }
}