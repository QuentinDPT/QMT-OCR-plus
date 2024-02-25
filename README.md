# QMT OCR+
> QMT OCR+ est un logiciel de démonstration pour un entretien d'embauche

## Attendu

| | | Attendu | Réalisation |
|-|-|:-|:-|
| `010` | 🔴 | Le logiciel doit être développé en C#, lisible et documenté | ✔️ |
| `020` | ⚪️ | Le logiciel devrait utiliser la technologie WPF/XAML et une architecture MVVM | 🟡 Partailly |
| `030` | ⚪️ | Le rendu visuel du logiciel devrait approcher de la charte graphique qmt | 🟡 Partailly |
| `040` | 🔴 | Le logiciel doit acquérir des images à partir de la webcam | ✔️ |
| `050` | 🔴 | Le logiciel devrait afficher les images brutes en live de manière fluide | ✔️ |
| `060` | ⚪️ | Le logiciel devrait appliquer un (ou plusieurs traitements) sur l’image brute sur demande utilisateur | ✔️ |
| `070` | ⚪️ | Le logiciel devrait afficher le résultat du (des) traitement(s) | ✔️ |

## Contexte

Lors de la réalisation, j'ai eu le droit a beaucoup de libertées en dehors des features demandées.

La réalisation s'est déroulée en 2 jours.

Je regrettes fortement de ne pas avoir pu plus tester mon code pendant ces deux jours. J'ai choisi délibérément de prioriser les features plutot que la qualité, chose que je ne fais pas en temps normal.

## Remarques

L'introduction d'un modèle MVVM impose un modèle de données. N'ayant pas identifié de modèle de données évident présent dans l'application, je n'ai pas pu mettre en place cette architechture.

Cependant, j'ai réaliser une structure qui permettra de faire évoluer le logiciel facilement.
