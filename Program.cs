MontyHallSim.GameLoop();

public static class MontyHallSim
{
    public static void GameLoop()
    {
        (int gamesPlayed, int gamesWon) totals = (0, 0);

        bool playAgain = true;
        while (playAgain)
        {
            (int gamesPlayed, int gamesWon) result = Play();
            totals.gamesPlayed += result.gamesPlayed;
            totals.gamesWon += result.gamesWon;

            double winRatio = (double)totals.gamesWon / totals.gamesPlayed;

            Console.WriteLine($"TOTAL GAMES PLAYED: {totals.gamesPlayed}");
            Console.WriteLine($"TOTAL WIN RATIO: {winRatio:P2}\n");
            
            playAgain = Confirm("Play again? (Y/N): ");

            if (playAgain && Confirm("Reset totals? (Y/N): "))
            {
                totals.gamesPlayed = 0;
                totals.gamesWon = 0;
            }
            
            Console.WriteLine();
        }
    }

    static (int gamesPlayed, int gamesWon) Play()
    {
        int simCount = 0;
        bool singleGame = false;
        bool switchDoors = false;
        int gamesWon = 0;

        Console.Clear();
        simCount = InputNumberWithinRange("How many simulations to run?" 
            + "\nEnter 1 for a single game with step-by-step execution."
            + "\nOr enter a number between 2 and 100,000,000 to run a batch test: ", 1, 100_000_000);
        singleGame = simCount == 1;
        
        if(!singleGame)
            switchDoors = Confirm("Switch doors? (Y/N): ");
        
        for (int i = 0; i < simCount; i++)
        {
            if (WonGame(switchDoors, singleGame))
                gamesWon++;
        }
        
        ShowResult(simCount, singleGame, gamesWon);
        return (simCount, gamesWon);
    }

    static bool WonGame(bool switchDoors, bool singleGame)
    {
        /*
        Of course, the most efficient way is:

        {
            int playerDoor = Random.Shared.Next(0, 3);
            if (switchDoors)
                return (playerDoor is 0 or 1);
            else return (playerDoor is 0);
        }

        But that assumes we already trust the maths.
        So instead we run each individual step as a proof. :)
        */

        int winningDoor = Random.Shared.Next(0, 3);
        int playerDoor = 0;
        (int doorA, int doorB) losingDoors;
        int revealedDoor = 0;

        if (singleGame)
        {
            Console.Clear();
            playerDoor = InputNumberWithinRange("Choose a door (1, 2, or 3): ", 1, 3) - 1;
            Console.Clear();
            Console.WriteLine($"You chose door {playerDoor + 1}.");
            Wait(2000);
        }
        else
            playerDoor = Random.Shared.Next(0, 3);

        if (playerDoor == winningDoor)
        {
            losingDoors = winningDoor switch
            {
                0 => (1, 2),
                1 => (0, 2),
                _ => (0, 1)
            };
            int r = Random.Shared.Next(0, 2);
            revealedDoor = r == 0 ? losingDoors.doorA : losingDoors.doorB;
        }
        else
            revealedDoor = 3 - winningDoor - playerDoor;

        if (singleGame)
        {
            Console.WriteLine($"Host opens door {revealedDoor + 1}. There's a goat behind it!");
            Wait(2000);
        }

        if (singleGame)
        {
            switchDoors = Confirm("Would you like to switch doors? (Y/N): ");
            if (switchDoors)
            {
                playerDoor = 3 - revealedDoor - playerDoor;
                Console.WriteLine($"\nYou switched to door {playerDoor + 1}.");
            }
            else
                Console.WriteLine($"\nYou stuck with door {playerDoor + 1}.");

            Wait(2000);
            Console.Write($"The host is opening door {playerDoor + 1}");

            for (int i = 0; i < 3; i++)
            {
                Wait(750);
                Console.Write(".");
                Wait(750);
            }
            Wait(1000);
        }
        else if (switchDoors)
            playerDoor = 3 - revealedDoor - playerDoor;

        return playerDoor == winningDoor;
    }

    static void ShowResult(int simCount, bool singleGame, int gamesWon)
    {
        Console.Clear();
        if (singleGame)
        {
            Console.WriteLine($"You {(gamesWon == 1 ? "won a car!" : "lost. Enjoy your goat.")}");
            DrawPrize(gamesWon);
        }
        else
        {
            double winRatio = (double)gamesWon / simCount;
            Console.WriteLine($"YOU WON {winRatio:P2} OF GAMES.\n");
        }
    }

    static int InputNumberWithinRange(string message, int min, int max)
    {
        while (true)
        {
            Console.Write(message);
            if (int.TryParse(Console.ReadLine(), out int input)
            && input >= min && input <= max)
                return input;
            else
                Console.WriteLine("Invalid input.\n");
        }
    }

    static bool Confirm(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine()?.Trim().ToLower();
            if (input is "y" or "n")
                return input == "y";
            else
                Console.WriteLine("Invalid input.\n");
        }
    }

    static void DrawPrize(int gamesWon)
    {
        if (gamesWon == 1)
        {
            Console.WriteLine();
            Console.WriteLine(@"   ______");
            Console.WriteLine(@"  /|_||_\`.__");
            Console.WriteLine(@" (   _    _ _\");
            Console.WriteLine(@" =`-(_)--(_)-'");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine(@"  //\\   //\\");
            Console.WriteLine(@" ((  \_//  ))");
            Console.WriteLine(@"  \  o o  /");
            Console.WriteLine(@"   (  =  )");
            Console.WriteLine(@"    `---'");
            Console.WriteLine();
        }
    }

    static void Wait(int ms) => Thread.Sleep(ms);
}