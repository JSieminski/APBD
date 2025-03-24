namespace APBD1;

public class Kontener
{
    protected double masaLadunku { get; set; } //kg
    protected double wysokosc { get; set; } // cm
    protected double masaKontenera { get; set; }
    protected double glebokosc { get; set; }
    protected string numerSeryjny { get; set; }
    protected double maxLadunek { get; set; }
    protected static int numerKontenera = 1;
    
    
    protected Kontener(double wysokosc, double masaKontenera, double glebokosc, char rodzajKontenera, double maxLadunek)
    {
        this.wysokosc = wysokosc;
        this.masaKontenera = masaKontenera;
        this.glebokosc = glebokosc;
        numerSeryjny = "KON-"+rodzajKontenera+"-"+numerKontenera.ToString();
        numerKontenera++;
        this.maxLadunek = maxLadunek;
    }

    public void wypiszDane()
    {
        Console.WriteLine("Masa ladunku: " + masaLadunku);
        Console.WriteLine("Wysokosc: " + wysokosc);
        Console.WriteLine("Masa kontenera: " + masaKontenera);
        Console.WriteLine("Glebokosc: " + glebokosc);
        Console.WriteLine("Numer seryjny: " + numerSeryjny);
        Console.WriteLine("Max Ladunek: " + maxLadunek);
    }
    
    public virtual void oproznijLadunek()
    {
        this.masaLadunku = 0;
    }

    public double getCiezarKontenera()
    {
        return this.masaLadunku+masaKontenera;
    }

    public virtual void zaladujKontener(double masaLadunku)
    {
        if (masaLadunku > maxLadunek)
        {
            throw new OverfillException("Przekroczono dopuszczalną masę ładunku w kontenerze " + numerSeryjny);
        }
        else
        {
            this.masaLadunku = masaLadunku;
        }
    }

}