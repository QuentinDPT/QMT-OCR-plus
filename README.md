# QMT‑OCR‑plus

> **QMT‑OCR‑plus** est un logiciel de traitement d’images et de vision industrielle
permettant de créer, orchestrer et exécuter des séquences
de traitement d’images avancées pour mesurer, valider ou invalider
des pièces dans un contexte industriel.

![https://github.com/QuentinDPT/QMT-OCR-plus/blob/master/resources/main%20page%20-%20object%20identification.png](https://github.com/QuentinDPT/QMT-OCR-plus/blob/master/resources/main%20page%20-%20object%20identification.png)

---

## 🚀 Fonctionnalités clés

### 🧠 Traitement d’images industriel
- Permet de **traiter des images en temps réel** pour des applications
de contrôle industriel (qualité, mesure, validation, etc.).
- Conçu pour être intégré dans des chaînes d’inspection automatisée.

### 🧩 Scripting avancé
- **Scripting en LUA** : écrivez des scripts personnalisés pour définir des traitements,
des mesures et des décisions.  
- Scripts LUA totalement exposés aux fonctionnalités de traitement d’image
et à l’API interne.

### 📜 Séquenceur YAML
- Utilise des **fichiers de séquence YAML** pour orchestrer des scripts LUA
et définir des process de traitement d’images complets.
- Permet de séquencer des opérations, gérer des conditions,
des boucles et des branchements logiques.

### 🔗 Intégration de moteurs de vision
- Accès direct dans LUA ou YAML aux fonctionnalités natives de :
  - **OpenCV**
  - **Halcon**
  - **Cognex**
- Combine les forces de plusieurs bibliothèques de vision pour des pipelines puissants.

### ⏱️ Exécution en temps réel
- Conçu pour des environnements où la performance est critique :
analyse d’images et exécution de scripts sans latence perceptible (soft real‑time).

---

## 🧠 Concepts de base

### 📌 Scripting LUA
Les scripts LUA sont le **cœur logique** de votre automatisation. Ils permettent :
- Lecture et pré‑traitement d’images
- Extraction de caractéristiques
- Calculs, décisions logiques
- Appels aux modules de traitement natifs

📝 Exemple simple :
```lua
require "logger"
require "geometry"

local points

function init()
  logger.logDebug("Initialisation des données")
  points = {
	  point.new(-8, 3),
	  point.new(-5, 7),
	  point.new(-3, 10),
	  point.new(-1, 14),
	  point.new(0, 15),
	  point.new(2, 19),
	  point.new(4, 22),
	  point.new(6, 27),
	  point.new(8, 30),
	  point.new(10, 34),
	  point.new(-6, 6),
	  point.new(1, 16),
	  point.new(3, 21),
	  point.new(5, 24),
	  point.new(7, 28),
  }
end

-- Boucle de traitement
function execute()
  logger.logDebug("Execution du script")
  
  local l = linear_regression(points)
  
  logger.logInfo("Angle detecte : " .. l:angle_deg())
  
  logger.logInfo(
    "Ligne (" ..
    l.start_point.x .. ", " ..
    l.start_point.y .. ") -> (" ..
    l.end_point.x .. ", " ..
    l.end_point.y .. ")")
end
```

Vous remarquerez que le script présente fonctions :
 - `init()`, qui est optionnel.
 Elle est executée avant le démarrage de la séquence, en dehors du cycle temps réel.
 - `execute()`, qui est **obligatoire**.
 Elle est executée au sein de la boucle temps réel.

---

### 🔁 Séquenceur YAML

Le séquenceur YAML vous permet de définir une **chaîne d’opérations** exécutées
dans un ordre prédéfini, avec possibilité d’appeler des scripts LUA.

**Exemple de séquence YAML**
```YAML
sequence:
  - name: Capture image
    lua_script: capture.lua

  - name: Pré‑traitement
    lua_script: preprocess.lua

  - name: Analyse
    lua_script: analyze.lua

  - name: Décision
    lua_script: decide.lua
```

---

## 🛠️ Installation

Cloner ce dépôt :

```bash
git clone https://github.com/QuentinDPT/QMT-OCR-plus.git
cd QMT-OCR-plus
```

**Pré‑requis** :

 - .NET Runtime (si l’app est en C# / WPF)
 - Modules de vision (OpenCV, Halcon, Cognex SDKs)
 - Interpréteur LUA compatible

---

## 🏗️ Organisation du code

Le projet est structuré comme suit :
```
├── QMTGroup.Web/            # Interface utilisateur
├── QMTGroup.Camera.*/       # Caméras disponible
|                            # (EmguCV, Halcon, Cognex, File, Folder)
├── QMTGroup.Models/         # Modèles de données
├── QMTGroup.ImageFilters/   # Filtres et fonctions de traitement
├── QMTGroup.DSL/            # Langage spécifique métier (LUA)
├── QMTGroup.DSL.Library.*/  # Librairies qui ajoute les fonctionalités
├── QMTGroup.Overlay.*/      # Formats d'overlay affichables (SVG, DXF)
└── QMTGroup.Image.*/        # Format d'images pour l'interopérabilité et compression
                             # (FFMPEG, EmguCV, Halcon, Cognex)
```
