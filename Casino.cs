using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public class Casino
{
    public const decimal minBet = 10;
    public const decimal initCash = 1000;
    public string Name = "";
    public Dictionary<string, IPlayer> players = new Dictionary<string, IPlayer>();

    public CasinoContext DB = new CasinoContext();
    public Displayer mainMenu = new Displayer("Main menu:");
    public Displayer loginTerminal = new Displayer("LOGIN");
    public Casino()
    {
        mainMenu.Add("root", "texasHoldem",     new MenuItem("1. Start texas Holdem"));
        mainMenu.Add("root", "back to login",   new MenuItem("2. Back to login"));
        mainMenu.Add("root", "exit",            new MenuItem("0. Exit"));
        // mainMenu.Display();
        loginTerminal.Add("root", "authentization", new MenuPoint("1. Log In"));
        loginTerminal.Add("root", "Identification", new MenuPoint("2. Sign In"));
        loginTerminal.Add("root", "Go in casino",   new MenuPoint("3. Welcome to casino!"));
    }
    public delegate bool authenticationEventHandler();
    public void PlayersAuthentication()
    {
        int actionId = 0;
        Action[] events = { _playerAuthentication, _playerIdentification, () => { Console.WriteLine("Open doors..."); } };
        while(actionId != 3)
        {
            try
            {
                loginTerminal.Display();
                actionId = loginTerminal.getUserInput.Invoke("Enter: ");
                events[actionId - 1].Invoke();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Incorrect input");
                Console.WriteLine(ex.ToString());
            }
        }
        Console.Clear();
    }
    public void Launch()
    {
        Action[] events = { () => { Console.WriteLine("Goodbye!!! We will be waiting for you again."); },
                                    _startTexasHoldem,
                                    PlayersAuthentication};
        int actionId = 0;
        do
        {
            mainMenu.Display();
            actionId = mainMenu.getUserInput.Invoke("Enter: ");
            events[actionId].Invoke();
        } while (actionId != 0);
    }

    private void _startTexasHoldem()
    {
        if (players.Count >= 2)
        {
            TexasHoldemCreator creator = new TexasHoldemCreator("TexasHoldemCreator");
            TexasHoldem holdem = creator.Create();
            holdem.AddCasino(this);
            foreach (Player player in players.Values)
            {
                player.currentGame = holdem;
                holdem.Players.Add(player);
            }
            holdem.StartGame();

            if (holdem.isFinished)
            {
                foreach (Player player in players.Values)
                {
                    _playerUpdateCash(player);
                }
            }
        }
        else
        {
            Console.WriteLine("To play Texas holdem need more than one player, please wait others");
        }
    }

    private void _playerUpdateCash(Player player)
    {
        var dbPlayer = DB.Players.Find(player.Id);
        if (dbPlayer != null)
        {
            dbPlayer.Cash = player.Cash;
            DB.Players.Update(dbPlayer);
            DB.SaveChanges();
        }
    }
    private void _playerAuthentication()
    {
        Console.WriteLine("Log In");
        Player player = new Player(this);
        string? name, password, email;
        (name, password, email) = _getUserInput();
        if (player.Authentication(name, password) && !players.ContainsKey(player.Name!))
        {
            Console.WriteLine("Welcome!!!\n \rWe are glad to see you again {0}", name);
            players.Add(player.Name!, player);
        }
        else if (players.ContainsKey(name))
            Console.WriteLine("Such user already available to enter casino");
        else
            Console.WriteLine("Undefiend user.\n \rPlease, try to sign in or check your password.");
    }
    private void _playerIdentification()
    {
        Console.WriteLine("Sign In");
        Player player = new Player(this);
        string? name, password, email;
        (name, password, email) = _getUserInput();
        if (player.Identification(name, password))
        {
            Console.WriteLine("Welcome to our game {0}!!!", name);
            DB.Add(player);
            DB.SaveChanges();
        }
        else
            Console.WriteLine("Such user in database, try to log in please.");
    }
    private (string?, string?, string?) _getUserInput()
    {
        Console.Write("Enter name: ");
        string? name = Console.ReadLine();
        Console.Write("Enter password: ");
        string? password = ReadPassword();
        Console.WriteLine();
        Console.Write("Enter email: ");
        string? email = Console.ReadLine();
        return (name, password, email);
    }

    protected string ReadPassword()
    {
        StringBuilder password = new StringBuilder();
        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && password.Length > 0)
                password.Remove(password.Length - 1, 1);
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                //Console.Write("");
                password.Append(keyInfo.KeyChar);
            }
            
        } while (key != ConsoleKey.Enter);
        return password.ToString();
    }
}
