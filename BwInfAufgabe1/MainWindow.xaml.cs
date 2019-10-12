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

                while (FarbenZuViel > 0)
                {
                    //Ermittle Farbe die am wertlosesten ist
                    int index = GetFarbeWithLowestPoints(Farben);
                    MessageBox.Show(index.ToString());

                    //Lösche alle Farbpaare mit ihr


                    FarbenZuViel--;
                }

                if (Dialogfenster)
                {
                    MessageBox.Show("Farben zu viel: " + FarbenZuViel.ToString());
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

            //Blume, die in den meisten Farbpaaren enthalten ist, in die Mitte
            int MiddleBlume = SetMiddelBlume(Farben, Blumenbeet);

            //Setze Partner der Mittelblume
            SetPartnersOfMiddleBlume(Farben, Farbpaare, Blumenbeet, MiddleBlume);

            //Setze die restlichen Farbpaare
            SetRestOfFarbpaare(Blumenbeet, Farbpaare);


            //Regel: Wenn schon alle Farbpaare verbraucht, dann restliche Indizes mit glichem Nachbaaranzahl mit der Farbe des Index, der am meisten Punkte gibt, auffüllen

            FillEmptyIndizies(Blumenbeet, Farbpaare);
            FillEmptyIndizies(Blumenbeet, Farbpaare);

            Ausgabe(Dialogfenster, Farbpaare, Farben, Blumenbeet);
        }
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
        private int GetFarbeWithLowestPoints(int[,] Farben)
        {
            int WenigstenPunkte = 6;
            int WenigstenPunkteIndex = 0;

            //Ermittelt die Farbe mit den wenigsten Farbkombinationen
            for (int i = 0; i < 7; i++)
            {
                if (WenigstenPunkte > Farben[i, 2])
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
        private void SetBlumeInBlumenbeet(int[,] Blumenbeet, int Platz, int Blume)
        {
            if (Blume > 0)
            {
                return;
            }
            Blumenbeet[Platz, 0] = Blume;
            SetBlumeToFarbe(Platz, Blume);
        }
        private int SetMiddelBlume(int[,] Farben, int[,] Blumenbeet)
        {
            int MeistenPartner = 0;
            int MeistenPartnerFarbnummer = -1;
            int MeistenPartnerIndex = 0;

            //Ermittelt die Farbe mit den meisten Farbkombinationen
            for (int i = 0; i < 7; i++)
            {
                if (MeistenPartner < Farben[i, 1])
                {
                    MeistenPartner = Farben[i, 1];
                    MeistenPartnerFarbnummer = Farben[i, 0];
                    MeistenPartnerIndex = i;
                }
                //Wenn zwei Blumen gleich viele Partner haben, wird geschaut welche Blume mehr Punkte ergibt
                else if (MeistenPartner == Farben[i, 1])
                {
                    if (Farben[MeistenPartnerIndex, 2] < Farben[i, 2])
                    {
                        MeistenPartner = Farben[i, 1];
                        MeistenPartnerFarbnummer = Farben[i, 0];
                        MeistenPartnerIndex = i;
                    }
                }
            }

            SetBlumeInBlumenbeet(Blumenbeet, 4, MeistenPartnerFarbnummer);

            return MeistenPartnerFarbnummer;
        }
        private void SetPartnersOfMiddleBlume(int[,] Farben, int[,] Farbpaare, int[,] Blumenbeet, int MiddleBlume)
        {
            //Alle Parner von Middle Blume 
            int[,] Partners = FindPartnersOfMidddleBlume(Farbpaare, Farben, MiddleBlume);

            //Sortiere Array (Je mehr Parner diese haben, desto besseren Platz, wenn gleichstand, dann der der mehr Punkte hat)
            SortArray(ref Partners, 2, 1);

            string Ausgabe4 = string.Empty;
            for (int i = 0; i < Partners.GetLength(0); i++)
            {
                for (int j = 0; j < Partners.GetLength(1); j++)
                {
                    Ausgabe4 += Partners[i, j] + " ";
                }
                Ausgabe4 += "\n";
            }
            MessageBox.Show("x\n" + Ausgabe4);

            //Setze erste Blume
            SetBlumeInBlumenbeet(Blumenbeet, 7, Partners[0, 0]);

            //Vermerke, dass diese Blume (und Farbpaar gesetzt wurde)
            Partners[0, 3] = 1;
            MarkThatBlumenPaarIsUsed(Farbpaare, Partners[0, 0], Blumenbeet[4, 1], false, -1);

            //Setze zweite Blume, wenn vorhanden (Wenn diese keinen weiteren Partner hat, dann auf schlechtesten Index)
            if (GetFirstFreeIndex(Partners) > 1)
            {
                //Schaue, ob zweite Blume einen Partner hat
                if (Partners[1, 2] > 1)
                {
                    //Schaue, ob zweiter Partner, mit erstem Partner einen Gemeinsamen Partner haben
                    int Partner = CheckIfTwoFarbenHaveAnOtherPartnerTogether(Farben, Partners[0, 0], Partners[1, 0], Blumenbeet[4, 1]);
                    if (Partner > 0)
                    {
                        //Setze zweite Blume
                        SetBlumeInBlumenbeet(Blumenbeet, 6, Partners[1, 0]);

                        //Vermerke, dass diese Blume (und Farbpaar gesetzt wurde)
                        Partners[1, 3] = 1;
                        MarkThatBlumenPaarIsUsed(Farbpaare, Partners[1, 0], Blumenbeet[4, 1], false, -1);

                        //Setze die Partner Blume von Blume 1 und Blume2
                        SetBlumeInBlumenbeet(Blumenbeet, 8, Partner);

                        //Vermerke, dass das Farbpaar von Blume1 - Partner und Blume2 - Partner gesetzt wurde
                        MarkThatBlumenPaarIsUsed(Farbpaare, Partners[1, 0], Partner, true, Partners[0, 0]);

                    }
                    else
                    {
                        //Setze zweite Blume
                        SetBlumeInBlumenbeet(Blumenbeet, 1, Partners[1, 0]);

                        //Vermerke, dass diese Blume (und Farbpaar gesetzt wurde)
                        Partners[1, 3] = 1;
                        MarkThatBlumenPaarIsUsed(Farbpaare, Partners[1, 0], Blumenbeet[4, 1], false, -1);
                    }
                }
                else
                {
                    //Setze zweite Blume auf schlechtesten Index
                    int Index = GetWorstNeighborIndex(Blumenbeet, 4);
                    SetBlumeInBlumenbeet(Blumenbeet, Index, Partners[1, 0]);

                    //Vermerke, dass diese Blume (und Farbpaar gesetzt wurde)
                    Partners[1, 3] = 1;
                    MarkThatBlumenPaarIsUsed(Farbpaare, Partners[1, 0], Blumenbeet[4, 1], false, -1);
                }
            }

            //Setze dritte Blume, wenn vorhanden, auf den schlechtesten noch freien Index
            if (GetFirstFreeIndex(Partners) > 2)
            {
                //Setze dritte Blume auf schlechtesten Index
                int Index = GetWorstNeighborIndex(Blumenbeet, 4);
                SetBlumeInBlumenbeet(Blumenbeet, Index, Partners[2, 0]);

                //Vermerke, dass diese Blume (und Farbpaar gesetzt wurde)
                Partners[2, 3] = 1;
                MarkThatBlumenPaarIsUsed(Farbpaare, Partners[2, 0], Blumenbeet[4, 1], false, -1);
            }
        }
        private int[,] FindPartnersOfMidddleBlume(int[,] Farbpaare, int[,] Farben, int MiddleBlume)
        {
            //Findet die Partner der Mittleren Blume
            int[,] Partner = new int[8, 4];

            for (int i = 0; i < Farbpaare.GetLength(0); i++)
            {
                if (Farbpaare[i, 0] == MiddleBlume)
                {
                    int index = GetFirstFreeIndex(Partner);
                    Partner[index, 0] = Farbpaare[i, 1];
                    Partner[index, 1] = Farbpaare[i, 2];

                    //Findet herraus, wie viele Partner, die wiederum haben
                    for (int j = 0; j < Farben.GetLength(0); j++)
                    {
                        if (Farben[j, 0] == Farbpaare[i, 1])
                        {
                            Partner[index, 2] = Farben[j, 1];
                            break;
                        }
                    }
                }
                if (Farbpaare[i, 1] == MiddleBlume)
                {
                    int index = GetFirstFreeIndex(Partner);
                    Partner[index, 0] = Farbpaare[i, 0];
                    Partner[index, 1] = Farbpaare[i, 2];

                    //Findet herraus, wie viele Partner, die wiederum haben
                    for (int j = 0; j < Farben.GetLength(0); j++)
                    {
                        if (Farben[j, 0] == Farbpaare[i, 0])
                        {
                            Partner[index, 2] = Farben[j, 1];
                            break;
                        }
                    }
                }
            }


            return Partner;
        }
        private void SortArray(ref int[,] Array, int Column, int SecondColumn)
        {
            int Zwischenwert1, Zwischenwert2, Zwischenwert3;

            // traverse 0 to array length 
            for (int i = 0; i < 8 - 1; i++)
            {
                // traverse i+1 to array length 
                for (int j = i + 1; j < 8; j++)
                {
                    // compare array element with  
                    // all next element 
                    if (Array[i, Column] < Array[j, Column])
                    {
                        Zwischenwert1 = Array[i, 0];
                        Array[i, 0] = Array[j, 0];
                        Array[j, 0] = Zwischenwert1;

                        Zwischenwert2 = Array[i, 1];
                        Array[i, 1] = Array[j, 1];
                        Array[j, 1] = Zwischenwert2;

                        Zwischenwert3 = Array[i, 2];
                        Array[i, 2] = Array[j, 2];
                        Array[j, 2] = Zwischenwert3;
                    }

                    if (Array[i, Column] == Array[j, Column])
                    {
                        if (Array[i, SecondColumn] < Array[j, SecondColumn])
                        {
                            Zwischenwert1 = Array[i, 0];
                            Array[i, 0] = Array[j, 0];
                            Array[j, 0] = Zwischenwert1;

                            Zwischenwert2 = Array[i, 1];
                            Array[i, 1] = Array[j, 1];
                            Array[j, 1] = Zwischenwert2;

                            Zwischenwert3 = Array[i, 2];
                            Array[i, 2] = Array[j, 2];
                            Array[j, 2] = Zwischenwert3;
                        }
                    }
                }
            }




        }
        private bool IsBlumenbeetIndexFree(int[,] Blumenbeet, int index)
        {
            if (Blumenbeet[index, 0] == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void MarkThatBlumenPaarIsUsed(int[,] Farbpaare, int Blume1, int Blume2, bool DreiFachpaar, int Blume3)
        {
            for (int i = 0; i < Farbpaare.GetLength(0); i++)
            {
                if ((Farbpaare[i, 0] == Blume1 && Farbpaare[i, 1] == Blume2) || (Farbpaare[i, 1] == Blume1 && Farbpaare[i, 0] == Blume2))
                {
                    Farbpaare[i, 3] = 1;
                }
                if (DreiFachpaar)
                {
                    if ((Farbpaare[i, 0] == Blume2 && Farbpaare[i, 1] == Blume3) || (Farbpaare[i, 1] == Blume2 && Farbpaare[i, 0] == Blume3))
                    {
                        Farbpaare[i, 3] = 1;
                    }
                }


            }
        }
        private int CheckIfTwoFarbenHaveAnOtherPartnerTogether(int[,] Farben, int Farbe1, int Farbe2, int Partner)
        {
            //Ermittle die Indizes der Beiden Farben, im Array
            int IndexFarbe1 = -1, IndexFarbe2 = -1, IndexPartner = -1;
            for (int i = 0; i < Farben.GetLength(0); i++)
            {
                if (Farbe1 == Farben[i, 0])
                {
                    IndexFarbe1 = i;
                }
                else if (Farbe2 == Farben[i, 0])
                {
                    IndexFarbe2 = i;
                }
                else if (Partner == Farben[i, 0])
                {
                    IndexPartner = i;
                }
            }

            //Überprüfe, ob diese einen gemeinsamen Partner haben
            for (int i = 3; i < 9; i++)
            {
                for (int j = 3; j < 9; j++)
                {
                    if (Farben[IndexFarbe1, i] == Farben[IndexFarbe2, j] && Farben[IndexFarbe2, j] != Farben[IndexPartner, 0] && Farben[IndexFarbe2, j] != 0)
                    {
                        return Farben[IndexFarbe1, i];
                    }
                }
            }

            //Wenn nicht, wird -1 zurückgegeben
            return -1;

        }
        private int GetWorstNeighborIndex(int[,] Blumenbeet, int StartIndex)
        {
            int TempIndex;
            int Index = -1;
            int NeighboorsOfIndex = 10;

            for (int i = 2; i < 8; i++)
            {
                TempIndex = Blumenbeet[StartIndex, i];
                if (Blumenbeet[TempIndex, 0] == -1 && Blumenbeet[TempIndex, 1] < NeighboorsOfIndex)
                {
                    if (TempIndex == 0 && i > 3)
                    {
                        return Index;
                    }
                    Index = TempIndex;
                    NeighboorsOfIndex = Blumenbeet[TempIndex, 1];
                }
            }

            return Index;
        }
        private void SetRestOfFarbpaare(int[,] Blumenbeet, int[,] Farbpaare)
        {
            //Gehe alle Farbpaare durch
            for (int i = 0; i < Farbpaare.GetLength(0); i++)
            {
                //Schaue, ob ein Farbpaar noch nicht belegt ist
                if (Farbpaare[i, 3] != 1)
                {
                    int Farbe1 = Farbpaare[i, 0];
                    int Farbe2 = Farbpaare[i, 1];
                    int FarbeSetzen;
                    int Index;

                    //Gehe das Blumenbeet durch
                    for (int j = 0; j < Blumenbeet.GetLength(0); j++)
                    {
                        //Schaue, ob eine der Farben im Farbpaar schon im Blumenbeet vorhanden ist
                        if (Blumenbeet[j, 0] == Farbe1)
                        {
                            FarbeSetzen = Farbe2;
                        }
                        else if (Blumenbeet[j, 0] == Farbe2)
                        {
                            FarbeSetzen = Farbe1;
                        }
                        else
                        {
                            continue;
                        }

                        //Schaue, ob schon alle Farbpaare gesetzt wurden
                        if (CheckIfAllFarbpaareAreSet(Farbpaare))
                        {
                            return;
                        }

                        //Ermittle den Index für die zweite (nicht gesetzte) Blume im Farbpaar
                        Index = GetWorstNeighborIndex(Blumenbeet, j);

                        //MessageBox.Show("i: " + i + "\nj: " + j + "\n" + Index);

                        //Setze erste Blume
                        SetBlumeInBlumenbeet(Blumenbeet, Index, FarbeSetzen);

                        //Vermerke, dass diese Blume (und Farbpaar gesetzt wurde)
                        MarkThatBlumenPaarIsUsed(Farbpaare, Farbe1, Farbe2, false, -1);

                        //string Ausgabe1 = string.Empty;
                        //for (int a = 0; a < Farbpaare.GetLength(0); a++)
                        //{
                        //    for (int b = 0; b < Farbpaare.GetLength(1); b++)
                        //    {
                        //        Ausgabe1 += Farbpaare[a, b] + " ";
                        //    }
                        //    Ausgabe1 += "\n";
                        //}
                        //MessageBox.Show(Ausgabe1);

                        //Wenn noch nicht alle Farbpaare geseetzt sind, wird diese Methode neu gestartet
                        if (!CheckIfAllFarbpaareAreSet(Farbpaare))
                        {
                            SetRestOfFarbpaare(Blumenbeet, Farbpaare);
                        }
                    }
                }
            }
        }
        private bool CheckIfAllFarbpaareAreSet(int[,] Farbpaare)
        {
            for (int i = 0; i < Farbpaare.GetLength(0); i++)
            {
                if (Farbpaare[i, 3] != 1)
                {
                    return false;
                }
            }
            return true;
        }
        private void FillEmptyIndizies(int[,] Blumenbeet, int[,] Farbpaare)
        {
            //Gehe durch das BLumenbeet
            for (int i = 0; i < Blumenbeet.GetLength(0); i++)
            {
                //Schaut, ob Platz noch nicht belegt
                if (Blumenbeet[i, 0] == -1)
                {
                    //Holt sich die Nachbarplätze
                    int[] Neighbors = GetNeighborBlumenOfIndex(Blumenbeet, i);
                    int[,] NeighborsFarben = new int[4, 2];

                    //Ermittelt für jeden dieser Nachbarplätze die beste Farbe
                    for (int j = 0; j < 4; j++)
                    {
                        GetBestNeighborFlower(Blumenbeet, Farbpaare, NeighborsFarben, Neighbors[j], j);
                    }

                    //Schaue, ob zwei Farben doppelt vorkommen
                    CheckIfTwoColorsAreIdentical(ref NeighborsFarben, Blumenbeet);

                    //Ermittle insgesammt beste Blume
                    int Blume = GetBestFlower(NeighborsFarben);

                    //Setze diese Blume
                    SetBlumeInBlumenbeet(Blumenbeet, i, Blume);
                }
            }
        }
        private int[] GetNeighborBlumenOfIndex(int[,] Blumenbeet, int Index)
        {
            int[] Neighbors = new int[4];
            for (int i = 0; i < 4; i++)
            {
                Neighbors[i] = Blumenbeet[Index, i + 2];
            }
            return Neighbors;
        }
        private void GetBestNeighborFlower(int[,] Blumenbeet, int[,] Farbpaare, int[,] NeighborsFarben, int Index, int Index2)
        {
            //Ermittle Farbe zu dem entsprechenden Index
            int Farbe = Blumenbeet[Index, 0];
            int PartnerFarbe = -1;
            int PartnerFarbePunkte = -1;

            //Ermittle für diese Farbe, den besten Partner
            for (int i = 0; i < Farbpaare.GetLength(0); i++)
            {
                if (Farbpaare[i, 0] == Farbe)
                {
                    if (Farbpaare[i, 2] > PartnerFarbePunkte)
                    {
                        PartnerFarbe = Farbpaare[i, 1];
                        PartnerFarbePunkte = Farbpaare[i, 2];
                    }
                }
                if (Farbpaare[i, 1] == Farbe)
                {
                    if (Farbpaare[i, 2] > PartnerFarbePunkte)
                    {
                        PartnerFarbe = Farbpaare[i, 0];
                        PartnerFarbePunkte = Farbpaare[i, 2];
                    }
                }
            }

            if (PartnerFarbe == -1)
            {
                return;
            }
            NeighborsFarben[Index2, 0] = PartnerFarbe;
            NeighborsFarben[Index2, 1] = PartnerFarbePunkte;
        }
        private void CheckIfTwoColorsAreIdentical(ref int[,] Array, int[,] Blumenbeet)
        {
            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(0); j++)
                {
                    if (Array[i, 0] == Array[j, 0]) //&& Array[j, 0] != Blumenbeet[4, 0]
                    {
                        Array[i, 1] += Array[j, 1];
                        Array[j, 1] = Array[i, 1];
                    }
                }
            }
        }
        private int GetBestFlower(int[,] Farbliste)
        {
            int Blume = -1;
            int BlumenWert = -1;

            for (int i = 0; i < Farbliste.GetLength(0); i++)
            {
                if (Farbliste[i, 1] > BlumenWert)
                {
                    BlumenWert = Farbliste[i, 1];
                    Blume = Farbliste[i, 0];
                }
            }

            return Blume;
        }
    }
}