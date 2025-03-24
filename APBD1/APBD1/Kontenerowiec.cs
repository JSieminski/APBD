namespace APBD1;

public class Kontenerowiec
{
    public List<Kontener> kontenery = new List<Kontener>();
    public double maksymalnaPredkosc;
    public int maxLiczbaKontenerow;
    public double maxWagaKontenerow; // tony - kontenery mialy wage w kg

    public double aktualnaWagaKontenerow = 0;


    public Kontenerowiec(double maksymalnaPredkosc, int maxLiczbaKontenerow, double maxWagaKontenerow)
    {
        this.maksymalnaPredkosc = maksymalnaPredkosc;
        this.maxLiczbaKontenerow = maxLiczbaKontenerow;
        this.maxWagaKontenerow = maxWagaKontenerow;
    }

    public void zaladujKontener(Kontener kontener)
    {
        if (kontenery.Count >= maxLiczbaKontenerow)
        {
            Console.WriteLine("Przekroczono maksymalna liczbe kontenerow");

        }
        else if (aktualnaWagaKontenerow + kontener.getCiezarKontenera() > maxWagaKontenerow*1000)
        {
            Console.WriteLine("Dodanie kontenera przekroczy maksymalna wage zaladunku");
        }
        else
        {
            aktualnaWagaKontenerow+= kontener.getCiezarKontenera();
            kontenery.Add(kontener);
        }
    }

    public void zaladujKontener(List<Kontener> kontener)
    {
        double tempZaladunek = 0;
        foreach (Kontener k in kontener)
        {
            tempZaladunek += k.getCiezarKontenera();
        }
        
        if (kontenery.Count + kontener.Count > maxLiczbaKontenerow)
        {
            Console.WriteLine("Dodanie listy kontenerow przekroczy maksymalna liczbe kontenerow");
        }
        else if(aktualnaWagaKontenerow + tempZaladunek > maxWagaKontenerow*1000)
        {
            Console.WriteLine("Dodanie listy kontenerow przekroczy maksymalna wage zaladunku kontenerow");
        }
        else
        {
            kontenery.AddRange(kontener);
            aktualnaWagaKontenerow+= tempZaladunek;
        }
    }

    public void rozladujKontener(Kontener kontener)
    {
        kontenery.Remove(kontener);
        aktualnaWagaKontenerow -= kontener.getCiezarKontenera();
    }

    public void przeniesKontener(Kontenerowiec kontenerowiec, Kontener kontener)
    {
        if (kontenery.Contains(kontener))
        {
            rozladujKontener(kontener);
            kontenerowiec.zaladujKontener(kontener);
        }
        else
        {
            Console.WriteLine("Kontenerowiec nie istnieje");
        }
    }


    public void zastapKontener(Kontener kontener1, Kontener kontener2)
    {
        if (kontenery.Contains(kontener1))
        {
            rozladujKontener(kontener1);
            zaladujKontener(kontener2);
        }
    }

    public void wypiszDane()
    {
        Console.WriteLine("Maksymalna predkosc: " + maksymalnaPredkosc);
        Console.WriteLine("Liczba kontenerow zaladowanych: " + kontenery.Count());
        Console.WriteLine("Maksymalna liczba kontenerow: " + maxLiczbaKontenerow);
        Console.WriteLine("Laczna waga kontenerow: " + aktualnaWagaKontenerow);
        Console.WriteLine("Maksymalna waga kontenerow: " + maxLiczbaKontenerow);
        Console.WriteLine("--------------------------");
        Console.WriteLine("Lista kontenerow: ");
        Console.WriteLine("--------------------------");
        if (kontenery.Count == 0)
        {
            Console.WriteLine("Aktualnie brak kontenerów na statku.");
        }
        else
        {
            foreach (Kontener k in kontenery)
            {
                k.wypiszDane();
            }
        }


    }
}