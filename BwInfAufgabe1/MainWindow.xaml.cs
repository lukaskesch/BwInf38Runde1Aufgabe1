﻿using System;
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
            string[] EingabeArray;
            int[,] FarbnummernUndHaeufigkeit = new int[9,2];
            int[,] Farbpaare;
            string EingabeString;
            int[,] Blumenbeet = CreateBlumenbeet();

            
            try
            {
                EingabeString = TextBoxInput.Text;
                EingabeArray = Regex.Split(EingabeString, "\r\n");

                Farbpaare = GetAllFarbpaare(EingabeArray);
                GetAllFarbnummern(Farbpaare, FarbnummernUndHaeufigkeit);
            }
            catch
            {
                MessageBox.Show("Die Eingabeparameter konnten nicht entgegengenommen werden");
                return;
            }

            Ausgabe(Farbpaare, FarbnummernUndHaeufigkeit);
        }

        private int[,] CreateBlumenbeet()
        {
            //Array für das Blumenbeet
            //Erste Spalte speichert den Nummerncode der Blumenfarbe und der Index gibt an welchem Steckplatz 
            //Zweite Spalte ist wie viele Nebenblumen ein Steckplatz hat
            int[,] Blumenbeet = new int[9, 2];

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
                //Es wird geschaut ob die Farbe aus dem Farbpaare Array (Spalte 1) schon im FarbnummernUndHaeufigkeit Array ist, wenn nicht wird diese hinzugefügt.
                //Außerdem wird der Zähler für die Farbe um eins erhöht
                int Index = GetIndexOfElementInArray(FarbnummernUndHaeufigkeit, Farbpaare[i, 0]);
                if (-1 == Index)
                {
                    Index = GetFirstFreeIndex(FarbnummernUndHaeufigkeit);
                    FarbnummernUndHaeufigkeit[Index, 0] = Farbpaare[i, 0];
                    FarbnummernUndHaeufigkeit[Index, 1] = 1;
                }
                else
                {
                    FarbnummernUndHaeufigkeit[Index, 1]++;
                }

                //Gleiche wie oben nur diesmal Spalte 2 des Farbpaar Arrays
                Index = GetIndexOfElementInArray(FarbnummernUndHaeufigkeit, Farbpaare[i, 1]);
                if (-1 == Index)
                {
                    Index = GetFirstFreeIndex(FarbnummernUndHaeufigkeit);
                    FarbnummernUndHaeufigkeit[Index, 0] = Farbpaare[i, 1];
                    FarbnummernUndHaeufigkeit[Index, 1] = 1;
                }
                else
                {
                    FarbnummernUndHaeufigkeit[Index, 1]++;
                }
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

        private void Ausgabe(int[,] Array1, int[,] Array2)
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

            MessageBox.Show(Ausgabe1);
            MessageBox.Show(Ausgabe2);
        }
    }
}