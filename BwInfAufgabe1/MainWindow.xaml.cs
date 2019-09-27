using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            int[] Farbnummern;
            int[] Farbpaare;
            string EingabeString;
            int[,] Blumenbeet = CreateBlumenbeet();

            //Konvertiere die Eingabe in das EingabeArray
            EingabeString = TextBoxInput.Text;
            EingabeArray = EingabeString.Split('\n');
            try
            {
                EingabeString = TextBoxInput.Text;
                EingabeArray = EingabeString.Split('\n');

                Farbpaare = GetFarbpaare(EingabeArray);
            }
            catch
            {
                return;
            }
        }

        

        private int[,] CreateBlumenbeet()
        {
            //Array für das Blumenbeet
            //Erste Spalte speichert den Nummerncode der Blumenfarbe und der Index gibt an welchem Steckplatz 
            //Zweite Spalte ist wie viele Nebenblumen ein Steckplatz hat
            int[,] Blumenbeet = new int[9,2];

            //Belegt jeden Index der Blumennummer spalte mit einer Prüfziffer
            for(int i = 0; i<Blumenbeet.Length; i++)
            {
                Blumenbeet[i, 0] = -1;
            }

            //Definiert wie viele Nebenblumen ein Steckplatz hat
            Blumenbeet[1, 0] = 2;
            Blumenbeet[1, 1] = 4;
            Blumenbeet[1, 2] = 4;
            Blumenbeet[1, 3] = 3;
            Blumenbeet[1, 4] = 6;
            Blumenbeet[1, 5] = 3;
            Blumenbeet[1, 6] = 4;
            Blumenbeet[1, 7] = 4; 
            Blumenbeet[1, 8] = 2; 


            return Blumenbeet;
        }

        private int BlumenNameToBlumenNummer(string Blumenname)
        {
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
                    return -1;
            }
        }
    }
}