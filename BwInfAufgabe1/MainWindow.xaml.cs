using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace BwInfAufgabe1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        int[] BestBlumenbeet = new int[9];
        int BestResult;
        int[,] Farben;
        int[,] Farbpaare;
        int[,] Blumenbeet;
        private void ButtonCalculate_Click(object sender, RoutedEventArgs e)
        {
            ClearVisualBlumenbeet();
            bool Dialogfenster = false;
            BestResult = 0;
            string EingabeString;
            string[] EingabeArray;
            int AnzahlFarben;
            int DifferenzFarben;
            int[,] FarbenVorlaeufig = new int[7, 4];
            int[,] FarbpaareVorlaeufig;
            Blumenbeet = CreateBlumenbeet();



            try
            {
                EingabeString = TextBoxInput.Text;
                EingabeArray = Regex.Split(EingabeString, "\r\n");

                //Es wird ermittelt, wie viele unterschiedliche Farben erwünscht sind
                AnzahlFarben = int.Parse(EingabeArray[0]);

                //Farbpaar Array und Farben Array werden gefüllt
                FarbpaareVorlaeufig = GetAllFarbpaare(EingabeArray);
                GetAllFarbnummern(FarbpaareVorlaeufig, FarbenVorlaeufig);

                Ausgabe(Dialogfenster, FarbpaareVorlaeufig, FarbenVorlaeufig, Blumenbeet);

                //Ausnahme, es wurden mehr Farben in Farbpaaren angegeben als Farben insgesammt erwünscht
                int FarbenZuViel = GetFirstFreeIndex(FarbenVorlaeufig) - int.Parse(EingabeArray[0]);
                while (FarbenZuViel > 0)
                {
                    //Ermittle Farbe die am wertlosesten ist
                    int indexFarbe = GetFarbeWithLowestPoints(FarbenVorlaeufig);

                    //Lösche alle Farbpaare mit ihr
                    DeleteFarbpaare(FarbpaareVorlaeufig, FarbenVorlaeufig, FarbenVorlaeufig[indexFarbe, 0]);

                    //Lösche Farbe
                    FarbenVorlaeufig[indexFarbe, 0] = 0;
                    FarbenZuViel--;
                }
            }
            catch
            {
                MessageBox.Show("Die Eingabeparameter konnten nicht entgegengenommen werden");
                throw;
                //return;
            }

            //Farbenarray mit Farben auffüllen, bis die Anzahl an erwünschten Farben erreicht ist
            Ausgabe(Dialogfenster, FarbpaareVorlaeufig, FarbenVorlaeufig, Blumenbeet);
            if (GetFirstFreeIndex(FarbenVorlaeufig) != -1)
            {
                DifferenzFarben = AnzahlFarben - GetFirstFreeIndex(FarbenVorlaeufig);
                if (DifferenzFarben > 0)
                {
                    FillFarben(DifferenzFarben, FarbenVorlaeufig);
                }

                if (Dialogfenster)
                {
                    MessageBox.Show("Differenz Farben: " + DifferenzFarben.ToString());
                }
            }
            //Ausgabe(Dialogfenster, FarbpaareVorlaeufig, FarbenVorlaeufig, Blumenbeet);

            //Arrays final sortieren
            FinalSort(FarbenVorlaeufig, FarbpaareVorlaeufig);

            Ausgabe(Dialogfenster, Farbpaare, Farben, Blumenbeet);


            //FillBlumenbeet(ref Farben, ref Farbpaare, ref Blumenbeet, 0, false);

            FillBlumenbeet2();

            //Das beste Blumenbeet anzeigen
            for (int i = 0; i < BestBlumenbeet.Length; i++)
            {
                SetBlumeToFarbe(i, BestBlumenbeet[i]);
            }
            MessageBox.Show("Das erstellt Blumenbeet hat eine Gesammtpunktzahl von: " + BestResult);
            TextBoxInput.Text += "\n\nErgebnis: " + BestResult;
        }

        //Blumenbeet füllen
        private void FillBlumenbeet1(ref int[,] Farben, ref int[,] Farbpaare, ref int[,] Blumenbeet, int Platz, bool reset)
        {
            if (Platz > 8 || Platz < 0)
            {
                return;
            }
            else
            {
                //Gehe alle Farben durch für Platz
                for (int i = 0; i < Farben.GetLength(0); i++)
                {
                    if (reset)
                    {
                        reset = false;
                        i = GetIndexOfElementInArray(Farben, Blumenbeet[Platz, 0]) + 1;

                        if (i >= Farben.GetLength(0))
                        {
                            break;
                        }
                    }

                    //MessageBox.Show("FillBlumenbeet(Platz: " + Platz + " Zurück: " + a + " Farbe: " + i + ")");
                    //TextBoxInput.Text += "\nFillBlumenbeet(" + Platz + ")";

                    //Choose - Setze Blume auf übergebenen Platz
                    SetBlumeInBlumenbeet(Blumenbeet, Platz, Farben[i, 0], Farbpaare);

                    //Explore - Setze alle möglichen Blumen auf den nächsten Platz
                    for (int j = 0; j < Farben.GetLength(0); j++)
                    {
                        if (Platz < 8)
                        {
                            FillBlumenbeet1(ref Farben, ref Farbpaare, ref Blumenbeet, Platz + 1, false);
                        }
                        else
                        {
                            //Check result
                            //CheckResult(Blumenbeet, Farbpaare);
                            break;
                        }
                    }
                }

                //Un-Choose - Ein Platz runter gehen und da weiter machen, wo man aufgehört hat
                FillBlumenbeet1(ref Farben, ref Farbpaare, ref Blumenbeet, Platz - 1, true);

            }
        }
        private void FillBlumenbeet2()
        {
            int FarbenLaenge = Farben.GetLength(0);
            int BlumenbeetLaenge = Blumenbeet.GetLength(0);

            for (int b0 = 0; b0 < BlumenbeetLaenge; b0++)
            {
                for (int f0 = 0; f0 < FarbenLaenge; f0++)
                {
                    //Setze Blume
                    SetBlumeInBlumenbeet(Blumenbeet, b0, Farben[f0, 0], Farbpaare);

                    for (int b1 = b0 + 1; b1 < BlumenbeetLaenge; b1++)
                    {
                        for (int f1 = 0; f1 < FarbenLaenge; f1++)
                        {
                            //Setze Blume
                            SetBlumeInBlumenbeet(Blumenbeet, b1, Farben[f1, 0], Farbpaare);

                            for (int b2 = b1 + 1; b2 < BlumenbeetLaenge; b2++)
                            {
                                for (int f2 = 0; f2 < FarbenLaenge; f2++)
                                {
                                    //Setze Blume
                                    SetBlumeInBlumenbeet(Blumenbeet, b2, Farben[f2, 0], Farbpaare);

                                    for (int b3 = b2 + 1; b3 < BlumenbeetLaenge; b3++)
                                    {
                                        for (int f3 = 0; f3 < FarbenLaenge; f3++)
                                        {
                                            //Setze Blume
                                            SetBlumeInBlumenbeet(Blumenbeet, b3, Farben[f3, 0], Farbpaare);

                                            for (int b4 = b3 + 1; b4 < BlumenbeetLaenge; b4++)
                                            {
                                                for (int f4 = 0; f4 < FarbenLaenge; f4++)
                                                {
                                                    //Setze Blume
                                                    SetBlumeInBlumenbeet(Blumenbeet, b4, Farben[f4, 0], Farbpaare);

                                                    for (int b5 = b4 + 1; b5 < BlumenbeetLaenge; b5++)
                                                    {
                                                        for (int f5 = 0; f5 < FarbenLaenge; f5++)
                                                        {
                                                            //Setze Blume
                                                            SetBlumeInBlumenbeet(Blumenbeet, b5, Farben[f5, 0], Farbpaare);

                                                            for (int b6 = b5 + 1; b6 < BlumenbeetLaenge; b6++)
                                                            {
                                                                for (int f6 = 0; f6 < FarbenLaenge; f6++)
                                                                {
                                                                    //Setze Blume
                                                                    SetBlumeInBlumenbeet(Blumenbeet, b6, Farben[f6, 0], Farbpaare);

                                                                    for (int b7 = b6 + 1; b7 < BlumenbeetLaenge; b7++)
                                                                    {
                                                                        for (int f7 = 0; f7 < FarbenLaenge; f7++)
                                                                        {
                                                                            //Setze Blume
                                                                            SetBlumeInBlumenbeet(Blumenbeet, b7, Farben[f7, 0], Farbpaare);

                                                                            for (int b8 = b7 + 1; b8 < BlumenbeetLaenge; b8++)
                                                                            {
                                                                                for (int f8 = 0; f8 < FarbenLaenge; f8++)
                                                                                {
                                                                                    //Setze Blume
                                                                                    SetBlumeInBlumenbeet(Blumenbeet, b8, Farben[f8, 0], Farbpaare);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        //Deklerationen
        private void ClearVisualBlumenbeet()
        {
            TabPanel1.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            TabPanel2.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            TabPanel3.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            TabPanel4.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            TabPanel5.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            TabPanel6.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            TabPanel7.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            TabPanel8.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            TabPanel9.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }
        private int[,] CreateBlumenbeet()
        {
            //Array für das Blumenbeet
            //Erste Spalte speichert den Nummerncode der Blumenfarbe und der Index gibt an welchem Steckplatz 
            //Zweite Spalte ist wie viele Nebenblumen ein Steckplatz hat
            int[,] Blumenbeet = new int[9, 8];

            //Belegt jeden Index der Blumennummer spalte mit einer Prüfziffer
            for (int i = 0; i < 9; i++)
            {
                Blumenbeet[i, 0] = -1;
            }

            //Definiert wie viele Nebenblumen ein Steckplatz hat
            Blumenbeet[0, 1] = 2;
            Blumenbeet[1, 1] = 4;
            Blumenbeet[2, 1] = 4;
            Blumenbeet[3, 1] = 3;
            Blumenbeet[4, 1] = 6;
            Blumenbeet[5, 1] = 3;
            Blumenbeet[6, 1] = 4;
            Blumenbeet[7, 1] = 4;
            Blumenbeet[8, 1] = 2;


            //Definiert, welche Nachbaar Blumenplätze jeder Blumenplatz hat
            Blumenbeet[0, 2] = 1;
            Blumenbeet[0, 3] = 2;

            Blumenbeet[1, 2] = 0;
            Blumenbeet[1, 3] = 2;
            Blumenbeet[1, 4] = 3;
            Blumenbeet[1, 5] = 4;

            Blumenbeet[2, 2] = 0;
            Blumenbeet[2, 3] = 1;
            Blumenbeet[2, 4] = 4;
            Blumenbeet[2, 5] = 5;

            Blumenbeet[3, 2] = 1;
            Blumenbeet[3, 3] = 4;
            Blumenbeet[3, 4] = 6;

            Blumenbeet[4, 2] = 1;
            Blumenbeet[4, 3] = 2;
            Blumenbeet[4, 4] = 3;
            Blumenbeet[4, 5] = 5;
            Blumenbeet[4, 6] = 6;
            Blumenbeet[4, 7] = 7;

            Blumenbeet[5, 2] = 2;
            Blumenbeet[5, 3] = 4;
            Blumenbeet[5, 4] = 7;

            Blumenbeet[6, 2] = 3;
            Blumenbeet[6, 3] = 4;
            Blumenbeet[6, 4] = 7;
            Blumenbeet[6, 5] = 8;

            Blumenbeet[7, 2] = 4;
            Blumenbeet[7, 3] = 5;
            Blumenbeet[7, 4] = 6;
            Blumenbeet[7, 5] = 8;

            Blumenbeet[8, 2] = 6;
            Blumenbeet[8, 3] = 7;

            return Blumenbeet;
        }
        private int[,] GetAllFarbpaare(string[] EingabeArray)
        {
            //Diese Methode liefert ein Array aus mit allen Farbpaaren die angegeben wurden, mit ihrer Punktzahl
            int lengh = int.Parse(EingabeArray[1]);
            int[,] Farbpaare = new int[lengh, 4];
            for (int i = 0; i < lengh; i++)
            {
                string[] EingabeArrayRow = EingabeArray[i + 2].Split(' ');
                Farbpaare[i, 0] = BlumenNameToBlumenNummer(EingabeArrayRow[0]);
                Farbpaare[i, 1] = BlumenNameToBlumenNummer(EingabeArrayRow[1]);
                Farbpaare[i, 2] = int.Parse(EingabeArrayRow[2]);
            }

            return Farbpaare;
        }
        private int BlumenNameToBlumenNummer(string Blumenname)
        {
            //Diese Methode konvertiert den übergebenen Blumennamen in einen Blumencode
            switch (Blumenname)
            {
                case "blau":
                    return 1;
                case "gelb":
                    return 2;
                case "gruen":
                    return 3;
                case "orange":
                    return 4;
                case "rosa":
                    return 5;
                case "rot":
                    return 6;
                case "tuerkis":
                    return 7;
                default:
                    return 0;
            }
        }
        private void GetAllFarbnummern(int[,] Farbpaare, int[,] FarbnummernUndHaeufigkeit)
        {
            //Diese Methode nimmt alle Farbpaare entgegen und ermittelt alle genutzen Farben und gibt diese in einem Array aus

            for (int i = 0; i < Farbpaare.GetLength(0); i++)
            {
                //Es wird geschaut ob die Farbe aus dem Farbpaare Array (Spalte 1) schon im Farben Array ist, wenn nicht wird diese hinzugefügt.
                //Außerdem wird der Zähler für die Farbe um eins erhöht
                int Index = GetIndexOfElementInArray(FarbnummernUndHaeufigkeit, Farbpaare[i, 0]);
                if (-1 == Index)
                {
                    Index = GetFirstFreeIndex(FarbnummernUndHaeufigkeit);
                    FarbnummernUndHaeufigkeit[Index, 0] = Farbpaare[i, 0];
                    FarbnummernUndHaeufigkeit[Index, 1] = 1;
                    FarbnummernUndHaeufigkeit[Index, 2] = Farbpaare[i, 2];
                }
                else
                {
                    FarbnummernUndHaeufigkeit[Index, 1]++;
                    FarbnummernUndHaeufigkeit[Index, 2] += Farbpaare[i, 2];
                }

                //Gleiche wie oben nur diesmal Spalte 2 des Farbpaar Arrays
                Index = GetIndexOfElementInArray(FarbnummernUndHaeufigkeit, Farbpaare[i, 1]);
                if (-1 == Index)
                {
                    Index = GetFirstFreeIndex(FarbnummernUndHaeufigkeit);
                    FarbnummernUndHaeufigkeit[Index, 0] = Farbpaare[i, 1];
                    FarbnummernUndHaeufigkeit[Index, 1] = 1;
                    FarbnummernUndHaeufigkeit[Index, 2] = Farbpaare[i, 2];
                }
                else
                {
                    FarbnummernUndHaeufigkeit[Index, 1]++;
                    FarbnummernUndHaeufigkeit[Index, 2] += Farbpaare[i, 2];
                }
            }

        }


        //Arrays
        private int GetIndexOfElementInArray(int[,] Array, int Element)
        {
            //Diese Metode überprügt ob das übergebene Element im Array ist
            for (int i = 0; i < Array.GetLength(0); i++)
            {
                if (Array[i, 0] == Element) { return i; }
            }
            return -1;
        }
        private int GetFirstFreeIndex(int[,] Array)
        {
            //Diese Methode gibt den ersten freien Index eines Arrays zurück
            for (int i = 0; i < Array.GetLength(0); i++)
            {
                if (Array[i, 0] == 0)
                {
                    return i;
                }
            }
            return -1;
        }


        //Loeschen nicht erlaubter Farben
        private void DeleteFarbpaare(int[,] FarbpaareVorlaeufig, int[,] FarbenVorlaeufig, int Farbe)
        {
            for (int i = 0; i < FarbpaareVorlaeufig.GetLength(0); i++)
            {
                if (FarbpaareVorlaeufig[i, 0] == Farbe || FarbpaareVorlaeufig[i, 1] == Farbe)
                {
                    //Subtrahiere den Paarwert von der Partnerblume
                    for (int j = 0; j < FarbenVorlaeufig.GetLength(0); j++)
                    {
                        if (FarbpaareVorlaeufig[i, 0] == FarbenVorlaeufig[j, 0])
                        {
                            FarbenVorlaeufig[j, 1]--;
                            FarbenVorlaeufig[j, 2] -= FarbpaareVorlaeufig[i, 2];
                        }
                        else if (FarbpaareVorlaeufig[i, 1] == FarbenVorlaeufig[j, 0])
                        {
                            FarbenVorlaeufig[j, 1]--;
                            FarbenVorlaeufig[j, 2] -= FarbpaareVorlaeufig[i, 2];
                        }
                    }

                    //Schiebe das Array um ein Platz nach vorne
                    for (int j = i; j < FarbpaareVorlaeufig.GetLength(0) - i + 1; j++)
                    {
                        if (j < FarbpaareVorlaeufig.GetLength(0) - 1)
                        {
                            FarbpaareVorlaeufig[j, 0] = FarbpaareVorlaeufig[j + 1, 0];
                            FarbpaareVorlaeufig[j, 1] = FarbpaareVorlaeufig[j + 1, 0];
                            FarbpaareVorlaeufig[j, 2] = FarbpaareVorlaeufig[j + 1, 0];
                        }
                        else
                        {
                            FarbpaareVorlaeufig[FarbpaareVorlaeufig.GetLength(0) - 1, 0] = 0;
                            FarbpaareVorlaeufig[FarbpaareVorlaeufig.GetLength(0) - 1, 1] = 0;
                            FarbpaareVorlaeufig[FarbpaareVorlaeufig.GetLength(0) - 1, 2] = 0;
                        }
                    }
                }
            }
        }
        private int GetFarbeWithLowestPoints(int[,] FarbenVorlaeufig)
        {
            int WenigstenPunkte = 6;
            int WenigstenPunkteIndex = 0;

            //Ermittelt die Farbe mit den wenigsten Farbkombinationen
            for (int i = 0; i < 7; i++)
            {
                if (WenigstenPunkte > FarbenVorlaeufig[i, 2] && FarbenVorlaeufig[i, 2] > 0)
                {
                    WenigstenPunkte = FarbenVorlaeufig[i, 2];
                    WenigstenPunkteIndex = i;
                }
                //Wenn zwei Blumen den gleichen Wert haben, wird die ausgewählt, die in mehr Farbpaaren vorkommt
                else if (WenigstenPunkte == FarbenVorlaeufig[i, 2])
                {
                    if (FarbenVorlaeufig[WenigstenPunkteIndex, 1] < FarbenVorlaeufig[i, 1])
                    {
                        WenigstenPunkte = FarbenVorlaeufig[i, 1];
                        WenigstenPunkteIndex = i;
                    }
                }
            }
            return WenigstenPunkteIndex;
        }
        private void FinalSort(int[,] FarbenVorlaeufig, int[,] FarbpaareVorlaeufig)
        {
            int a = 0;

            //Finde herraus, wie viele Farben es tatsächlich gibt
            for (int i = 0; i < FarbenVorlaeufig.GetLength(0); i++)
            {
                if (FarbenVorlaeufig[i, 0] != 0)
                {
                    a++;
                }
            }

            //Erstellt das richtige Farben Array
            Farben = new int[a, 4];
            a = 0;
            for (int i = 0; i < FarbenVorlaeufig.GetLength(0); i++)
            {
                if (FarbenVorlaeufig[i, 0] == 0)
                {
                    a++;
                }
                else
                {
                    Farben[i - a, 0] = FarbenVorlaeufig[i, 0];
                    Farben[i - a, 1] = FarbenVorlaeufig[i, 1];
                    Farben[i - a, 2] = FarbenVorlaeufig[i, 2];
                }
            }

            a = 0;
            for (int i = 0; i < FarbpaareVorlaeufig.GetLength(0); i++)
            {
                if (FarbpaareVorlaeufig[i, 0] != 0)
                {
                    a++;
                }
            }

            Farbpaare = new int[a, 4];
            a = 0;
            for (int i = 0; i < FarbpaareVorlaeufig.GetLength(0); i++)
            {
                if (FarbpaareVorlaeufig[i, 0] == 0)
                {
                    a++;
                }
                else
                {
                    Farbpaare[i - a, 0] = FarbpaareVorlaeufig[i, 0];
                    Farbpaare[i - a, 1] = FarbpaareVorlaeufig[i, 1];
                    Farbpaare[i - a, 2] = FarbpaareVorlaeufig[i, 2];
                }
            }
        }


        //Fuelle mit erlaubten Farben auf
        private void FillFarben(int i, int[,] FarbnummernUndHaeufigkeit)
        {
            //Diese Methode füllt das Farben Array auf die gewünschte anzahl an Farben auf
            for (int Farbnummer = 1; Farbnummer <= 9; Farbnummer++)
            {
                if (GetIndexOfElementInArray(FarbnummernUndHaeufigkeit, Farbnummer) == -1)
                {
                    if (i == 0)
                    {
                        return;
                    }
                    int index = GetFirstFreeIndex(FarbnummernUndHaeufigkeit);
                    FarbnummernUndHaeufigkeit[index, 0] = Farbnummer;
                    FarbnummernUndHaeufigkeit[index, 1] = 0;
                    FarbnummernUndHaeufigkeit[index, 2] = 0;
                    i--;
                }
            }
        }


        //Setze Blume
        private void SetBlumeInBlumenbeet(int[,] Blumenbeet, int Platz, int Blume, int[,] Farbpaare)
        {
            Blumenbeet[Platz, 0] = Blume;

            CheckResult(Blumenbeet, Farbpaare);
            //SetBlumeToFarbe(Platz, Blume);
        }
        private void SetBlumeToFarbe(int Blumenplatz, int Blumennummer)
        {
            switch (Blumenplatz)
            {
                case 0:
                    TabPanel1.Background = GetFarbeOfBlume(Blumennummer);
                    break;
                case 1:
                    TabPanel2.Background = GetFarbeOfBlume(Blumennummer);
                    break;
                case 2:
                    TabPanel3.Background = GetFarbeOfBlume(Blumennummer);
                    break;
                case 3:
                    TabPanel4.Background = GetFarbeOfBlume(Blumennummer);
                    break;
                case 4:
                    TabPanel5.Background = GetFarbeOfBlume(Blumennummer);
                    break;
                case 5:
                    TabPanel6.Background = GetFarbeOfBlume(Blumennummer);
                    break;
                case 6:
                    TabPanel7.Background = GetFarbeOfBlume(Blumennummer);
                    break;
                case 7:
                    TabPanel8.Background = GetFarbeOfBlume(Blumennummer);
                    break;
                case 8:
                    TabPanel9.Background = GetFarbeOfBlume(Blumennummer);
                    break;
                default:
                    break;
            }
        }
        private Brush GetFarbeOfBlume(int Blumennummer)
        {
            switch (Blumennummer)
            {
                case 1:
                    return new SolidColorBrush(Color.FromRgb(0, 0, 255));
                case 2:
                    return new SolidColorBrush(Color.FromRgb(255, 255, 0));
                case 3:
                    return new SolidColorBrush(Color.FromRgb(0, 255, 0));
                case 4:
                    return new SolidColorBrush(Color.FromRgb(255, 165, 0));
                case 5:
                    return new SolidColorBrush(Color.FromRgb(255, 192, 203));
                case 6:
                    return new SolidColorBrush(Color.FromRgb(255, 0, 0));
                case 7:
                    return new SolidColorBrush(Color.FromRgb(63, 136, 143));
                default:
                    return new SolidColorBrush(Color.FromRgb(255, 255, 255));

            }
        }


        //Ergebnis Berechnung
        private int[] GetHigherNeighborBlumenOfIndex(int[,] Blumenbeet, int Index)
        {
            int[] Neighbors = new int[3];
            switch (Index)
            {
                case 0:
                    Neighbors[0] = 1;
                    Neighbors[1] = 2;
                    break;
                case 1:
                    Neighbors[0] = 2;
                    Neighbors[1] = 3;
                    Neighbors[2] = 4;
                    break;
                case 2:
                    Neighbors[0] = 4;
                    Neighbors[1] = 5;
                    break;
                case 3:
                    Neighbors[0] = 4;
                    Neighbors[1] = 6;
                    break;
                case 4:
                    Neighbors[0] = 5;
                    Neighbors[1] = 6;
                    Neighbors[2] = 7;
                    break;
                case 5:
                    Neighbors[0] = 7;
                    break;
                case 6:
                    Neighbors[0] = 7;
                    Neighbors[1] = 8;
                    break;
                case 7:
                    Neighbors[0] = 8;
                    break;
                default:
                    break;
            }
            //int[] Neighbors = new int[3];
            //int index = 0;
            //for (int i = 0; i < 6; i++)
            //{
            //    if (Blumenbeet[Index, i + 2] <= Index)
            //    {
            //        continue;
            //    }
            //    Neighbors[index] = Blumenbeet[Index, i + 2];
            //    index++;
            //}

            //string Ausgabe1 = Index.ToString() + "\n";
            //for (int a = 0; a < Neighbors.GetLength(0); a++)
            //{
            //    Ausgabe1 += Neighbors[a] + "\n";
            //}
            //MessageBox.Show(Ausgabe1);

            return Neighbors;
        }
        private void CheckResult(int[,] Blumenbeet, int[,] Farbpaare)
        {
            int result = CalculateResult(Blumenbeet, Farbpaare);

            if (result > BestResult)
            {
                BestResult = result;

                for (int i = 0; i < BestBlumenbeet.Length; i++)
                {
                    BestBlumenbeet[i] = Blumenbeet[i, 0];
                }
            }
        }
        private int CalculateResult(int[,] Blumenbeet, int[,] Farbpaare)
        {
            int result = 0;

            //Gehe durch das Blumenbeet
            for (int i = 0; i < Blumenbeet.GetLength(0) - 1; i++)
            {
                //Markiere Farbe als benutzt
                for (int j = 0; j < Farben.GetLength(0); j++)
                {
                    if (Farben[j, 0] == Blumenbeet[i, 0])
                    {
                        Farben[j, 3]++;
                    }
                }

                //Holt sich die Nachbarn jeder einzelnen Blume
                int[] Neighbors = GetHigherNeighborBlumenOfIndex(Blumenbeet, i);

                //Überprüfe, welche Nachbarn ein Blumenppar sind
                for (int j = 0; j < Neighbors.Length; j++)
                {
                    if (j > 0 && Neighbors[j] == 0)
                    {
                        break;
                    }

                    CheckIfTwoColorsAreAPair(Farbpaare, Blumenbeet[i, 0], Blumenbeet[Neighbors[j], 0], ref result);
                }
            }

            //Schau nach, ob alle Farbpaare gesetz wurden
            for (int i = 0; i < Farbpaare.GetLength(0); i++)
            {
                if (Farbpaare[i, 3] == 0)
                {
                    result = 0;
                }
                Farbpaare[i, 3] = 0;
            }

            //Schaue nach, ob alle Farben gesetzt wurden
            for (int i = 0; i < Farben.GetLength(0); i++)
            {
                if (Farben[i, 3] == 0)
                {
                    result = 0;
                }
                Farben[i, 3] = 0;
            }
            return result;
        }
        private void CheckIfTwoColorsAreAPair(int[,] Farbpaare, int Blume1, int Blume2, ref int result)
        {
            for (int i = 0; i < Farbpaare.GetLength(0); i++)
            {
                if ((Farbpaare[i, 0] == Blume1 && Farbpaare[i, 1] == Blume2) || (Farbpaare[i, 0] == Blume2 && Farbpaare[i, 1] == Blume1))
                {
                    result += Farbpaare[i, 2];
                    Farbpaare[i, 3] = 1;
                }
            }
        }

        //Ausgabe der Arrays
        private void Ausgabe(bool Ausgabe, int[,] Array1, int[,] Array2, int[,] Blumenbeet)
        {
            if (Ausgabe == false)
            {
                return;
            }
            //Diese Methode gibt alle Werte der beiden Arrays aus
            string Ausgabe1 = string.Empty;
            for (int i = 0; i < Array1.GetLength(0); i++)
            {
                for (int j = 0; j < Array1.GetLength(1); j++)
                {
                    Ausgabe1 += Array1[i, j] + " ";
                }
                Ausgabe1 += "\n";
            }

            string Ausgabe2 = string.Empty;
            for (int i = 0; i < Array2.GetLength(0); i++)
            {
                for (int j = 0; j < Array2.GetLength(1); j++)
                {
                    Ausgabe2 += Array2[i, j] + " ";
                }
                Ausgabe2 += "\n";
            }

            string Ausgabe3 = string.Empty;
            for (int i = 0; i < Blumenbeet.GetLength(0); i++)
            {
                for (int j = 0; j < Blumenbeet.GetLength(1); j++)
                {
                    Ausgabe3 += Blumenbeet[i, j] + " ";
                }
                Ausgabe3 += "\n";
            }



            MessageBox.Show(Ausgabe1);
            MessageBox.Show(Ausgabe2);
            MessageBox.Show(Ausgabe3);


        }


        //Beispiele
        private void ButtonBeispiel1_Click(object sender, RoutedEventArgs e)
        {
            TextBoxInput.Text = "7\r\n2\r\nrot blau 3\r\nrot tuerkis 2";
        }
        private void ButtonBeispiel2_Click(object sender, RoutedEventArgs e)
        {
            TextBoxInput.Text = "2\r\n2\r\nrot tuerkis 3\r\ngruen rot 1";
        }
        private void ButtonBeispiel3_Click(object sender, RoutedEventArgs e)
        {
            TextBoxInput.Text = "3\r\n2\r\ntuerkis rot 3\r\nrot gruen 1";
        }
        private void ButtonBeispiel4_Click(object sender, RoutedEventArgs e)
        {
            TextBoxInput.Text = "7\r\n2\r\nrot tuerkis 3\r\ngruen rot 1";
        }
        private void ButtonBeispiel5_Click(object sender, RoutedEventArgs e)
        {
            TextBoxInput.Text = "7\r\n6\r\nrot tuerkis 3\r\ngruen rot 1\r\nrot rosa 3\r\nblau rosa 2\r\ngelb orange 1\r\ngruen orange 1";
        }
        private void ButtonBeispiel6_Click(object sender, RoutedEventArgs e)
        {
            TextBoxInput.Text = "7\r\n7\r\nrot tuerkis 3\r\nrot gruen 1\r\nrot rosa 3\r\nblau rosa 2\r\ngelb orange 1\r\ngruen orange 1\r\nblau tuerkis 2";
        }
    }
}