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

namespace SudokuPOO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextBox[] TextBoxes;

        public MainWindow()
        {
            InitializeComponent();
            this.TextBoxes = GenerateTextBoxes();
        }
        public TextBox[] GenerateTextBoxes()
        {
            TextBox[] TextBoxes = new TextBox[81];
            for (int i = 0; i <= 80; i++)
            {
                TextBox TBox = new TextBox();
                TBox.Margin = new Thickness(5 + (25 * (i % 9)), 5 + (25 * (i / 9)), 0, 0);
                TBox.Text = i.ToString();
                TBox.Width = 20;
                TBox.Height = 20;
                TBox.VerticalAlignment = VerticalAlignment.Top;
                TBox.HorizontalAlignment = HorizontalAlignment.Left;
                _ = MaGrille.Children.Add(TBox);
                TextBoxes[i] = TBox;
            }
            return TextBoxes;
        }
        public void DrawLines()
        {

        }

        private void Resoudre(object sender, RoutedEventArgs e)
        {
            // Liste d'entiers pour passer les valeurs 
            List<int> Valeurs = new List<int>();
            // Boucle sur les TextBoxes pour remplir la liste
            for (int i = 0; i <= 80; i++)
            {
                // On passe par une variable intermédiaire
                int valeur_i = 0;
                bool parse0 = int.TryParse(TextBoxes[i].Text, out valeur_i);
                Valeurs.Add(valeur_i);
            }

            Grille Sudoku = new Grille(Valeurs);
            bool SudokuPossible = Sudoku.Regles();
            if (!SudokuPossible)
            {
                var impossible = new info_popup();
                impossible.Owner = this;
                impossible.ShowDialog();
            }
            
            if (SudokuPossible && Sudoku.EstResolue())
            {
                Afficher(Sudoku);
                var termine = new info_popup();
                termine.Owner = this;
                termine.Title = "Terminé";
                termine.Bouton_ok.Content = "OK";
                termine.infotext.Text = "Grille résolue :)";
                termine.ShowDialog();

            }


            if (SudokuPossible && ! Sudoku.EstResolue()) // La grille n'est pas résolue, on va devoir faire des hypothèses.
            {
                int max_grilles = 55; // On limite le nombre de grilles générées pour ne pas planter.
                                      // 55 est une valeur obtenue par tâtonnement : dans le pire des cas (grille vide), en limitant à 55 grilles générées,
                                      // On arrive malgré tout à deux solutions. On peut donc déterminer dans n'importe quel cas si la grille a une ou plusieurs solutions.
                int compteur_grilles = 0;
                List<Grille> ListeGrillesResolues = new List<Grille>(); // On va faire la liste des solutions pour les compter.
                List<Grille> ListeGrillesAResoudre = new List<Grille>(); // On va stocker les grilles à résoudre.
                ListeGrillesAResoudre.Add(Sudoku);
                do
                {
                    Grille GrilleAResoudre = ListeGrillesAResoudre.Last();
                    ListeGrillesAResoudre.Remove(GrilleAResoudre);
                    (Grille GrilleA, Grille GrilleB) = Grille.Hypothese(GrilleAResoudre);
                    if (GrilleA.Regles())
                    {
                        if (GrilleA.EstResolue())
                        {
                            ListeGrillesResolues.Add(GrilleA);
                            compteur_grilles++;
                        }
                        else // La GrilleA est possible mais pas résolue
                        {
                            if (GrilleB.Regles())
                            {
                                if (GrilleB.EstResolue()) // La Grille B est résolue
                                {
                                    ListeGrillesResolues.Add(GrilleB);
                                    compteur_grilles++;
                                }
                                else // Aucune des grilles n'est résolue. On reste sur la branche A, plus déterministe, en plaçant la GrilleA en dernière dans la liste.
                                     // ça nous permettra de récupérer la GrilleA au prochain tour.
                                     // Même dans le pire cas (grille vide), procéder ainsi nous assure d'avoir au moins une case résolue par cycle d'hypothèses.
                                {
                                    ListeGrillesAResoudre.Add(GrilleB);
                                    ListeGrillesAResoudre.Add(GrilleA); 
                                    compteur_grilles++;
                                }
                            }
                        }
                    }
                } while (compteur_grilles < max_grilles && ListeGrillesAResoudre.Count > 0);
                if (ListeGrillesResolues.Count == 0)
                {
                    var impossible = new info_popup(); // Par défaut info_popup affiche le message "Grille Impossible"
                    impossible.Owner = this;
                    impossible.ShowDialog();
                }
                if (ListeGrillesResolues.Count == 1)
                {
                    Afficher(ListeGrillesResolues.Last());
                    var termine = new info_popup();
                    termine.Owner = this;
                    termine.Title = "Terminé";
                    termine.Bouton_ok.Content = "OK";
                    termine.infotext.Text = "Grille résolue :)";
                    termine.ShowDialog();

                }
                if (ListeGrillesResolues.Count > 1)
                {
                    Afficher(ListeGrillesResolues.Last());
                    var Plusieurs_Grilles = new info_popup();
                    Plusieurs_Grilles.Owner = this;
                    Plusieurs_Grilles.Title = "Plusieurs possibilités";
                    Plusieurs_Grilles.Bouton_ok.Content = "OK";
                    Plusieurs_Grilles.infotext.Text = "Il existe plusieurs solutions.\n En voici une.";
                    Plusieurs_Grilles.ShowDialog();
                }
            }
        }
        private void Afficher(Grille UneGrille)
        {
            List<int> ListeValeurs = UneGrille.Liste_Valeurs();
            for (int indice = 0; indice <= 80; indice++)
            {
                if (ListeValeurs[indice] != 0)
                { TextBoxes[indice].Text = ListeValeurs[indice].ToString(); }
            }
        }
        private void Effacer(object sender, RoutedEventArgs e)
        {
            foreach (TextBox TB in TextBoxes)
            { TB.Text = ""; }
        }
    }
}
