using email_checker;



Task.Run(async () => await Checker.MainAsync(args)).Wait();

Console.WriteLine("************************************* ENDED *************************************");
Console.ReadLine();