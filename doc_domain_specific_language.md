# Documentation : Domain Specific Language (DSL)

## Introduction

### Definition

A Domain Specific Language (DSL) is a programming language or script specifically designed for a particular application domain. Unlike general-purpose languages, which are designed to address a wide range of software problems, DSLs are tailored to efficiently handle specific tasks within a particular field or industry.

### General purpose

Le DSL permet à l'utilisateur d'écrire du code vision pour l'executer au sein du logiciel.

## Our DSL

Notre "Domain Specific Language" (DSL) utilise la vision en tant que base "domain". C'est a dire que l'objectif principal de notre DSL est de répondre à des problématiques de vision (vision industrielle majoritairement).

Notre DSL présente plusieurs fonctionalitées :
 - Langage LUA,
 - Librairies
   - Logger,
   - Camera,
   - Technologies (Halcon, Cognex, Matlab, ...)
 - Librairies extensibles,
 - Ecoute d'evenements 

Notre DSL gère 3 types différents :
| Nom | Description | Exemples |
|---|---|---|
| `ValueObject` | Une valeure en mémoire, ne possède pas de fonctionalitées | `int`, `bool`, `double` |
| `Object` | c'est une valeure en mémoire, possède des fonctionalitées | `time` |
| `ComplexObject` | c'est un pointeur vers un objet, possède des fonctionalitées | `image`, `camera` |

**Code Samples**

> `ValueObject`
> 
> ```lua
> -- Déclaration
> valeur = 1
> 
> -- Utilisation
> valeur = valeur + 10
> print(valeur)
> ```

> `Object`
> ```lua
> require "time"
> 
> -- Déclaration
> object = time.now()
> 
> -- Utilisation
> print(object.year)
> ```

> `ComplexObject`
> ```lua
> require "camera"
> 
> -- Déclaration
> complexObject = camera.get("1234")
> 
> -- Utilisation
> image = complexObject.grab()
> ```
> `image` est un compex object. Vous n'avez pas accès directement aux données de l'image. Vous devez passer par des fonctionalitées de l'objet image.


