using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPoker
{
    public enum CardSuitEnum
    {
        Hearts = 3, Diamonds = 2, Clubs = 1, Spades = 0
    }
    public enum CardValuesEnum
    {
        Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10,
        Valey = 11, Queen = 12, King = 13, Ace = 14
    }

    public struct Card : ICloneable, IComparable
    {
        public CardSuitEnum Suit;
        public CardValuesEnum Value;
        public Card(CardSuitEnum suit, CardValuesEnum value)
        {
            Suit = suit;
            Value = value;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public int CompareTo(object? obj)
        {
            if (obj is Card card)
                return card.Value.CompareTo(Value);
            else
                throw new ArgumentException("Invalid argument to compare");
        }
    }
/*    public void Check();
    public void Call();
    public void Raise();
    public void Fold();*/
}
