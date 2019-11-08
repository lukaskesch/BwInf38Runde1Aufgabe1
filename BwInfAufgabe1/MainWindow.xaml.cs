﻿using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

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
        int TemporaryResult;
        int BestResult;
        int[,] Farben;
        int[,] Farbpaare;
        int[] Blumenbeet;
        int[] Neighbors = new int[3];
        int Kontrolle;
        int AnzahlFarbenGesetzt;
        int AnzahlFarben;
        Stopwatch timer = new Stopwatch();
        private void ButtonCalculate_Click(object sender, RoutedEventArgs e)
        {
            ClearVisualBlumenbeet();
            BestResult = 0;
            TemporaryResult = 0;
            AnzahlFarbenGesetzt = 0;
            string EingabeString;
            string[] EingabeArray;
            int DifferenzFarben;
            Blumenbeet = new int[9];
            int[,] FarbenVorlaeufig = new int[7, 4];
            int[,] FarbpaareVorlaeufig;
            timer.Reset();


            try
            {
                EingabeString = TextBoxInput.Text;
                EingabeArray = Regex.Split(EingabeString, "\r\n");

                //Es wird ermittelt, wie viele unterschiedliche Farben erwünscht sind
                AnzahlFarben = int.Parse(EingabeArray[0]);

                //Farbpaar Array und Farben Array werden gefüllt
                FarbpaareVorlaeufig = GetAllFarbpaare(EingabeArray);
                GetAllFarbnummern(FarbpaareVorlaeufig, FarbenVorlaeufig);


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

                //Farbenarray mit Farben auffüllen, bis die Anzahl an erwünschten Farben erreicht ist
                if (GetFirstFreeIndex(FarbenVorlaeufig) != -1)
                {
                    DifferenzFarben = AnzahlFarben - GetFirstFreeIndex(FarbenVorlaeufig);
                    if (DifferenzFarben > 0)
                    {
                        FillFarben(DifferenzFarben, FarbenVorlaeufig);
                    }
                }

                //Arrays final sortieren
                FinalSort(FarbenVorlaeufig, FarbpaareVorlaeufig);

                //Errechne das beste Blumenbeet
                timer.Start();
                FillBlumenbeet1(0);
                timer.Stop();

                //Das beste Blumenbeet anzeigen
                for (int i = 0; i < BestBlumenbeet.Length; i++)
                {
                    SetBlumeToFarbe(i, BestBlumenbeet[i]);
                }
                MessageBox.Show("Das erstellt Blumenbeet hat eine Gesammtpunktzahl von " + BestResult + " welche innerhalb von " + timer.Elapsed.TotalSeconds + " Sekunden berechnet wurde");
                //TextBoxInput.Text += "\n\nErgebnis: " + BestResult;

            }
            catch
            {
                MessageBox.Show("Die Eingabeparameter konnten nicht entgegengenommen werden");
                throw;
                //return;
            }
        }

        //Blumenbeet füllen
        private void FillBlumenbeet1(int Platz)
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
                    //Choose - Setze Blume auf Platz
                    Blumenbeet[Platz] = Farben[i, 0];

                    //Markiere Blume als benutzt
                    if (Farben[i, 3] == 0)
                    {
                        AnzahlFarbenGesetzt++;
                    }
                    Farben[i, 3]++;

                    //Check result
                    if (CheckResult(Platz))
                    {
                        FillBlumenbeet1(Platz + 1);
                    }

                    //Markiere Blume als nicht benutzt
                    Farben[i, 3]--;
                    if (Farben[i, 3] == 0)
                    {
                        AnzahlFarbenGesetzt--;
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
        private void GetAllFarbnummern(int[,] FarbpaareVorlaeufig, int[,] FarbenVorlaeufig)
        {
            //Diese Methode nimmt alle Farbpaare entgegen und ermittelt alle genutzen Farben und gibt diese in einem Array aus

            for (int i = 0; i < FarbpaareVorlaeufig.GetLength(0); i++)
            {
                //Es wird geschaut ob die Farbe aus dem Farbpaare Array (Spalte 1) schon im Farben Array ist, wenn nicht wird diese hinzugefügt.
                //Außerdem wird der Zähler für die Farbe um eins erhöht
                int Index = GetIndexOfElementInArray(FarbenVorlaeufig, FarbpaareVorlaeufig[i, 0]);
                if (-1 == Index)
                {
                    Index = GetFirstFreeIndex(FarbenVorlaeufig);
                    FarbenVorlaeufig[Index, 0] = FarbpaareVorlaeufig[i, 0];
                    FarbenVorlaeufig[Index, 1] = 1;
                    FarbenVorlaeufig[Index, 2] = FarbpaareVorlaeufig[i, 2];
                }
                else
                {
                    FarbenVorlaeufig[Index, 1]++;
                    FarbenVorlaeufig[Index, 2] += FarbpaareVorlaeufig[i, 2];
                }

                //Gleiche wie oben nur diesmal Spalte 2 des Farbpaar Arrays
                Index = GetIndexOfElementInArray(FarbenVorlaeufig, FarbpaareVorlaeufig[i, 1]);
                if (-1 == Index)
                {
                    Index = GetFirstFreeIndex(FarbenVorlaeufig);
                    FarbenVorlaeufig[Index, 0] = FarbpaareVorlaeufig[i, 1];
                    FarbenVorlaeufig[Index, 1] = 1;
                    FarbenVorlaeufig[Index, 2] = FarbpaareVorlaeufig[i, 2];
                }
                else
                {
                    FarbenVorlaeufig[Index, 1]++;
                    FarbenVorlaeufig[Index, 2] += FarbpaareVorlaeufig[i, 2];
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
        private int[] GetHigherNeighborBlumenOfIndex(int Index)
        {
            switch (Index)
            {
                case 0:
                    Neighbors[0] = 1;
                    Neighbors[1] = 2;
                    Neighbors[2] = 0;
                    break;
                case 1:
                    Neighbors[0] = 2;
                    Neighbors[1] = 3;
                    Neighbors[2] = 4;
                    break;
                case 2:
                    Neighbors[0] = 4;
                    Neighbors[1] = 5;
                    Neighbors[2] = 0;
                    break;
                case 3:
                    Neighbors[0] = 4;
                    Neighbors[1] = 6;
                    Neighbors[2] = 0;
                    break;
                case 4:
                    Neighbors[0] = 5;
                    Neighbors[1] = 6;
                    Neighbors[2] = 7;
                    break;
                case 5:
                    Neighbors[0] = 7;
                    Neighbors[1] = 0;
                    Neighbors[2] = 0;
                    break;
                case 6:
                    Neighbors[0] = 7;
                    Neighbors[1] = 8;
                    Neighbors[2] = 0;
                    break;
                case 7:
                    Neighbors[0] = 8;
                    Neighbors[1] = 0;
                    Neighbors[2] = 0;
                    break;
                default:
                    break;
            }
            return Neighbors;
        }
        private bool CheckResult(int Platz)
        {
            TemporaryResult = 0;

            if (AnzahlFarben - AnzahlFarbenGesetzt > 8 - Platz)
            {
                return false;
            }
            else
            {
                //Gehe durch das Blumenbeet und errechne für jede Blume, die Punkte mit ihren Nachbarblumen
                for (short i = 0; i < Blumenbeet.GetLength(0); i++)
                {
                    //Schaue, ob das Blumenbeet an dieser Stelle noch nicht besetzt ist
                    if (Blumenbeet[i] == 0)
                    {
                        return true;
                    }

                    //Holt sich die Nachbarn jeder einzelnen Blume
                    Neighbors = GetHigherNeighborBlumenOfIndex(i);

                    //Überprüfe, welche Nachbarn ein Blumenpaar sind
                    for (short j = 0; j < Neighbors.Length; j++)
                    {
                        if (j > 0 && Neighbors[j] == 0 || i == Blumenbeet.GetLength(0) - 1)
                        {
                            break;
                        }

                        CheckIfTwoColorsAreAPair(Farbpaare, Blumenbeet[i], Blumenbeet[Neighbors[j]], ref TemporaryResult);
                    }
                }

                //Schau nach, ob alle Farbpaare gesetz sind
                Kontrolle = 0;
                for (short i = 0; i < Farbpaare.GetLength(0); i++)
                {
                    if (Farbpaare[i, 3] == 0)
                    {
                        TemporaryResult = int.MinValue;
                        Kontrolle++;
                    }
                    Farbpaare[i, 3] = 0;
                }
                //Wenn nicht, wird überprüft, ob das weitere Vorgehen abgebrochen werden soll
                if (Kontrolle > (8 - Platz) * 2)
                {
                    return false;
                }

                //Schaue nach, ob alle Farben gesetzt wurden
                if (AnzahlFarben > AnzahlFarbenGesetzt)
                {
                    TemporaryResult = int.MinValue;
                }

                //Schaue nach, ob das Zwischenergebnis, immer noch das beste ist
                if (TemporaryResult > BestResult)
                {
                    BestResult = TemporaryResult;

                    for (int i = 0; i < BestBlumenbeet.Length; i++)
                    {
                        BestBlumenbeet[i] = Blumenbeet[i];
                    }
                }

                return true;
            }
        }
        private void CheckIfTwoColorsAreAPair(int[,] Farbpaare, int Blume1, int Blume2, ref int result)
        {
            for (short i = 0; i < Farbpaare.GetLength(0); i++)
            {
                if ((Farbpaare[i, 0] == Blume1 && Farbpaare[i, 1] == Blume2) || (Farbpaare[i, 0] == Blume2 && Farbpaare[i, 1] == Blume1))
                {
                    result += Farbpaare[i, 2];
                    Farbpaare[i, 3] = 1;
                }
            }
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