MontyHallSim();

void MontyHallSim()
{
    bool playAgain = true;
    while (playAgain)
    {
        Play();
        while (true)
        {
            Console.Write("Play again? (Y/N): ");

            string? input = Console.ReadLine()?.Trim().ToLower();
            if (input is "y" or "n")
            {
                playAgain = input == "y";
                break;
            }
            else
                Console.WriteLine("Invalid input.\n");
        }
    }
}

void Play()
{
    int simCount;
    bool switchDoors;
    int gamesWon = 0;

    while (true)
    {
        Console.Write("How many simulations to run? (Max: 100,000,000): ");

        if ((int.TryParse(Console.ReadLine(), out simCount)
            && simCount is not < 1 and not > 100_000_000))
            break;
        else
            Console.WriteLine("Invalid input.\n");
    }

    while (true)
    {
        Console.Write("Switch doors? (Y/N): ");

        string? input = Console.ReadLine()?.Trim().ToLower();
        if (input is "y" or "n")
        {
            switchDoors = input == "y";
            break;
        }
        else
            Console.WriteLine("Invalid input.\n");
    }

    for (int i = 0; i < simCount; i++)
        if (WonGame(switchDoors)) gamesWon++;

    ShowResult(simCount, gamesWon);
}

bool WonGame(bool switchDoors)
{
    int winningDoor = Random.Shared.Next(0, 3);
    int playerDoor = Random.Shared.Next(0, 3);
    int revealedDoor = 0;

    while (revealedDoor == playerDoor || revealedDoor == winningDoor)
        revealedDoor++;

    if (switchDoors)
        playerDoor = 3 - revealedDoor - playerDoor;

    return playerDoor == winningDoor;
}

void ShowResult(int simCount, int gamesWon)
{
    double winRatio = (double)gamesWon / simCount;
    Console.WriteLine($"\nYOU WON {winRatio:P2} OF GAMES.\n");
}