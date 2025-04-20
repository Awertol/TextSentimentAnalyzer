using Spectre.Console;
using TextSentimentAnalyzer;
using static System.Net.Mime.MediaTypeNames;

bool running = true;
var language = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Zvol [green]jazyk[/] / Choose [green]language[/]:")
                .AddChoices(new[] { "Čeština", "English" }));

var table = new Table();
table.Border = TableBorder.Rounded;
table.AddColumn(language == "Čeština" ? "Kód" : "Code");
table.AddColumn(language == "Čeština" ? "Jazyk" : "Language");

table.AddRow("EN", "English");
table.AddRow("ES", "Spanish");
table.AddRow("PT", "Portuguese");
table.AddRow("DE", "German");
table.AddRow("PL", "Polish");
table.AddRow("FR", "French");
table.AddRow("NL", "Dutch");
table.AddRow("DA", "Danish");
table.AddRow("IT", "Italian");
table.AddRow("CS", "Czech");
table.AddRow("SK", "Slovak");
table.AddRow("HU", "Hungarian");

while (running == true)
{
    AnsiConsole.Clear(); //vymazání konzole vždy když cyklus znovu začne

    if (language == "Čeština")
    {
        AnsiConsole.MarkupLine("[bold green]Vítej v Texto Analyzátoru TM.[/]");
        AnsiConsole.WriteLine("Jazyky, na kterých se model učil:");
    }
    else
    {
        AnsiConsole.MarkupLine("[bold green]Welcome to Text Analyzer TM.[/]");
        AnsiConsole.WriteLine("Languages the model was trained on:");
    }

    AnsiConsole.Write(table);

    //Nabídka možností - menu
    var option = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title(language == "Čeština" ? "[underline]Vyber možnost - použij šipky na klávesnici:[/]" : "[underline]Choose an option - use arrow keys:[/]")
            .AddChoices(new[]
            {
                    language == "Čeština" ? "Analyzovat text" : "Analyze text",
                    language == "Čeština" ? "Info o aplikaci" : "App info",
                    language == "Čeština" ? "Zobrazit dataset" : "View dataset",
                    language == "Čeština" ? "Ukončit aplikaci" : "Exit application"
            }));
    switch (option)
    {
        case "Analyzovat text" or "Analyze text":
            AnsiConsole.MarkupLine($"[bold green]{(language == "Čeština" ? "Zadejte text pro analýzu. [/]" : "Enter text for analysis[/]")}");
            Console.Write("-> ");
            string inputedText = Console.ReadLine();
            inputedText = inputedText.Replace('.',' ').Trim();
            inputedText = inputedText.Replace('?', ' ').Trim();
            inputedText = inputedText.Replace('!', ' ').Trim();
            var sampleData = new TextSentimentModel.ModelInput()
            {
                Col0 = inputedText,
            };

            //Load model and predict output
            var result = TextSentimentModel.Predict(sampleData);
            AnsiConsole.MarkupLine($"[bold underline magenta]{(language == "Čeština" ? "\t\t\t\t  ODHAD [/]" : "\t\t\t\tPREDICTION [/]")}");
            string prediction = result.PredictedLabel == "Pozitivní" ? (language == "Čeština" ? "  Pozitivní" : "  Positive") : (language == "Čeština" ? "  Negativní" : "  Negative");
            AnsiConsole.WriteLine("\t\t\t      " + prediction + "\n");

            float maxScore = Math.Max(result.Score[0], result.Score[1]);
            AnsiConsole.Write(new BarChart()
            .Width(60)
            .Label(language == "Čeština" ? "[bold underline cyan3]JISTOTA (0 - 100 %)[/]" : "[bold underline cyan3]CERTAINTY (0 - 100 %)[/]")
            .AddItem(prediction, (Convert.ToDouble(maxScore.ToString("0.00")) * 100), Color.Yellow));

            AnsiConsole.MarkupLine("[deepskyblue1]--------------------------------------------------------------------[/]");
            AnsiConsole.MarkupLine($"[bold blue]{(language == "Čeština" ? "Zmáčkni klávesu pro návrat do menu[/]" : "Press any button to return to the menu[/]")}");
            Console.ReadKey();
            break;
        case "Info o aplikaci" or "App info":
            AnsiConsole.Clear();
            Panel panel = new Panel("");
            if(language == "Čeština")
            {
                panel = new Panel(@"
                [bold]Aplikace: Texto analyzátor[/]
                [bold]Verze: 1.0.0[/]
                [bold]Autor: Michal Helgert[/]
                [bold]Popis: Konzolová aplikace pro analýzu sentimentu textu - zdali je pozitivní či negativní[/]

                [bold underline cyan]Technologie: [/]
                  C#  
                  .NET 8  
                  knihovna Spectre.Console - uživatelské rozhraní
                  knihovna ML.NET  - trénování dat")
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1, 1, 1),
                }.Header("[bold underline steelblue1] O aplikaci [/]", Justify.Center);
            }
            else
            {
                panel = new Panel(@"
                [bold]Aplikace: Text Analyzer TM[/]
                [bold]Version: 1.0.0[/]
                [bold]Author: Michal Helgert[/]
                [bold]Description: A console application for simple text sentiment analysis[/]

                [bold underline cyan]Technologies: [/]
                  C#  
                  .NET 8  
                  Spectre.Console - user interface library
                  ML.NET - library for training data")
                {
                    Border = BoxBorder.Rounded,
                    Expand = true,
                    Padding = new Padding(1, 1, 1, 1),
                }.Header("[bold underline steelblue1] About app [/]", Justify.Center);
            }

            AnsiConsole.Write(panel);
            AnsiConsole.MarkupLine("[deepskyblue1]--------------------------------------------------------------------[/]");
            AnsiConsole.MarkupLine($"[bold blue]{(language == "Čeština" ? "Zmáčkni klávesu pro návrat do menu[/]" : "Press any button to return to the menu[/]")}");
            Console.ReadKey();
            break;
        case "Zobrazit dataset" or "View dataset":
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[deepskyblue1]DATASET: [/]");
            LoadCSVTable();
            AnsiConsole.MarkupLine("[deepskyblue1]--------------------------------------------------------------------[/]");
            AnsiConsole.MarkupLine($"[bold blue]{(language == "Čeština" ? "Zmáčkni klávesu pro návrat do menu[/]" : "Press any button to return to the menu[/]")}");
            Console.ReadKey();
            break;
        case "Ukončit aplikaci" or "Exit application":
            running = false;
            Thread.Sleep(200);
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold green]Byeeee! :)[/]");
            Thread.Sleep(2500);
            break;
        default:
            break;
    }
}

void LoadCSVTable()
{
    string filePath = "dataset_vety_prefinal.csv"; 

    if (!File.Exists(filePath))
    {
        AnsiConsole.MarkupLine("[red]Soubor nebyl nalezen.[/]");
        return;
    }

    var table = new Table();
    table.Border = TableBorder.Rounded;
    table.AddColumn("Věta");
    table.AddColumn("Hodnocení");

    foreach (var line in File.ReadLines(filePath))
    {
        if (string.IsNullOrWhiteSpace(line))
            continue;

        // Oddělí sloupce podle čárky
        var parts = SplitCsvLine(line);

        if (parts.Length >= 2)
        {
            table.AddRow(parts[0], parts[1]);
        }
    }

    AnsiConsole.Write(table);
}
string[] SplitCsvLine(string line)
{
    if (line.StartsWith("\""))
    {
        int commaIndex = line.LastIndexOf("\",");
        if (commaIndex != -1)
        {
            string sentence = line.Substring(1, commaIndex - 1);
            string label = line.Substring(commaIndex + 2).Trim();
            return new[] { sentence, label };
        }
    }

    return line.Split(',');
}
