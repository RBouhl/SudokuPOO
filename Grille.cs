using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SudokuPOO
{
    // Classe "Grille". Représente une grille de sudoku. 
    public class Grille
    {
        private List<int>[] _Valeurs;  // Une grille est un tableau de liste. Tableau de 10 listes.
                                       // La première liste (indice 0) est la liste des cases résolues.
                                       // La Nième liste est la liste des cases pour laquelle la valeur N est possible.

        public Grille() // Constructeur de grille vide. Dans une grille vide, toutes les valeurs sont possibles partout.

        {
            _Valeurs = new List<int>[10];
            _Valeurs[0] = new List<int>();
            for (int valeur = 1; valeur <= 9; valeur++)
            {
                _Valeurs[valeur] = new List<int>();
                for (int IndiceCase = 0; IndiceCase <= 80; IndiceCase++)
                { _Valeurs[valeur].Add(IndiceCase); }
            }
        }
        public Grille(List<int> Liste_Valeurs) // Constructeur de grille partielle. Prends en paramètre une liste d'entiers.
                                               // Si l'entier est entre 1 et 9 : c'est la valeur de cette case.
                                               // Sinon, la case a une valeur inconnue.
                                               // Si la case a une valeur inconnue, toutes les valeurs y sont possibles.
        {
            _Valeurs = new List<int>[10];
            for (int v = 0; v <= 9; v++) // Initialisation de chaque liste (sinon elle est de type "null" et on ne peut rien y ajouter)
            { _Valeurs[v] = new List<int>(); }
            for (int IndiceCase = 0; IndiceCase <= 80; IndiceCase++)
            {
                if (Liste_Valeurs[IndiceCase] > 9 || Liste_Valeurs[IndiceCase] < 1) // Case de valeur inconnue.
                {
                    for (int v = 1; v <= 9; v++)
                    { _Valeurs[v].Add(IndiceCase); } // Toutes les valeurs de 1 à 9 sont possibles, mais la case n'est pas listée comme connue.
                }
                else
                {
                    _Valeurs[0].Add(IndiceCase); // La case est listée comme connue
                    _Valeurs[Liste_Valeurs[IndiceCase]].Add(IndiceCase); // La liste des cases ayant cette valeur est mise à jour en y ajoutant l'indice de la case.
                }
            }
        }
        public static (Grille, Grille) Hypothese(Grille UneGrille) // Fais une hypothèse sur une grille non résolue.
                                                            // Prend la grille passée en paramètre, renvoie deux grilles-hypothèse complémentaires.
                                                            // Par exemple la case 5 de la grille en paramètre est inconnue.
                                                            // Renvoie une grille où cette case vaut 1, et une grille où cette case vaut tout sauf 1.
                                                            // On s'y prend ainsi : on commence par recopier la grille de départ à l'identique.
                                                            // Ensuite on prendra une hypothèse sur la plus petite case non résolue.
        {
            List<int>[] _ValeursA = new List<int>[10];
            List<int>[] _ValeursB = new List<int>[10];
            for (int i = 0; i <= 9; i++)
            {
                _ValeursA[i] = new List<int>();
                _ValeursB[i] = new List<int>();
                foreach (int UneCase in UneGrille._Valeurs[i])
                {
                    _ValeursA[i].Add(UneCase);
                    _ValeursB[i].Add(UneCase);
                }
            }
            Grille GrilleA = new Grille();
            Grille GrilleB = new Grille();
            GrilleA._Valeurs = _ValeursA;
            GrilleB._Valeurs = _ValeursB;
            // Maintenant on cherche la première case non résolue
            int Indice = 0;
            while (UneGrille._Valeurs[0].Contains(Indice))
            { Indice++; }
            // Cette case est résolue dans A, avec sa plus petite valeur possible
            int valeurA = 1;
            while (!UneGrille._Valeurs[valeurA].Contains(Indice))
            { valeurA++; }
            // la valeur valeurA est possible pour la case Indice. La case Indice prend la valeurA dans la grilleA.
            for (int v = 1; v <= 9; v++)
            {
                if (v != valeurA)
                {
                    GrilleA._Valeurs[v].Remove(Indice);
                }
            }
            // la valeur valeurA est interdite dans la case Indice pour la grille B.
            GrilleB._Valeurs[valeurA].Remove(Indice);
            return (GrilleA, GrilleB);
        }

        public int Valeur(int IndiceCetteCase) // Prend en paramètre l'indice d'une case, renvoie la valeur de cette case si la case est connue.
                                               // Jette une exception si la valeur de la case est inconnue : on est censés n'appeler "Valeur" que sur des cases connues

        {
            int val = 0;
            if (_Valeurs[0].Contains(IndiceCetteCase))
            {
                for (int v = 1; v <= 9; v++)
                {
                    if (_Valeurs[v].Contains(IndiceCetteCase))
                    { val = v; }
                }
            }
            return val;
        }
        public bool EstPossible() // Indique si la grille est possible.
                                  // Une grille est possible si TOUTES les cases sont possibles.
                                  // Une case est possible si AU MOINS une valeur est possible.
        {
            bool grille_possible = true;
            for (int indice = 0; indice <=80; indice++)
            {
                bool case_possible = false;
                for (int val = 1;val <= 9; val++)
                {
                    case_possible = case_possible || _Valeurs[val].Contains(indice);
                }
                grille_possible = grille_possible && case_possible;
            }
            return grille_possible;
        }
        public bool EstResolue()    // Indique si cette grille est résolue. Une grille est résolue si toutes ses cases sont résolues.
                                    // Quand toutes les cases sont résolues, la liste des cases résolues comporte toutes les 81 cases.
        {
            return _Valeurs[0].Count == 81;
        }
        public List<int> Ligne(int IndiceCetteCase) // Prend en paramètre l'indice d'une case.
                                                    // Renvoie la liste des indices des cases sur la même ligne,
                                                    // Sauf la case donnée au départ.
        {
            List<int> ListeLigne = new List<int>();
            int IndiceLigne = IndiceCetteCase / 9;
            for (int indice = IndiceLigne * 9; indice <= IndiceLigne * 9 + 8; indice++) // Parcours de la ligne de cette case
            {
                if (indice != IndiceCetteCase)
                {
                    ListeLigne.Add(indice);
                }
            }
            return ListeLigne;
        }
        public List<int> Colonne(int IndiceCetteCase) // Prend en paramètre l'indice d'une case. 
                                                      // Renvoie la liste des indices des cases sur la même colonne,
                                                      // Sauf la case donnée au départ
        {
            List<int> ListeColonne = new List<int>();
            int IndiceColonne = IndiceCetteCase % 9; // Numéro de la colonne, de 0 à 8.
            for (int indice = 0; indice <= 8; indice++)
            {
                if (IndiceColonne + (indice * 9) != IndiceCetteCase)
                {
                    ListeColonne.Add(IndiceColonne + (indice * 9));
                }
            }
            return ListeColonne;
        }
        public List<int> Bloc(int IndiceCetteCase) // Prend en paramètre l'indice d'une case.
                                                   // Renvoie la liste des indices des cases dans le même bloc,
                                                   // Sauf la case donnée au départ
        {
            List<int> ListeBloc = new List<int>();
            static int BlocIndice(int i)   // On définit une fonction qui renvoie l'indice du bloc (de 0 à 8) en fonction de l'indice de la case.
            { return (3 * (i / 27)) + ((i / 3) % 3); }
            for (int indice = 0; indice <= 80; indice++)   // On parcours toute la grille pour trouver les cases ayant le même indice de bloc que la case de départ.
            {
                if (BlocIndice(indice) == BlocIndice(IndiceCetteCase) && indice != IndiceCetteCase)
                {
                    ListeBloc.Add(indice);
                }
            }
            return ListeBloc;
        }

        public List<int> Voisinage(int IndiceCetteCase) // Le voisinage, c'est la Ligne, la Colonne, ET le Bloc.
        {
            List<int> Voisins = new List<int>();
            Voisins.AddRange(Ligne(IndiceCetteCase));
            Voisins.AddRange(Colonne(IndiceCetteCase));
            Voisins.AddRange(Bloc(IndiceCetteCase));
            return Voisins;
        }

        public List<int> Liste_Valeurs() // Permet d'obtenir la liste des valeurs connues. Les valeurs inconnues sont remplacées par 0.
        {
            List<int> ListeValeurs = new List<int>();
            for (int indice = 0; indice <= 80; indice++)
            {
                int valeur_de_case = 0;
                if (_Valeurs[0].Contains(indice))
                {
                    for (int v = 1; v <= 9; v++)
                    {
                        if (_Valeurs[v].Contains(indice))
                        { valeur_de_case = v; }
                    }
                }
                ListeValeurs.Add(valeur_de_case);
            }
            return ListeValeurs;
        }
        // Règles du sudoku
        // 1 - Une valeur ne peut pas se trouver plus d'une fois sur une ligne, une colonne ou un bloc.
        // 2 - Une valeur doit se trouver une fois sur une ligne, une colonne ou un bloc.

        // Fonction d'application des deux règles :
        // - Une case connue rend impossible sa valeur sur sa ligne, sa colonne et son bloc.
        // - Une case inconnue n'ayant qu'une valeur possible prend cette valeur.
        // Paramètre : aucun (s'applique sur une grille, c'est une méthode)
        // Effet : Prend chaque case connue, applique ses contraintes au voisinage.
        // Renvoie : un booléen "possible"
        // "possible" indique si la grille est pour l'instant possible à résoudre (pas de contradictions détectées), 
        // On regroupe les deux règles. Algorithme :
        // On parcourt toutes les cases :
        // - Si la case est connue elle empêche sa valeur d'être présente dans le voisinage,
        // - Si la case est inconnue, elle devient connue si une valeur est impossible dans tout son voisinage.
        // - On a "possible" tant qu'au moins une valeur est possible pour chaque case
        // On continue la boucle tant qu'on a du "progres", c'est à dire qu'on a réussi à supprimer des valeurs possibles.
        
        public bool Regles()
        {
            bool possible = true;
            bool progres;
            // Fonction d'ajout de case résolue.
            // Permet d'ajouter une case à la liste des cases résolues.
            // Vérifie que la case n'est pas déjà dans la liste.
            // Renvoie "true" si l'ajout a été effectif.
            // Renvoie "false" si la case était déjà présente.
            bool ajout_resolue(int IndiceCase)
            {
                bool deja_resolue = _Valeurs[0].Contains(IndiceCase);
                if (deja_resolue)
                {
                    return false;
                }
                else
                {
                    _Valeurs[0].Add(IndiceCase);
                    return true;
                }
            }
            do
            {
                progres = false;
                for (int CetteCase = 0; CetteCase <= 80; CetteCase++) // On parcourt les cases connues.
                {
                    if (_Valeurs[0].Contains(CetteCase)) // CETTE CASE EST CONNUE
                    {
                        // Pour chaque case connue : on prend sa valeur.
                        int ValeurCetteCase = Valeur(CetteCase);
                        // On interdit cette valeur dans le voisinage : Colonne, ligne et bloc.

                        if (ValeurCetteCase != 0)
                        {
                            foreach (int AutreCase in Voisinage(CetteCase))
                            {
                                progres = _Valeurs[ValeurCetteCase].Remove(AutreCase) || progres;
                            }
                        }
                    }
                    else // CETTE CASE EST INCONNUE
                    {
                        // CetteCase = Case inconnue. On parcourt les valeurs.
                        for (int val = 1; val <= 9; val++)
                        {
                            int compte_valeur_impossible = 0;
                            foreach (int AutreCase in Ligne(CetteCase)) // On parcourt la ligne
                            {
                                if (!_Valeurs[val].Contains(AutreCase)) // AutreCase ne peut pas prendre la valeur "val"
                                {
                                    compte_valeur_impossible++;
                                }
                            }
                            if (compte_valeur_impossible == 8) // La valeur "val" est impossible dans 8 cases de la ligne de CetteCase.
                                                               // Donc la case CetteCase prend la valeur "val".
                            {
                                for (int v = 1; v <= 9; v++)
                               {
                                    if (v != val)
                                    {
                                        _Valeurs[v].Remove(CetteCase);
                                    }
                                }
                                progres = ajout_resolue(CetteCase) || progres; // CetteCase est maintenant connue.
                            }
                            compte_valeur_impossible = 0;
                            foreach (int AutreCase in Colonne(CetteCase)) // On parcourt la colonne
                            {
                                if (!_Valeurs[val].Contains(AutreCase)) // AutreCase ne peut pas prendre la valeur "val"
                                {
                                    compte_valeur_impossible++;
                                }
                            }
                            if (compte_valeur_impossible == 8) // La valeur "val" est impossible dans 8 cases de la Colonne de CetteCase.
                                                               // Donc la case CetteCase prend la valeur "val".
                            {
                                for (int v = 1; v <= 9; v++)
                                {
                                    if (v != val)
                                    {
                                        _Valeurs[v].Remove(CetteCase);
                                    }
                                }
                                progres = ajout_resolue(CetteCase) || progres; // CetteCase est maintenant connue.
                            }
                            compte_valeur_impossible = 0;
                            foreach (int AutreCase in Bloc(CetteCase)) // On parcourt le Bloc
                            {
                                if (!_Valeurs[val].Contains(AutreCase)) // AutreCase ne peut pas prendre la valeur "val"
                                {
                                    compte_valeur_impossible++;
                                }
                            }
                            if (compte_valeur_impossible == 8) // La valeur "val" est impossible dans 8 cases du Bloc de CetteCase.
                                                               // Donc la case CetteCase prend la valeur "val".
                            {
                                for (int v = 1; v <= 9; v++)
                                {
                                    if (v != val)
                                    {
                                         _Valeurs[v].Remove(CetteCase);
                                    }
                                }
                                progres = ajout_resolue(CetteCase) || progres;  // CetteCase est maintenant connue.
                            }
                        }
                    }
                    // Après avoir appliqué les nouvelles contraintes, on passe les cases en "résolues" si on peut les résoudre.
                    // Case résolue : case pour laquelle il n'y a plus qu'une seule valeur qui est possible.
                    int compte_val_CetteCase = 0; // Nombre de valeurs possibles pour CetteCase
                    for (int val = 1; val <= 9; val++)
                    {
                        if (_Valeurs[val].Contains(CetteCase)) // CetteCase peut prendre la valeur "val"
                        {
                            compte_val_CetteCase++;
                        }
                    }
                    if (compte_val_CetteCase == 0) // Grille impossible : CetteCase ne peut prendre aucune valeur
                    { 
                        possible = false;
                        progres = false;
                        break;
                    }
                    if (compte_val_CetteCase == 1) // La case CetteCase fait son entrée dans la liste des cases résolues
                    {
                        progres = ajout_resolue(CetteCase) || progres;
                    }
                }
            } while (progres);
            return possible;
        }

    }
}
