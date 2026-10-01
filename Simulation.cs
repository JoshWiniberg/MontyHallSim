// Monty Hall simulation to prove that switching doors is the better strategy.
// Console version for now, logic will be extracted for web version later.

using static MontyHallSim.Helpers;
namespace MontyHallSim;

public static class Simulation
{
    // PRIZES - 0 = Goat, 1 = Car
    static readonly string[][] prizes = new string[][]
    {
        new string[]
        {
            @"  //\\   //\\",
            @" ((  \_//  ))",
            @"  \  o o  /",
            @"   (  =  )",
            @"    `---'"
        },
        new string[]
        {
            @"   ______",
            @"  /|_||_\`.__",
            @" (   _    _ _\",
            @" =`-(_)--(_)-'",
        }
    };



    // MAIN LOOP
    public static void RunGame()
    {
        (int gamesPlayed, int gamesWon, int switchCount) totals = (0, 0, 0);

        Console.CursorVisible = false;

        bool playAgain = true;
        while (playAgain)
        {
            // Play round, get results
            (int gamesPlayed, int gamesWon, int switchCount) result = Play();

            // End round
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
                totals = (0, 0, 0);
        }
    }



    // GET USER CONFIG AND FIRE THE SIMULATION STEPS
    static (int gamesPlayed, int gamesWon, int switchCount) Play()
    {
        (int gamesWon, int switchCount) roundTotals = (0, 0);

        Console.Clear();

        int simCount = InputNumberWithinRange(
            1, 100_000_000,
            "How many simulations to run?",
            "Enter 1 for a single game with step-by-step execution.",
            "Or enter a number between 2 and 100,000,000 to run a batch test: ");

        if (simCount > 1) // Batch test mode
        {
            bool switchDoors = Confirm("Switch doors? (Y/N): ");

            Console.Clear();
            Console.WriteLine($"Simulating {simCount} games with strategy: {(switchDoors ? "Switch" : "Don't Switch")}");

            for (int i = 0; i < simCount; i++)
            {
                (bool won, bool switched) result = (BatchMode(switchDoors));
                roundTotals.gamesWon += result.won ? 1 : 0;
                roundTotals.switchCount += result.switched ? 1 : 0;
            }
        }
        else // Single game mode
        {
            (bool won, bool switched) result = SingleGame();
            roundTotals.gamesWon += result.won ? 1 : 0;
            roundTotals.switchCount += result.switched ? 1 : 0;
        }

        ShowResult(simCount, roundTotals.gamesWon);
        return (simCount, roundTotals.gamesWon, roundTotals.switchCount);
    }


    // SINGLE GAME MODE
    static (bool won, bool switched) SingleGame()
    {
        int winningDoor = Random.Shared.Next(0, 3);

        Console.Clear();

        int playerDoor = InputNumberWithinRange(1, 3, "Choose a door (1, 2, or 3): ") - 1;

        Console.Clear();
        Write($"You chose door {playerDoor + 1}.");
        Wait(2000);
        Console.WriteLine();

        int revealedDoor = RevealDoor(playerDoor, winningDoor);

        Write($"Host opens door {revealedDoor + 1}. There's a goat behind it!");
        Console.WriteLine();
        Wait(2000);

        bool switchDoors = Confirm("Would you like to switch doors? (Y/N): ");
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

        return ((playerDoor == winningDoor), switchDoors);
    }



    // BATCH MODE
    static (bool won, bool switched) BatchMode(bool switchDoors)
    {
        int winningDoor = Random.Shared.Next(0, 3);
        int playerDoor = Random.Shared.Next(0, 3);

        int revealedDoor = RevealDoor(playerDoor, winningDoor);

        if (switchDoors)
            playerDoor = 3 - revealedDoor - playerDoor;

        return ((playerDoor == winningDoor), switchDoors);
    }



    // REVEAL A DOOR THAT IS NOT THE PLAYER'S CHOICE OR THE WINNING DOOR
    static int RevealDoor(int playerDoor, int winningDoor)
    {
        (int doorA, int doorB) losingDoors;

        if (playerDoor == winningDoor)
        {
            losingDoors = winningDoor switch
            {
                0 => (1, 2),
                1 => (0, 2),
                _ => (0, 1)
            };
            int r = Random.Shared.Next(0, 2);
            return r == 0 ? losingDoors.doorA : losingDoors.doorB;
        }
        else
            return 3 - winningDoor - playerDoor;
    }



    // DISPLAY THE RESULTS OF THE SIMULATION ROUND
    static void ShowResult(int simCount, int gamesWon)
    {
        Console.Clear();

        if (simCount == 1)
        {
            Write($"You {(gamesWon == 1 ? "won a car!" : "lost. Enjoy your goat.")}");
            Wait(2000);

            Console.CursorVisible = false;
            Console.WriteLine();
            
            foreach (var line in prizes[gamesWon])
                Console.WriteLine(line);
            
            Console.WriteLine();
            Wait(2000);
        }
        else
        {
            double winRatio = (double)gamesWon / simCount;
            Write($"YOU WON {winRatio:P2} OF GAMES THAT ROUND.\n");
        }
    }



    static void Wait(int ms) => Thread.Sleep(ms);
}
