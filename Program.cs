MontyHallSim.GameLoop();

public static class MontyHallSim
{
    const int k_TextDelay = 10;
    
    public static void GameLoop()
    {
        (int gamesPlayed, int gamesWon, int switchCount) totals = (0, 0, 0);

        Console.CursorVisible = false;

        bool playAgain = true;
        while (playAgain)
        {
            (int gamesPlayed, int gamesWon, int switchCount) result = Play();
            totals.gamesPlayed += result.gamesPlayed;
            totals.gamesWon += result.gamesWon;
            totals.switchCount += result.switchCount;

            double switchRatio = (double)totals.switchCount / totals.gamesPlayed;
            double winRatio = (double)totals.gamesWon / totals.gamesPlayed;
            
            Write($"TOTAL GAMES PLAYED: {totals.gamesPlayed}");
            Write($"TOTAL SWITCH RATIO: {switchRatio:P2}");
            Write($"TOTAL WIN RATIO: {winRatio:P2}", 2);

            playAgain = Confirm("Play again? (Y/N): ");

            if (playAgain && Confirm("Reset totals? (Y/N): "))
            {
                totals.gamesPlayed = 0;
                totals.gamesWon = 0;
                totals.switchCount = 0;
            }
        }
    }

    static (int gamesPlayed, int gamesWon, int switchCount) Play()
    {
        int simCount = 0;
        bool singleGame = false;
        bool switchDoors = false;
        (int gamesWon, int switchCount) roundTotals = (0, 0);

        Console.Clear();
        simCount = InputNumberWithinRange(
            1, 100_000_000,
            "How many simulations to run?", 
            "Enter 1 for a single game with step-by-step execution.",
            "Or enter a number between 2 and 100,000,000 to run a batch test: ");
        
        singleGame = simCount == 1;
        
        if (!singleGame)
            switchDoors = Confirm("Switch doors? (Y/N): ");

        Console.Clear();
        Console.WriteLine($"Simulating {simCount} games with strategy: {(switchDoors ? "Switch" : "Stay")}");

        for (int i = 0; i < simCount; i++)
        {
            (bool won, bool switched) result = (WonGame(switchDoors, singleGame));
            roundTotals.gamesWon += result.won ? 1 : 0;
            roundTotals.switchCount += result.switched ? 1 : 0;
        }
        
        ShowResult(simCount, singleGame, roundTotals.gamesWon);
        return (simCount, roundTotals.gamesWon, roundTotals.switchCount);
    }

    static (bool won, bool switched) WonGame(bool switchDoors, bool singleGame)
    {
        /*
        Of course, the most efficient way to work out the result is:

        {
            int playerDoor = Random.Shared.Next(0, 3);
            if (switchDoors)
                won = (playerDoor is 0 or 1);
            else
                won = (playerDoor is 0);
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
            playerDoor = InputNumberWithinRange(1, 3, "Choose a door (1, 2, or 3): ") - 1;
            Console.Clear();
            Write($"You chose door {playerDoor + 1}.");
            Wait(2000);
            Console.WriteLine();
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
            Write($"Host opens door {revealedDoor + 1}. There's a goat behind it!");
            Console.WriteLine();
            Wait(2000);
        }

        if (singleGame)
        {
            switchDoors = Confirm("Would you like to switch doors? (Y/N): ");
            if (switchDoors)
            {
                playerDoor = 3 - revealedDoor - playerDoor;
                Console.WriteLine();
                Write($"You switched to door {playerDoor + 1}.");
            }
            else
            {
                Console.WriteLine();
                Write($"You stuck with door {playerDoor + 1}.");
            }

            Wait(2000);
            Console.WriteLine();
            Write($"The host is opening door {playerDoor + 1}", 0);

            for (int i = 0; i < 3; i++)
            {
                Console.Write(".");
                Wait(1000);
            }
            Wait(1000);
        }
        else if (switchDoors)
            playerDoor = 3 - revealedDoor - playerDoor;

        return ((playerDoor == winningDoor), switchDoors);
    }

    static void ShowResult(int simCount, bool singleGame, int gamesWon)
    {
        Console.Clear();
        if (singleGame)
        {
            Write($"You {(gamesWon == 1 ? "won a car!" : "lost. Enjoy your goat.")}");
            Wait(2000);
            DrawPrize(gamesWon);
            Wait(2000);
        }
        else
        {
            double winRatio = (double)gamesWon / simCount;
            Write($"YOU WON {winRatio:P2} OF GAMES THAT ROUND.\n");
        }
    }

    static int InputNumberWithinRange(int min, int max, string? message1 = null, string? message2 = null, string? message3 = null)
    {
        while (true)
        {
            if (message1 != null)
                Write(message1, 0);
            if (message2 != null)
            {
                Console.WriteLine();
                Write(message2, 0);
            }
            if (message3 != null)
            {
                Console.WriteLine();
                Write(message3, 0);
            }

            Console.CursorVisible = true;
            if (int.TryParse(Console.ReadLine(), out int input)
            && input >= min && input <= max)
            {
                Console.CursorVisible = false;
                return input;
            }
            else
            {
                Write("Invalid input.");
                Console.WriteLine();
            }
        }
    }

    static bool Confirm(string? message1 = null, string? message2 = null, string? message3 = null)
    {
        Console.CursorVisible = true;
        while (true)
        {
            if (message1 != null)
                Write(message1, 0);
            if (message2 != null)
            {
                Console.WriteLine();
                Write(message2, 0);
            }
            if (message3 != null)
            {
                Console.WriteLine();
                Write(message3, 0);
            }

            Console.CursorVisible = true;
            string? input = Console.ReadLine()?.Trim().ToLower();
            if (input is "y" or "n")
            {
                Console.CursorVisible = false;
                return input == "y";
            }
            else
            {
                Write("Invalid input.");
            }
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

    static void Write(string text, int newLine = 1)
    {
        char[] chars = text.ToCharArray();
        
        Console.CursorVisible = false;

        foreach (var c in chars)
        {
            Console.Write(c);
            Wait(k_TextDelay);
        }
        
        for (int i = 0; i < newLine; i++)
            Console.WriteLine();
    }
    
    static void Wait(int ms) => Thread.Sleep(ms);
}