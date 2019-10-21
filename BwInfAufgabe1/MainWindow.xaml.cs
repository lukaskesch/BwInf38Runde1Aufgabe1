using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

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

        private void ButtonCalculate_Click(object sender, RoutedEventArgs e)
        {
            ClearVisualBlumenbeet();
            bool Dialogfenster = false;
            string EingabeString;
            string[] EingabeArray;
            int AnzahlFarben;
            int DifferenzFarben;
            int[,] Farben = new int[7, 9];
            for (int i = 0; i < 7; i++)
            {
                Farben[i, 1] = -1;
                Farben[i, 2] = -1;
            }
            int[,] Farbpaare;
            int[,] Blumenbeet = CreateBlumenbeet();

            if (CheckboxAusgabe.IsChecked == true)
            {
                Dialogfenster = true;
            }

            try
            {
                EingabeString = TextBoxInput.Text;
                EingabeArray = Regex.Split(EingabeString, "\r\n");

                //Es wird ermittelt, wie viele unterschiedliche Farben erwünscht sind
                AnzahlFarben = int.Parse(EingabeArray[0]);

                //Farbpaar Array und Farben Array werden gefüllt
                Farbpaare = GetAllFarbpaare(EingabeArray);
                GetAllFarbnummern(Farbpaare, Farben);

                //Ausnahme, es wurden mehr Farben in Farbpaaren angegeben als Farben insgesammt erwünscht
                int FarbenZuViel = GetFirstFreeIndex(Farben) - int.Parse(EingabeArray[0]);

                if (Dialogfenster)
                {
                    MessageBox.Show("Farben zu viel: " + FarbenZuViel.ToString());
                }

                while (FarbenZuViel > 0)
                {
                    //Ermittle Farbe die am wertlosesten ist
                    int indexFarbe = GetFarbeWithLowestPoints(Farben);
                    MessageBox.Show(indexFarbe.ToString());

                    //Lösche alle Farbpaare mit ihr
                    DeleteFarbpaare(Farbpaare, Farben[indexFarbe, 0]);

                    FarbenZuViel--;
                }


            }
            catch
            {
                MessageBox.Show("Die Eingabeparameter konnten nicht entgegengenommen werden");
                return;
            }

            //Farbenarray mit Farben auffüllen, bis die Anzahl an erwünschten Farben erreicht ist
            Ausgabe(Dialogfenster, Farbpaare, Farben, Blumenbeet);
            if (GetFirstFreeIndex(Farben) != -1)
            {
                DifferenzFarben = AnzahlFarben - GetFirstFreeIndex(Farben);
                if (DifferenzFarben > 0)
                {
                    FillFarben(DifferenzFarben, Farben);
                }

                if (Dialogfenster)
                {
                    MessageBox.Show("Differenz Farben: " + DifferenzFarben.ToString());
                }
            }

            //Blumenbeet mit diesen Farben auffüllen
            for (int i = 0; i < 7; i++)
            {
                if (Farben[i, 1] == 0)
                {
                    SetBlumeInBlumenbeet(Blumenbeet, GetLowestPointInBlumenbeet(Blumenbeet), Farben[i, 0]);
                }
            }



            FillBlumenbeet(Farben, Farbpaare, Blumenbeet, 8);

            MessageBox.Show("Das erstellt Blumenbeet hat eine Gesammtpunktzahl von: " + CalculateResult(Blumenbeet, Farbpaare));
        }
        private void FillBlumenbeet(int[,] Farben, int[,] Farbpaare, int[,] Blumenbeet, int Platz)
        {
            //System.Threading.Thread.Sleep(1000);
            if (Platz >= 9 || Platz < 0)
            {
                //Check result


                return;
            }
            else
            {
                //Gehe alle Farben durch für Platz
                for (int i = 0; i < Farben.GetLength(0); i++)
                {
                    //Choose - Setze Blume auf übergebenen Platz
                    SetBlumeInBlumenbeet(Blumenbeet, Platz, Farben[i, 0]);

                    //Explore - Setze alle möglichen Blumen auf den nächsten Platz
                    for (int j = 0; j < Farben.GetLength(0); j++)
                    {
                        FillBlumenbeet(Farben, Farbpaare, Blumenbeet, Platz + 1);
                    }

                    //Unchose - Gehe ein Blumenplatz zurueck
                    //Platz -= 1;
                }
                //Gehe einen Platz zurück
                if (Platz <= 0)
                {
                    return;
                }
                else
                {
                    FillBlumenbeet(Farben, Farbpaare, Blumenbeet, Platz - 1);
                }
            }
        }




        private void SetNachbarblumen(int[,] Farben, int index, int Farbe1, int Farbe2)
        {
            if (Farben[index, 0] != Farbe1)
            {
                Farben[index, GetFirstFreeIndexOfRow(Farben, index)] = Farbe1;
            }
            if (Farben[index, 0] != Farbe2)
            {
                Farben[index, GetFirstFreeIndexOfRow(Farben, index)] = Farbe2;
            }
        }
        private int GetFirstFreeIndexOfRow(int[,] Array, int Row)
        {
            for (int i = 0; i < Array.GetLength(1); i++)
            {
                if (Array[Row, i] == 0)
                {
                    return i;
                }
            }
            return -1;
        }
        private int GetLowestPointInBlumenbeet(int[,] Blumenbeet)
        {
            //Ermittelt die Anzahl an niedrigster Nachbarn an einem unbekannten Platz
            int LowestNeighbours = 6;
            for (int i = 0; i < 9; i++)
            {
                if (Blumenbeet[i, 0] == -1)
                {
                    if (Blumenbeet[i, 1] < LowestNeighbours)
                    {
                        LowestNeighbours = Blumenbeet[i, 1];
                    }
                }
            }

            //Ermittelt den dazugehörigen Platz und gibt diesen zurück
            int index = 0;
            for (int i = 0; i < 9; i++)
            {
                if (Blumenbeet[i, 1] == LowestNeighbours && Blumenbeet[i, 0] == -1)
                {
                    return index;
                }
                index++;
            }
            return -1;
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

                    SetNachbarblumen(FarbnummernUndHaeufigkeit, Index, Farbpaare[i, 0], Farbpaare[i, 1]);


                }
                else
                {
                    FarbnummernUndHaeufigkeit[Index, 1]++;
                    FarbnummernUndHaeufigkeit[Index, 2] += Farbpaare[i, 2];
                    SetNachbarblumen(FarbnummernUndHaeufigkeit, Index, Farbpaare[i, 0], Farbpaare[i, 1]);

                }

                //Gleiche wie oben nur diesmal Spalte 2 des Farbpaar Arrays
                Index = GetIndexOfElementInArray(FarbnummernUndHaeufigkeit, Farbpaare[i, 1]);
                if (-1 == Index)
                {
                    Index = GetFirstFreeIndex(FarbnummernUndHaeufigkeit);
                    FarbnummernUndHaeufigkeit[Index, 0] = Farbpaare[i, 1];
                    FarbnummernUndHaeufigkeit[Index, 1] = 1;
                    FarbnummernUndHaeufigkeit[Index, 2] = Farbpaare[i, 2];
                    SetNachbarblumen(FarbnummernUndHaeufigkeit, Index, Farbpaare[i, 0], Farbpaare[i, 1]);

                }
                else
                {
                    FarbnummernUndHaeufigkeit[Index, 1]++;
                    FarbnummernUndHaeufigkeit[Index, 2] += Farbpaare[i, 2];
                    SetNachbarblumen(FarbnummernUndHaeufigkeit, Index, Farbpaare[i, 0], Farbpaare[i, 1]);

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
        private void DeleteFarbpaare(int[,] Farbpaare, int Farbe)
        {
            for (int i = 0; i < Farbpaare.GetLength(0); i++)
            {
                if (Farbpaare[i, 0] == Farbe || Farbpaare[i, 1] == Farbe)
                {
                    for (int j = i; j < Farbpaare.GetLength(0) - i + 1; j++)
                    {
                        if (j < Farbpaare.GetLength(0) - 1)
                        {
                            Farbpaare[j, 0] = Farbpaare[j + 1, 0];
                            Farbpaare[j, 1] = Farbpaare[j + 1, 0];
                            Farbpaare[j, 2] = Farbpaare[j + 1, 0];
                        }


                        else
                        {
                            Farbpaare[Farbpaare.GetLength(0) - 1, 0] = 0;
                            Farbpaare[Farbpaare.GetLength(0) - 1, 1] = 0;
                            Farbpaare[Farbpaare.GetLength(0) - 1, 2] = 0;
                        }
                        try
                        {
                            if (Farbpaare[j + 2, 0] == 0)
                            {
                                Farbpaare[j + 1, 0] = 0;
                                Farbpaare[j + 1, 1] = 0;
                                Farbpaare[j + 1, 2] = 0;
                            }
                        }
                        catch (Exception)
                        {
                            break;
                        }

                        //Schaue doppelten platz und setze diesen 0
                    }
                }
            }
        }
        private int GetFarbeWithLowestPoints(int[,] Farben)
        {
            int WenigstenPunkte = 6;
            int WenigstenPunkteIndex = 0;

            //Ermittelt die Farbe mit den wenigsten Farbkombinationen
            for (int i = 0; i < 7; i++)
            {
                if (WenigstenPunkte > Farben[i, 2] && Farben[i, 2] > 0)
                {
                    WenigstenPunkte = Farben[i, 2];
                    WenigstenPunkteIndex = i;
                }
                //Wenn zwei Blumen den gleichen Wert haben, wird die ausgewählt, die in mehr Farbpaaren vorkommt
                else if (WenigstenPunkte == Farben[i, 2])
                {
                    if (Farben[WenigstenPunkteIndex, 1] < Farben[i, 1])
                    {
                        WenigstenPunkte = Farben[i, 1];
                        WenigstenPunkteIndex = i;
                    }
                }
            }
            return WenigstenPunkteIndex;
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
        private void SetBlumeInBlumenbeet(int[,] Blumenbeet, int Platz, int Blume)
        {
            if (Blume == 0)
            {
                return;
            }
            Blumenbeet[Platz, 0] = Blume;
            SetBlumeToFarbe(Platz, Blume);
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
            int index = 0;
            for (int i = 0; i < 6; i++)
            {
                if (Blumenbeet[Index, i + 2] <= Index)
                {
                    continue;
                }
                Neighbors[index] = Blumenbeet[Index, i + 2];
                index++;

            }

            string Ausgabe1 = Index.ToString() + "\n";
            for (int a = 0; a < Neighbors.GetLength(0); a++)
            {
                Ausgabe1 += Neighbors[a] + "\n";
            }
            MessageBox.Show(Ausgabe1);

            return Neighbors;
        }
        private int CalculateResult(int[,] Blumenbeet, int[,] Farbpaare)
        {
            int result = 0;

            //Gehe durch das Blumenbeet
            for (int i = 0; i < Blumenbeet.GetLength(0); i++)
            {
                //Holt sich die Nachbarn jeder einzelnen Blume
                int[] Neighbors = GetHigherNeighborBlumenOfIndex(Blumenbeet, i);

                for (int j = 0; j < Neighbors.Length; j++)
                {
                    if (j > 0 && Neighbors[j] == 0)
                    {
                        continue;
                    }

                    CheckIfTwoColorsAreAPair(Farbpaare, Blumenbeet[i, 0], Blumenbeet[Neighbors[j], 0], ref result);
                }
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

    }
}