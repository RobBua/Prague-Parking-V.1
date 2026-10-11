class Program
{

    static string[] parkering = new string[100];

    static void Main()
    {

        for (int i = 0; i < 100; i++)
        {
            parkering[i] = "";
        }


        string[] menyval = new string[6];
        menyval[0] = "Ta emot fordon";
        menyval[1] = "Flytta fordon";
        menyval[2] = "Hämta ut fordon";
        menyval[3] = "Sök fordon";
        menyval[4] = "Visa alla upptagna platser";
        menyval[5] = "Avsluta";

        // Snurran körs tills användaren väljer "Avsluta" (då blir kör false)
        bool kör = true;
        while (kör)
        {
            // VisaMeny visar menyn och ger tillbaka numret på det man valde
            int val = VisaMeny("=== PRAGUE PARKING ===", menyval);


            if (val == 0)
            {
                TaEmotFordon();
            }
            else if (val == 1)
            {
                FlyttaFordon();
            }
            else if (val == 2)
            {
                HamtaUtFordon();
            }
            else if (val == 3)
            {
                SokFordon();
            }
            else if (val == 4)
            {
                VisaAlla();
            }
            else if (val == 5)
            {
                kör = false; // Avslutar programmet
            }
        }
    }

    // MENY MED PILTANGENTER

    static int VisaMeny(string rubrik, string[] alternativ)
    {
        int valt = 0;


        while (true)
        {
            Console.Clear(); // rensa skärmen så att inget gammalt ligger kvar
            Console.WriteLine(rubrik);
            Console.WriteLine("Använd piltangenterna och tryck Enter");
            Console.WriteLine();

            // Skriv ut alla alternativ. Det markerade får grön färg och ett >
            for (int i = 0; i < alternativ.Length; i++)
            {
                if (i == valt)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {alternativ[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {alternativ[i]}");
                }
            }

            // true gör att tangenten inte skrivs ut på skärmen.
            ConsoleKey tangent = Console.ReadKey(true).Key;

            if (tangent == ConsoleKey.UpArrow)
            {
                valt--; // ett steg uppåt
                if (valt < 0)
                {
                    valt = alternativ.Length - 1; // hoppa ner till sista alternativet
                }
            }
            else if (tangent == ConsoleKey.DownArrow)
            {
                valt++; // ett steg nedåt
                if (valt >= alternativ.Length)
                {
                    valt = 0; // hoppa upp till första alternativet
                }
            }
            else if (tangent == ConsoleKey.Enter)
            {
                Console.Clear();
                return valt;
            }
        }
    }

    // Väntar på att användaren trycker på en tangent
    static void Paus()
    {
        Console.WriteLine();
        Console.WriteLine("Tryck valfri tangent för att gå tillbaka till menyn...");
        Console.ReadKey(true);
    }

    // MENYVALEN (en metod för varje val i huvudmenyn)


    // Tar emot ett nytt fordon och talar om vilken plats det ska köras till
    static void TaEmotFordon()
    {
        // Låt användaren välja mellan Bil och MC med samma meny som ovan
        string[] typer = new string[2];
        typer[0] = "Bil";
        typer[1] = "MC";
        int typVal = VisaMeny("Vilken typ av fordon?", typer);

        // Sparar typen som "CAR" eller "MC" i parkeringsrutan
        string typ = "MC";
        if (typVal == 0)
        {
            typ = "CAR";
        }

        // Fråga efter registreringsnummer. Tom text betyder att det var ogiltigt.
        string reg = FrågaRegnr();
        if (reg == "")
        {
            return; // avbryt och gå tillbaka till menyn
        }

        // Kolla att fordonet inte redan står parkerat (-1 betyder "hittades inte")
        if (HittaFordon(reg) != -1)
        {
            Console.WriteLine($"Fordonet {reg} finns redan på parkeringen.");
            Paus();
            return;
        }

        // Leta efter en lämplig plats.
        int index = HittaLedigPlats(typ);
        if (index == -1)
        {
            Console.WriteLine("Parkeringen är full.");
            Paus();
            return;
        }

        // index + 1 eftersom användaren ser plats 1-100.
        LäggTillFordon(index, typ, reg);
        Console.WriteLine($"Kör {typ} {reg} till plats {index + 1}.");
        Paus();
    }

    // Flyttar ett fordon från en plats till en annan
    static void FlyttaFordon()
    {
        string reg = FrågaRegnr();
        if (reg == "")
        {
            return;
        }

        // Hitta var fordonet står just nu
        int från = HittaFordon(reg);
        if (från == -1)
        {
            Console.WriteLine($"Hittar inget fordon med registreringsnummer {reg}.");
            Paus();
            return;
        }

        string typ = HämtaTyp(från, reg);
        Console.WriteLine($"{typ} {reg} står på plats {från + 1}.");
        Console.Write("Ange ny plats (1-100): ");



        int till;
        bool ok = int.TryParse(Console.ReadLine(), out till); // TryParse försöker göra om texten till ett heltal.
        if (ok == false || till < 1 || till > 100)
        {
            Console.WriteLine("Ogiltigt platsnummer.");
            Paus();
            return;
        }

        // Gör om från plats 1-100 till index 0-99
        int tillIndex = till - 1;
        if (tillIndex == från)
        {
            Console.WriteLine("Fordonet står redan på den platsen.");
            Paus();
            return;
        }


        if (FårPlats(tillIndex, typ) == false)
        {
            Console.WriteLine($"Plats {till} är upptagen eller har inte utrymme för {typ}.");
            Paus();
            return;
        }

        // Flytta: ta bort från gamla platsen och lägg till på nya
        TaBortFordon(från, typ, reg);
        LäggTillFordon(tillIndex, typ, reg);
        Console.WriteLine($"Flyttade {typ} {reg} från plats {från + 1} till plats {till}.");
        Paus();
    }

    // Tar bort ett fordon när kunden hämtar ut det
    static void HamtaUtFordon()
    {
        string reg = FrågaRegnr();
        if (reg == "")
        {
            return;
        }

        int index = HittaFordon(reg);
        if (index == -1)
        {
            Console.WriteLine($"Hittar inget fordon med registreringsnummer {reg}.");
            Paus();
            return;
        }

        string typ = HämtaTyp(index, reg);
        TaBortFordon(index, typ, reg);
        Console.WriteLine($"{typ} {reg} hämtades ut från plats {index + 1}.");
        Paus();
    }

    // Söker efter ett fordon och talar om vilken plats det står på
    static void SokFordon()
    {
        string reg = FrågaRegnr();
        if (reg == "")
        {
            return;
        }

        int index = HittaFordon(reg);
        if (index == -1)
        {
            Console.WriteLine($"Fordonet {reg} finns inte på parkeringen.");
        }
        else
        {
            Console.WriteLine($"{HämtaTyp(index, reg)} {reg} står på plats {index + 1}.");
        }
        Paus();
    }

    // Skriver ut alla rutor som innehåller något
    static void VisaAlla()
    {
        bool någon = false; // håller reda på om vi hittat något fordon

        for (int i = 0; i < 100; i++)
        {
            if (parkering[i] != "")
            {
                // Byt ut # och ; mot mellanslag så att texten blir lättare att läsa
                string text = parkering[i].Replace("#", " ").Replace(";", "  ");
                Console.WriteLine($"Plats {i + 1}: {text}");
                någon = true;
            }
        }

        if (någon == false)
        {
            Console.WriteLine("Parkeringen är tom.");
        }
        Paus();
    }

    // HJÄLPMETODER

    // Frågar efter ett registreringsnummer och kontrollerar att det är giltigt.
    static string FrågaRegnr()
    {
        Console.Write("Ange registreringsnummer (max 10 tecken): ");

        // Trim() tar bort mellanslag i början/slutet.
        // ToUpper() gör om till stora bokstäver så att abc123 och ABC123 blir samma.
        string reg = Console.ReadLine().Trim().ToUpper();

        // Max 10 tecken. Tecknen # och ; används i vår lagring, så de får inte förekomma.
        if (reg.Length == 0 || reg.Length > 10 || reg.Contains("#") || reg.Contains(";"))
        {
            Console.WriteLine("Ogiltigt registreringsnummer (1-10 tecken, inga # eller ;).");
            Paus();
            return "";
        }
        return reg;
    }

    // Räknar hur många fordon som står i en ruta.
    // Varje fordon slutar på ett ;, så vi räknar antalet ;
    static int AntalFordon(string plats)
    {
        int antal = 0;
        for (int i = 0; i < plats.Length; i++)
        {
            if (plats[i] == ';')
            {
                antal++;
            }
        }
        return antal;
    }


    // Returnerar index (0-99) för rutan, eller -1 om fordonet inte finns.
    // Söker efter "#ABC123;" (med # och ;) så att ABC12 inte matchar ABC123.
    static int HittaFordon(string reg)
    {
        for (int i = 0; i < 100; i++)
        {
            if (parkering[i].Contains("#" + reg + ";"))     // Letar igenom alla rutor efter ett registreringsnummer.
            {
                return i;
            }
        }
        return -1;
    }

    // Tar reda på om fordonet i en ruta är en bil ("CAR") eller en mc ("MC")
    static string HämtaTyp(int index, string reg)
    {
        if (parkering[index].Contains("CAR#" + reg + ";"))
        {
            return "CAR";
        }
        return "MC";
    }

    // Kollar om ett fordon får plats i en viss ruta.
    // Regler: en tom ruta passar allt. En ruta med en ensam MC har plats för en MC till.
    static bool FårPlats(int index, string typ)
    {
        string plats = parkering[index];

        if (plats == "")
        {
            return true;
        }
        if (typ == "MC" && plats.StartsWith("MC#") && AntalFordon(plats) == 1)
        {
            return true;
        }
        return false;
    }

    // Hittar första lämpliga platsen för ett nytt fordon.
    // Returnerar index (0-99), eller -1 om parkeringen är full.
    static int HittaLedigPlats(string typ)
    {
        if (typ == "MC")
        {
            // En MC försöker först dela ruta med en ensam MC, så att vi sparar plats
            for (int i = 0; i < 100; i++)
            {
                if (parkering[i].StartsWith("MC#") && AntalFordon(parkering[i]) == 1)
                {
                    return i;
                }
            }
        }

        // Annars: ta första helt tomma ruta (gäller alla bilar och MC utan sällskap)
        for (int i = 0; i < 100; i++)
        {
            if (parkering[i] == "")
            {
                return i;
            }
        }
        return -1; // ingen plats hittades
    }

    // Lägger till ett fordon sist i en ruta, t.ex. "CAR#ABC123;"
    static void LäggTillFordon(int index, string typ, string reg)
    {
        parkering[index] = parkering[index] + typ + "#" + reg + ";";
    }

    // Tar bort ett fordon från en ruta genom att ersätta dess text med ingenting
    static void TaBortFordon(int index, string typ, string reg)
    {
        parkering[index] = parkering[index].Replace(typ + "#" + reg + ";", "");
    }
}

