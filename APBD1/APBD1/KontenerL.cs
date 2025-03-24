namespace APBD1;

public class KontenerL : Kontener, IHazardNotifier
{
    
    public KontenerL() : base( 259, 2300, 606, 'L', 24000)
    {
    }
    
    public void notify()
    {
        Console.WriteLine("Zaszla niebezpieczna sytuacja. Numer kontenera: " + numerSeryjny);
    }
    
    
    public override void zaladujKontener(double masaLadunku)
    {
        zaladujKontener(masaLadunku, true);
    }

    public void zaladujKontener(double masaLadunku, bool czyBezpieczny)
    {
        if (czyBezpieczny)
        {
            if (masaLadunku < maxLadunek * 0.9)
            {
                this.masaLadunku = masaLadunku;
            }
            else
            {
                notify();
                throw new OverfillException("Przekroczono dopuszczalną masę ładunku dla kontenera L " + numerSeryjny);
            }
        }
        else
        {
            if (masaLadunku < maxLadunek * 0.5)
            {
                this.masaLadunku = masaLadunku;
            }
            else
            {
                notify();
                throw new OverfillException("Przekroczono dopuszczalną masę ładunku dla kontenera L " + numerSeryjny);
            }
        }
    }
}

