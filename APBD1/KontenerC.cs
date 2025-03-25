namespace APBD1;
public class KontenerC : Kontener
{
    private Dictionary<String, double> produkty;
    protected String rodzajProduktu;
    protected double temperatura;

    public KontenerC(String rodzajProduktu, double temperatura) : base( 259, 2300, 606, 'C', 24000)
    {
        this.rodzajProduktu = rodzajProduktu;
        this.temperatura = temperatura;
        this.produkty = new Dictionary<string, double>();
        produkty.Add("Bananas", 13.3);
        produkty.Add("Chocolate", 18.0);
        produkty.Add("Fish", 2);
        produkty.Add("Meat", -15);
        produkty.Add("Ice cream", -18);
        produkty.Add("Frozen pizza", -30);
        produkty.Add("Cheese", 7.2);
        produkty.Add("Sausages", 5);
        produkty.Add("Butter", 20.5);
        produkty.Add("Eggs", 19);
    }

    public void zaladujKontener(double masaLadunku, String typLadunku)
    {
        if (masaLadunku > maxLadunek)
        {
            throw new OverfillException("Przekroczono dopuszczalną masę ładunku dla kontenera L " + numerSeryjny);
        }
        if (typLadunku != rodzajProduktu)
        {
            Console.WriteLine("Probujesz umiescic w kontenerze zly typ produktu!" );
            Console.WriteLine("Powinienes chciec umiescic: " + rodzajProduktu);
        }
        else if (produkty[typLadunku]<temperatura)
        {
            Console.WriteLine("Temperatura kontenera jest za wysoka na ten produkt!");
        }
        else
        {
            this.masaLadunku = masaLadunku;
        }
    }
    
}