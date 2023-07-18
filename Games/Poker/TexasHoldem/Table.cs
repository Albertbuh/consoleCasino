using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface ITableState
{
    public void OpenCards(Table table, object o);
}
public class Table : IPoker
{
    public ITableState state { get; set; }
    public IPoker.Card[] cards;
    public bool isOpen = false;
    public Table(ITableState state, int amount)
    {
        cards = new IPoker.Card[amount];
        this.state = state;
    }
    public void OpenCards(Deck deck)
    {
        state.OpenCards(this, deck);
    }
}
public class PreflopTableState : ITableState
{
    public void OpenCards(Table table, object o)
    {
        table.state = new FlopTableState();
        if (o is Deck deck)
        {
            table.cards[0] = deck[0];
            table.cards[1] = deck[1];
            table.cards[2] = deck[2];
        }        
    }
}
public class FlopTableState : ITableState
{
    public void OpenCards(Table table, object o)
    {
        table.state = new TurnTableState();
        if(o is Deck deck)
        {
            table.cards[3] = deck[3];
        }
    }
}
public class TurnTableState : ITableState
{
    public void OpenCards(Table table, object o)
    {
        table.state = new RiverTableState();
        if (o is Deck deck)
            table.cards[4] = deck[4];
    }
}
public class RiverTableState : ITableState
{
    public void OpenCards(Table table, object o)
    {
        table.isOpen = true;
        if(o is Deck deck)
        {
            deck.RemoveCards(0, 5);
        }
    }
}
