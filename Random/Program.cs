using System.CommandLine;

namespace random;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var seedOption =
            new Option<int?>(aliases: ["--seed", "-s"], description: "The seed to use for the random number generator.")
            {
                ArgumentHelpName = "integer"
            };

        var minOption =
            new Option<string>(aliases: ["--min", "-m"],
                               description: "The minimum value for the random number generator.",
                               getDefaultValue: () => "0")
            {
                ArgumentHelpName = "number"
            };

        var maxOption =
            new Option<string>(aliases: ["--max", "-x"],
                               description: "The maximum value for the random number generator.",
                               getDefaultValue: () => "1.0")
            {
                ArgumentHelpName = "number"
            };

        var countOption =
            new Option<int>(aliases: ["--count", "-c"], description: "The number of random numbers to generate.",
                            getDefaultValue: () => 1)
            {
                ArgumentHelpName = "integer"
            };

        var formatOption =
            new Option<string?>(aliases: ["--format", "-f"],
                                description: """
                                             The format to use for the random numbers. 
                                             See also: 
                                             https://learn.microsoft.com/dotnet/api/system.string.format
                                             """)
            {
                ArgumentHelpName = "string"
            };

        var rootCommand = new RootCommand("""
                                          Random number generator.
                                            Generates random numbers in [min, max].
                                            If the minimum and maximum values are integers, the random numbers will be integers,
                                            otherwise they will be doubles.
                                          """)
        {
            seedOption,
            minOption,
            maxOption,
            countOption,
            formatOption
        };

        rootCommand.SetHandler(GenRandomNumber, seedOption, minOption, maxOption, countOption, formatOption);
        return await rootCommand.InvokeAsync(args);
    }


    private static Task GenRandomNumber(int? seed, string min, string max, int count, string? format)
    {
        var isInteger = !(min.Contains('.') || max.Contains('.'));

        // check is number
        if (!double.TryParse(min, out var minValue))
        {
            Console.WriteLine($"Invalid minimum value: {min}");
            return Task.CompletedTask;
        }

        if (!double.TryParse(max, out var maxValue))
        {
            Console.WriteLine($"Invalid maximum value: {max}");
            return Task.CompletedTask;
        }

        if (minValue > maxValue)
        {
            Console.WriteLine($"Invalid minimum value: {min}");
            return Task.CompletedTask;
        }

        var random = seed.HasValue ? new Random(seed.Value) : new Random();

        for (var i = 0; i < count; i++)
        {
            if (isInteger)
            {
                var l = random.NextInt64((long)minValue, (long)maxValue + 1);
                if (format == null)
                    Console.WriteLine(l);
                else
                    Console.WriteLine($"{{0:{format}}}", l);
            }
            else
            {
                var d = random.NextDouble() * (maxValue - minValue) + minValue;
                if (format == null)
                    Console.WriteLine(d);
                else
                    Console.WriteLine($"{{0:{format}}}", d);
            }
        }

        return Task.CompletedTask;
    }
}