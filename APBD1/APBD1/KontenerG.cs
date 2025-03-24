namespace APBD1;

public class KontenerG : Kontener, IHazardNotifier
{
    public double cisnienie;
    public KontenerG(double cisnienie) : base( 259, 2300, 606, 'G', 24000)
    {
        this.cisnienie = cisnienie;
    }

    public void notify()
    {
        Console.WriteLine("Zaszla niebezpieczna sytuacja. Numer kontenera: " + numerSeryjny);
    }
    
    public void oproznijLadunek()
    {
        this.masaLadunku *= 0.05;
    }
    
    public void zaladujKontener(double masaLadunku)
    {
        if (masaLadunku > maxLadunek)
        {
            throw new OverfillException("Przekroczono dopuszczalną masę ładunku dla kontenera L " + numerSeryjny);
        }
        this.masaLadunku = masaLadunku;   
    }
}