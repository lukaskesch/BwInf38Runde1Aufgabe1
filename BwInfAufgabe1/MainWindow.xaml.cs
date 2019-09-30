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
            string EingabeString;
            string[] EingabeArray;
            int AnzahlFarben;
            int DifferenzFarben;
            int[,] Farben = new int[7, 9];
            for(int i = 0; i < 7; i++)
            {
                Farben[i, 1] = -1;
                Farben[i, 2] = -1;
            }
            int[,] Farbpaare;
            int[,] Blumenbeet = CreateBlumenbeet();

            
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
                    //Lösche alle Farbpaare mit ihr
                }

                MessageBox.Show("Farben zu viel: " + FarbenZuViel.ToString());
            }
            catch
            {
                MessageBox.Show("Die Eingabeparameter konnten nicht entgegengenommen werden");
                return;
            }

            //Farbenarray mit Farben auffüllen, bis die Anzahl an erwünschten Farben erreicht ist
            Ausgabe(Farbpaare, Farben, Blumenbeet);
            if(GetFirstFreeIndex(Farben) != -1)
            {
                DifferenzFarben = AnzahlFarben - GetFirstFreeIndex(Farben);
                if (DifferenzFarben > 0)
                {
                    FillFarben(DifferenzFarben, Farben);
                }
                MessageBox.Show("Differenz Farben: " + DifferenzFarben.ToString());
            }

            //Blumenbeet mit diesen Farben auffüllen
            for (int i = 0; i < 7; i++)
            {
                if(Farben[i, 1] == 0)
                {
                    SetBlumeInBlumenbeet(Blumenbeet, GetLowestPointInBlumenbeet(Blumenbeet), Farben[i, 0]);
                }
            }

            //Blume, die in den meisten Farbpaaren enthalten ist, in die Mitte
            SetMiddelBlume(Farben, Blumenbeet);

            Ausgabe(Farbpaare, Farben, Blumenbeet);
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
            int[,] Farbpaare = new int[lengh, 3];
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
        private void SetNachbarblumen(int[,] Farben, int index,  int Farbe1, int Farbe2)
        {
            if(Farben[index,0] != Farbe1)
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
                if (Array[i,0] == Element) { return i; }
            }
            return -1;
        }
        private int GetFirstFreeIndex(int[,] Array)
        {
            //Diese Methode gibt den ersten freien Index eines Arrays zurück
            for (int i = 0; i < Array.GetLength(0); i++)
            {
                if (Array[i,0] == 0)
                {
                    return i;
                }
            }
            return -1;
        }
        private int GetFirstFreeIndexOfRow(int[,] Array, int Row)
        {
            for(int i = 0; i < Array.GetLength(1); i++)
            {
                if(Array[Row, i] == 0)
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
        private void Ausgabe(int[,] Array1, int[,] Array2, int[,] Blumenbeet)
        {
            //Diese Methode gibt alle Werte der beiden Arrays aus
            string Ausgabe1 = string.Empty;
            for(int i = 0; i< Array1.GetLength(0); i++)
            {
                for(int j = 0; j < Array1.GetLength(1); j++)
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
            for(int i = 0; i < 9; i++)
            {
                if(Blumenbeet[i,0] == -1)
                {
                    if(Blumenbeet[i,1] < LowestNeighbours)
                    {
                        LowestNeighbours = Blumenbeet[i, 1];
                    }
                }
            }

            //Ermittelt den dazugehörigen Platz und gibt diesen zurück
            int index = 0;
            for(int i = 0; i < 9; i++)
            {
                if(Blumenbeet[i,1] == LowestNeighbours && Blumenbeet[i,0] == -1)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
        private void SetBlumeInBlumenbeet(int[,] Blumenbeet, int Platz, int Blume)
        {
            Blumenbeet[Platz, 0] = Blume;
            SetBlumeToFarbe(Platz, Blume);
        }
        private void SetMiddelBlume(int[,] Farben, int[,] Blumenbeet)
        {
            int MeistenPartner = 0;
            int MeistenPartnerFarbnummer = -1;
            int MeistenPartnerIndex = 0;

            //Ermittelt die Farbe mit den meisten Farbkombinationen
            for(int i = 0; i < 7; i++)
            {
                if(MeistenPartner < Farben[i, 1])
                {
                    MeistenPartner = Farben[i, 1];
                    MeistenPartnerFarbnummer = Farben[i, 0];
                    MeistenPartnerIndex = i;
                }
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
        }
    }
}