using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public class Player : IPlayer, IObserver
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public decimal Cash { get; set; }

    public Casino Casino; 
    public Game currentGame;
    [NotMapped]
    public object? Hand { get; set; }
    [NotMapped]
    public decimal Bet { get; set; }
    public void CreateHand<T>(T hand) => Hand = hand;
    private Player() { } //constructor for database
    public Player(Casino casino)
    {
        Casino = casino;
        Bet = Casino.minBet;
    }
    

    private string CountHash(string? message)
    {
        if (message == null)
            return "";

        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(message);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }
    }

    public bool Authentication(string? userinfo, string? password, string? email = "")
    {
        if (userinfo == null || password == null)
            return false;

        bool isAuthenticated = false;
        //var players = Casino.DB.Players.Where((IPlayer player) => player.Name!.ToLower().Equals(userinfo) || player.Email!.Equals(userinfo));
        IPlayer? player = Casino.DB.Players.FirstOrDefault((IPlayer player) => player.Name!.ToLower().Equals(userinfo) 
                                                                            || player.Email!.Equals(userinfo));
        if (player != null)
        {
            password = CountHash(password);
            if (password.Equals(player.Password))
            {
                Id = player.Id;
                Name = player.Name;
                Password = player.Password;
                Email = player.Email;
                Cash = player.Cash;
                isAuthenticated = true;
            }
        }
        return isAuthenticated;
    }
    public bool Identification(string? username, string? password, string? email = "")
    {
        if (Casino.DB.Players.Any((IPlayer player) => player.Name!.Equals(username) 
                                                   || player.Email!.Equals(email)))
            return false;       // it means that we have this user in the database

        Name = username;
        Password = CountHash(password);
        Email = email;
        Cash = Casino.initCash;
        return true;
    }

    public void PrintInfo()
    {
        Console.WriteLine($"{Name} : {Email} : {Password} : {Cash}");
    }

    public void Update(object o)
    {
        if(o is HoldemDealer.BetInfo Hud)
        {
            Console.Clear();
            MessageBuilder builder = new MessageBuilder();
            builder.AddText($"TABLE OF PLAYER \"{Name}\":\n");
            builder.AddText("=====================================================\n");
            builder.AddText($"Bank: {Hud.bank,5:C2}\n");
            ConsoleColor betColor = Console.ForegroundColor;
            if (Hud.maxBet != Bet)
                betColor = ConsoleColor.Green;
            builder.AddText("Current Maximum bet: ");
            builder.AddTextWithColor($"{Hud.maxBet,5:C2}\n", betColor);

            builder.AddText($"Cash: {Cash,5:C2}\n");
            builder.AddText($"Your bet: {Bet,5:C2}\n");
            if (Hand is IPoker.Card[] hand)
            {
                builder.AddTextWithColor("\nHand: ", Console.ForegroundColor);
                foreach (var card in hand)
                {
                    ConsoleColor clr;
                    if (Hud.lobby.isRed(card))
                        clr = ConsoleColor.Red;
                    else
                        clr = ConsoleColor.White;
                    builder.AddTextWithColor(Hud.lobby.PrintCard(card) + " ", clr);
                }
                builder.AddText("\n");
            }
            builder.GetMessage().Render();
            Hud.tableProjection.Render();
            Console.WriteLine();
        }   
        else if (o is HoldemDealer.WinnerInfo info)
        {
            Console.WriteLine(info.message);
            if (!info.winner)
                Cash -= Bet;
            else
                Cash += info.bank;            
        }
    }
}


