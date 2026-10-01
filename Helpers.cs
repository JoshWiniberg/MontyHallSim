namespace MontyHallSim;

public static class Helpers
{
    const int k_TextDelay = 10;



    // WRITING METHODS
    public static void Write(string text, int newLine = 1)
    {
        char[] chars = text.ToCharArray();

        Console.CursorVisible = false;

        try
        {
            foreach (var c in chars)
            {
                Console.Write(c);
                Wait(k_TextDelay);
            }

            for (int i = 0; i < newLine; i++)
                Console.WriteLine();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }

    static void WritePrompts(params string[] messages)
    {
        try
        {
            for (int i = 0; i < messages.Length; i++)
            {
                if (i < messages.Length - 1)
                    Write(messages[i], 1);
                else
                    Write(messages[i], 0);
            }
        }
        catch (Exception e)
        {
            Write($"Error: {e.Message}");
        }
    }



    // INPUT METHODS
    public static int InputNumberWithinRange(int min, int max, params string[] messages)
    {
        while (true)
        {
            WritePrompts(messages);

            Console.CursorVisible = true;
            if (int.TryParse(Console.ReadLine(), out int input)
            && input >= min && input <= max)
            {
                Console.CursorVisible = false;
                return input;
            }
            else
            {
                Write("Invalid input.", 2);
                Console.WriteLine();
            }
        }
    }

    public static bool Confirm(params string[] messages)
    {
        while (true)
        {
            WritePrompts(messages);

            Console.CursorVisible = true;
            string? input = Console.ReadLine()?.Trim().ToLower();
            if (input is "y" or "n")
            {
                Console.CursorVisible = false;
                return input == "y";
            }
            else
            {
                Write("Invalid input.", 2);
            }
        }
    }



    static void Wait(int ms) => Thread.Sleep(ms);
}
