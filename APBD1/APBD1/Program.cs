using APBD1;

          // 1. Stworzenie kontenera danego typu
            Console.WriteLine("=== Test 1: Tworzenie kontenerów ===");
            KontenerL kontenerL = new KontenerL();
            KontenerG kontenerG = new KontenerG(1500); // przykładowe ciśnienie
            KontenerC kontenerC = new KontenerC("Fish", -10); // dla produktu "Fish" temperatura -10 (spełnia warunek)
            
            // 2. Załadowanie ładunku do danego kontenera
            Console.WriteLine("\n=== Test 2: Ładowanie ładunku do kontenerów ===");
            try
            {
                // Dla KontenerL, używamy metody zaladujKontener (wartość < 90% max = 180)
                kontenerL.zaladujKontener(150);
                Console.WriteLine("KontenerL zaladowany ładunkiem 150");
            }
            catch (OverfillException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            try
            {
                // Dla KontenerG (max = 2000)
                kontenerG.zaladujKontener(1500);
                Console.WriteLine("KontenerG zaladowany ładunkiem 1500");
            }
            catch (OverfillException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            // Dla KontenerC korzystamy z przeciążonej metody (sprawdzenie rodzaju ładunku)
            kontenerC.zaladujKontener(100, "Fish");
            Console.WriteLine("KontenerC zaladowany ładunkiem 100 (produkt: Fish)");

            // 3. Załadowanie kontenera na statek
            Console.WriteLine("\n=== Test 3: Załadowanie pojedynczego kontenera na statek ===");
            // Ustawiamy statek: maks. 5 kontenerów, maks. 20 ton (czyli 20*1000 kg)
            Kontenerowiec statek1 = new Kontenerowiec(30, 5, 20);
            statek1.zaladujKontener(kontenerL);
            Console.WriteLine("Na statek1 dodano kontenerL");

            // 4. Załadowanie listy kontenerów na statek
            Console.WriteLine("\n=== Test 4: Załadowanie listy kontenerów na statek ===");
            List<Kontener> listaKontenerow = new List<Kontener> { kontenerG, kontenerC };
            statek1.zaladujKontener(listaKontenerow);
            Console.WriteLine("Na statek1 dodano listę: kontenerG i kontenerC");

            // 5. Usunięcie kontenera ze statku
            Console.WriteLine("\n=== Test 5: Usunięcie kontenera ze statku ===");
            statek1.rozladujKontener(kontenerG);
            Console.WriteLine("Ze statek1 usunięto kontenerG");

            // 6. Rozładowanie kontenera
            Console.WriteLine("\n=== Test 6: Rozładowanie kontenera ===");
            Console.WriteLine("Stan konteneraC przed rozładowaniem:");
            kontenerC.wypiszDane();
            kontenerC.oproznijLadunek();
            Console.WriteLine("Stan konteneraC po rozładowaniu:");
            kontenerC.wypiszDane();

            // 7. Zastąpienie kontenera na statku innym kontenerem
            Console.WriteLine("\n=== Test 7: Zastąpienie kontenera na statku ===");
            KontenerL nowyKontenerL = new KontenerL();
            try
            {
                nowyKontenerL.zaladujKontener(100); // ustawienie ładunku
                Console.WriteLine("Nowy kontenerL zaladowany ładunkiem 100");
            }
            catch (OverfillException ex)
            {
                Console.WriteLine(ex.Message);
            }
            // Zastępujemy stary kontenerL nowym
            statek1.zastapKontener(kontenerL, nowyKontenerL);
            Console.WriteLine("Na statek1 zastąpiono stary kontenerL nowym");

            // 8. Przeniesienie kontenera między dwoma statkami
            Console.WriteLine("\n=== Test 8: Przeniesienie kontenera między statkami ===");
            Kontenerowiec statek2 = new Kontenerowiec(25, 5, 20);
            // Przenosimy kontenerC ze statek1 do statek2
            statek1.przeniesKontener(statek2, kontenerC);
            Console.WriteLine("Przeniesiono kontenerC ze statek1 do statek2");

            // 9. Wypisanie informacji o danym kontenerze
            Console.WriteLine("\n=== Test 9: Informacje o kontenerze ===");
            nowyKontenerL.wypiszDane();

            // 10. Wypisanie informacji o danym statku i jego ładunku
            Console.WriteLine("\n=== Test 10: Informacje o statkach ===");
            Console.WriteLine("Statek 1:");
            statek1.wypiszDane();
            Console.WriteLine("Statek 2:");
            statek2.wypiszDane();
            
            Console.WriteLine("\nTesty zakończone. Naciśnij dowolny klawisz, aby zakończyć.");
            Console.ReadKey();