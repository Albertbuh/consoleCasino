using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class PlayerExtension
{
    

    public static void PrintTexasHoldemHud(this Player player, decimal bank, decimal maxBet, string table)
    {
        if (player.currentGame is TexasHoldem holdem)
        {
            Console.WriteLine($"TABLE OF PLAYER \"{player.Name}\":");
            Console.WriteLine("=====================================================");
            Console.WriteLine($"Bank: {bank,5:C2}");
            Console.WriteLine($"Current Maximum bet: {maxBet,5:C2}");

            Console.WriteLine("Cash: {0,5:C2}", player.Cash);
            Console.WriteLine($"Your bet: {player.Bet,5:C2}");
            if (player.Hand is IPoker.Card[] hand)
            {
                MessageBuilder builder = new MessageBuilder();
                builder.AddTextWithColor("\nHand: ", Console.ForegroundColor);
                foreach(var card in hand)
                {
                    ConsoleColor clr;
                    if (holdem.isRed(card))
                        clr = ConsoleColor.Red;
                    else
                        clr = ConsoleColor.White;
                    builder.AddTextWithColor(holdem.PrintCard(card) + " ", clr);
                }
                builder.AddText("\n");
                builder.GetMessage().Render();
               // Console.WriteLine($"\nHand: {holdem.PrintCard(hand[0])} {holdem.PrintCard(hand[1])}");
            }

            Console.WriteLine("Table: {0}", table);
            Console.WriteLine();
        }
    }

}
