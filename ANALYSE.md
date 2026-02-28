# Analyse Complète du Mod KindredExtract v1.8.2 pour V Rising

## Table des Matières

1. [Vue d'Ensemble](#1-vue-densemble)
2. [Architecture Technique](#2-architecture-technique)
3. [Point d'Entrée — Plugin.cs](#3-point-dentrée--plugincs)
4. [Noyau du Mod — Core.cs](#4-noyau-du-mod--corecs)
5. [Système de Commandes](#5-système-de-commandes)
6. [Système d'Extraction d'Entités](#6-système-dextraction-dentités)
7. [Services](#7-services)
8. [Patches Harmony](#8-patches-harmony)
9. [Extensions ECS](#9-extensions-ecs)
10. [Modèles de Données](#10-modèles-de-données)
11. [Convertisseurs de Commandes](#11-convertisseurs-de-commandes)
12. [Diagramme d'Architecture](#12-diagramme-darchitecture)
13. [Résumé des Commandes Disponibles](#13-résumé-des-commandes-disponibles)
14. [Dépendances](#14-dépendances)
15. [Conclusion](#15-conclusion)

---

## 1. Vue d'Ensemble

**KindredExtract** est un mod serveur pour V Rising développé par **odjit**. Son rôle principal est de **dump/extraire** des informations internes du jeu (entités ECS, prefabs, localisations, systèmes, états de joueurs, etc.) vers des fichiers texte ou JSON pour le débogage et l'analyse du serveur.

| Propriété | Valeur |
|---|---|
| Nom | KindredExtract |
| Version | 1.8.2 |
| Auteur | odjit |
| Framework | BepInEx (IL2CPP) |
| Patching | HarmonyLib |
| Commandes | VampireCommandFramework |
| Architecture du jeu | Unity ECS (Entity Component System) |
| Taille du DLL | 2.5 Mo |
| Fichiers décompilés | 38 fichiers C# |

---

## 2. Architecture Technique

Le mod s'organise en **7 namespaces** :

```
KindredExtract/                     # Noyau : Plugin, Core, ECSExtensions, EntityDebug, Helper
KindredExtract.Commands/            # Commandes chat : DumpCommands, EntityCommands, StateCommands
KindredExtract.Commands.Converters/ # Convertisseurs de paramètres de commande
KindredExtract.Data/                # Données statiques (Prefabs connus)
KindredExtract.Models/              # Modèles de données (Database, Player, Systems, etc.)
KindredExtract.Patches/             # Patches Harmony (hooks sur le jeu)
KindredExtract.Services/            # Services (Players, Prefabs, Localization, ECS Systems)
```

---

## 3. Point d'Entrée — Plugin.cs

```csharp
[BepInPlugin("KindredExtract", "KindredExtract", "1.8.2")]
[BepInDependency(VampireCommandFramework)]
public class Plugin : BasePlugin
```

### Séquence de chargement :

1. **`Load()`** — Appelé par BepInEx au démarrage :
   - Log du message de chargement
   - Initialisation de la `Database` (configuration BepInEx)
   - Création de l'instance **Harmony** et patch de l'assembly
   - Enregistrement de toutes les commandes via `CommandRegistry.RegisterAll()`

2. **`OnGameInitialized()`** — Appelé via le patch `InitializationPatch` :
   - Vérifie que le `PrefabCollectionSystem` est prêt (dictionnaire non vide)
   - Appelle `Core.InitializeAfterLoaded()`

3. **`Unload()`** — Désinscrit les commandes et retire les patches Harmony

---

## 4. Noyau du Mod — Core.cs

`Core` est la **classe statique centrale** qui contient toutes les références aux systèmes du jeu :

### Propriétés principales :

| Propriété | Type | Description |
|---|---|---|
| `TheWorld` | `World` | Le monde Unity ECS (cherche "Server" puis "Client_0") |
| `IsServer` | `bool` | `true` si on est sur le monde serveur |
| `EntityManager` | `EntityManager` | Gestionnaire d'entités ECS |
| `ServerScriptMapper` | `ServerScriptMapper` | Mapper de scripts serveur |
| `ServerTime` | `double` | Temps serveur actuel |
| `Players` | `PlayerService` | Service de gestion des joueurs |
| `Prefabs` | `PrefabService` | Service des prefabs |
| `Localization` | `LocalizationService` | Service de localisation |
| `EcsSystemHierarchyService` | Service | Hiérarchie des systèmes ECS |
| `EcsSystemDumpService` | Service | Dump des systèmes ECS |

### Méthodes clés :

- **`InitializeAfterLoaded()`** : Initialise les services après chargement complet du jeu (PlayerService, PrefabService, ComponentInitializer)
- **`CountPrefabs(maxNum, filter)`** : Parcourt TOUTES les entités du monde, compte les PrefabGUIDs, retourne les N plus fréquents
- **`SavePrefabCountToCSV()`** : Exporte les comptages en CSV (`prefab_count.csv` et `non_prefab_count.csv`). Note : détruit les entités orphelines "Simulate"
- **`StartCoroutine()`** : Crée un GameObject persistant (`DontDestroyOnLoad`) pour exécuter des coroutines Unity

---

## 5. Système de Commandes

Le mod expose **3 groupes de commandes** dans le chat du jeu :

### 5.1 DumpCommands (`/dump` ou `.dump`)

Commandes d'extraction de données massives vers fichiers :

| Commande | Alias | Description |
|---|---|---|
| `.dump prefabs` | `.dump p` | Dump tous les prefabs en fichier C# (`prefabs.txt`) |
| `.dump types` | `.dump t` | Dump les types de composants ECS (`componentTypes.txt`) |
| `.dump entityqueries` | `.dump eq` | Dump toutes les EntityQueries des systèmes |
| `.dump prefabjsons` | `.dump pj` | Dump les prefabs en fichiers JSON groupés par préfixe |
| `.dump guidpos` | — | Export CSV des positions de toutes les entités d'un PrefabGUID donné |
| `.dump localization` | — | Sauvegarde les données de localisation |
| `.dump prefabnames` | — | Dump les noms localisés des prefabs (cherche dans ManagedCharacterHUD, ManagedItemData, ManagedUnitBloodTypeData, ManagedMissionData, ManagedTechData, ManagedPerkData, ManagedBlueprintData, ManagedAbilityGroupData, ManagedDataDropGroup, ManagedBuildMenuTagData, ManagedBuildMenuGroupData, ManagedBuildMenuCategoryData, ManagedSpellSchoolData) |
| `.dump systems` | `.dump s` | Dump la hiérarchie des systèmes ECS |
| `.dump archetypes` | `.dump a` | Dump tous les archétypes ECS avec le nombre d'entités |

### 5.2 EntityCommands (`/entity` ou `.entity` / `.e`)

Commandes de manipulation directe d'entités :

| Commande | Alias | Description |
|---|---|---|
| `.entity teleport <id> [version]` | `.e tp` | Téléporte le joueur à l'entité spécifiée (crée un `PlayerTeleportDebugEvent`) |
| `.entity despawn <id> [version]` | `.e d` | Tue l'entité via `StatChangeUtility.KillEntity()` |
| `.entity destroy <id> [version]` | `.e del` | Détruit l'entité en ajoutant `DestroyTag` + `DestroyData` |
| `.entity topcount [n] [filter]` | `.e tc` | Affiche les N entités les plus fréquentes (optionnellement filtrées) |

### 5.3 StateCommands (`/state` ou `.state` / `.s`)

Le groupe le plus complet — extraction d'état d'entités :

| Commande | Alias | Description |
|---|---|---|
| `.state switchdump` | — | Bascule entre le dump Kindred (custom) et le dump ProjectM (natif) |
| `.state clan <name>` | `.s c` | Dump l'état complet d'un clan |
| `.state player [joueur]` | `.s p` | Dump l'état complet d'un joueur (User + Character + Team + Progression) |
| `.state slots [joueur]` | `.s s` | Dump tous les slots d'abilités d'un joueur |
| `.state inventory [joueur]` | `.s i` | Dump l'inventaire complet |
| `.state door [joueur]` | `.s d` | Dump les portes possédées par un joueur |
| `.state ownedby [joueur]` | `.s o` | Dump toutes les entités possédées par un joueur |
| `.state entity <id> [version]` | `.s e` | Dump l'état d'une entité spécifique |
| `.state prefab [id]` | — | Dump un ou tous les prefabs |
| `.state teams` | `.s t` | Dump toutes les données d'équipes |
| `.state nearby [rayon]` | `.s n` | Dump les entités dans un rayon autour du joueur |
| `.state tilemodels [rayon]` | `.s tm` | Dump les TileModels proches |
| `.state rooms [rayon]` | `.s r` | Dump les rooms (pièces de château) proches |
| `.state SetPasteBinKeysNoLog` | — | Configure des clés PasteBin pour upload automatique |
| `.state vblood <nom>` | — | Dump l'état d'un VBlood spécifique |
| `.state allvbloods` | — | Dump tous les VBloods |
| `.state buffs` | — | Dump tous les buffs actifs du joueur |
| `.state itemdata` | — | Dump les données de tous les items |

### Système de nommage intelligent des fichiers

La méthode `OutputEntityState()` (~450 lignes) génère automatiquement des noms de fichiers descriptifs selon le type d'entité :
- `User_NomDuJoueur_123_1.txt`
- `Player_NomDuPersonnage_456_1.txt`
- `Castle_789_1.txt`
- `Inventory_NomDuJoueur_ItemPrefab_012_1.txt`
- `Buff_345_1_(NomDuBuff on Player NomDuJoueur).txt`
- `Clan_NomDuClan_678_1.txt`
- `CastleTeam_NomDuPropriétaire_901_1.txt`
- etc.

Supporte aussi un système **"_Prev"** pour garder l'état précédent d'un fichier.

### Intégration PasteBin

Les résultats peuvent être uploadés automatiquement sur PasteBin via l'API (avec clé API, user key, et dossier optionnel). Les pastes expirent après 1 heure.

---

## 6. Système d'Extraction d'Entités

### EntityDebug.cs (~900 lignes)

Le coeur du système de dump d'entités. Fonctionnement :

1. **`RegisterExtractor<T>()`** — Enregistre un extracteur pour un type de composant ECS. Trois stratégies :
   - Composants "zero-sized" (tags) : juste le nom
   - Composants avec des champs : utilise la réflexion pour lire tous les champs
   - Composants buffer : lit les éléments du DynamicBuffer

2. **`RetrieveComponentData(entity)`** — Méthode principale de dump :
   - Itère sur tous les composants de l'entité
   - Pour chaque composant, appelle l'extracteur enregistré
   - Formate le résultat avec le nom du composant et ses données
   - Gère spécialement certains types (PrefabGUID → nom résolu, LocalizationKey → texte traduit, Entity → résolution récursive)

3. **`FieldToString()`** — Convertisseur universel de champs en texte :
   - Gère les types primitifs, enums, FixedString, Entity, PrefabGUID
   - Résolution récursive des sous-champs pour les structs
   - Traitement spécial des `NetworkedEntity`, `ModifiableEntity`, etc.

### ComponentInitializer.cs

Enregistre massivement les extracteurs pour **des centaines** de types de composants ECS V Rising :
- Composants ProjectM : `AbilityBar`, `AiMoveState`, `Blood`, `Buff`, `CastleHeart`, `Door`, `Equipment`, etc.
- Composants Unity : `Translation`, `Rotation`, `PhysicsCollider`, etc.
- Composants réseau : `User`, `PlayerCharacter`, `NetworkId`, etc.
- Composants Stunlock : `PrefabGUID`, `MapZoneData`, etc.

---

## 7. Services

### 7.1 PlayerService

Gère le cache de joueurs connectés/déconnectés :
- Maintient un dictionnaire `steamId → PlayerData` (nom, entité user, entité personnage, connecté/déconnecté)
- Méthodes `UserOnline()` / `UserOffline()` pour mettre à jour le statut
- `GetAllPlayers()` retourne la liste complète

### 7.2 PrefabService

Cache simplifié pour les PrefabGUIDs fréquemment utilisés :
- `AbilityGroupSlot` — pour le dump des slots

### 7.3 LocalizationService

Gère les traductions du jeu :
- Charge un fichier JSON intégré (`Localization.English.json`) comme fallback
- Méthode `GetLocalization(guid)` pour résoudre un AssetGuid en texte
- `SaveLocalization()` pour exporter le dictionnaire complet en JSON

### 7.4 EcsSystemDumpService

Dump les hiérarchies de systèmes ECS :
- Parcourt tous les `World` Unity (Server, Client, etc.)
- Pour chaque monde, dump l'arbre des systèmes avec `EcsSystemHierarchyService`
- Écrit dans un dossier `EcsSystemHierarchy/`

### 7.5 EcsSystemHierarchyService

Reconstruit l'arbre hiérarchique des systèmes ECS :
- Détecte les `ComponentSystemGroup` et leurs enfants
- Construit un arbre `EcsSystemTreeNode` avec comptage récursif
- Formate la sortie avec indentation

---

## 8. Patches Harmony

### 8.1 InitializationPatch

```csharp
[HarmonyPatch(typeof(GameBootstrap), "Start")]
```
- Se greffe sur `GameBootstrap.Start()` (démarrage du serveur de jeu)
- Appelle `Plugin.OnGameInitialized()` pour finaliser l'initialisation du mod

### 8.2 OnUserConnected_Patch

```csharp
[HarmonyPatch(typeof(ServerBootstrapSystem), "OnUserConnected")]
```
- Intercepte les connexions de joueurs
- Met à jour le `PlayerService` via `Core.Players.UserOnline()`

### 8.3 OnUserDisconnected_Patch

```csharp
[HarmonyPatch(typeof(ServerBootstrapSystem), "OnUserDisconnected")]
```
- Intercepte les déconnexions de joueurs
- Met à jour le `PlayerService` via `Core.Players.UserOffline()`

### 8.4 Destroy_TravelBuffSystem_Patch

```csharp
[HarmonyPatch(typeof(Destroy_TravelBuffSystem), "OnUpdate")]
```
- Se greffe sur le système de destruction des buffs de voyage
- Usage probablement lié au suivi des téléportations

---

## 9. Extensions ECS

### ECSExtensions.cs

Méthodes d'extension génériques pour simplifier la manipulation d'entités Unity ECS avec IL2CPP :

| Méthode | Description |
|---|---|
| `entity.Read<T>()` | Lit un composant d'une entité (marshaling mémoire brut) |
| `entity.Write<T>(data)` | Écrit un composant sur une entité |
| `entity.Has<T>()` | Vérifie si une entité possède un composant |
| `entity.Add<T>()` | Ajoute un composant à une entité |
| `entity.Remove<T>()` | Supprime un composant d'une entité |
| `prefabGuid.LookupName()` | Résout un PrefabGUID en nom lisible |
| `entity.GetComponentString()` | Liste tous les composants d'une entité en string |

Ces extensions utilisent du code **unsafe** et du marshaling de pointeurs pour contourner les limitations d'IL2CPP et accéder directement aux données ECS en mémoire.

---

## 10. Modèles de Données

### Database.cs
Configuration BepInEx du mod. Actuellement vide (`InitConfig()` ne configure rien) — infrastructure prête pour de futurs settings.

### Player.cs / PlayerData.cs
- `Player` : record contenant `CharacterName`, `PlatformId`, `IsOnline`, `UserEntity`, `CharEntity`
- `PlayerData` : classe simple avec `CharacterName`, `SteamID`, `IsOnline`, `UserEntity`, `CharEntity`

### BloodType.cs
Enum des types de sang dans V Rising : `Frailed`, `Creature`, `Warrior`, `Rogue`, `Brute`, `Scholar`, `Worker`, `Mutant`, `VBlood`, `GateBoss`, `Draculin`, `Immortal`, `None`

### EcsSystemCategory.cs
Enum des catégories de systèmes ECS : `Managed`, `Unmanaged`, `ManagedGroup`, `UnmanagedGroup`

### EcsSystemTreeNode.cs
Noeud d'arbre pour la hiérarchie de systèmes avec : `Name`, `Category`, `Children`, `Counts`

### KnownUnknowns.cs
Liste de composants ECS connus qui ne sont pas dans les assemblies chargées — évite les logs d'erreur lors de l'enregistrement des extracteurs.

### Prefabs.cs (Data)
Contient un seul prefab statique : `AbilityGroupSlot = new PrefabGUID(-1466452881)` utilisé pour le dump des slots d'abilités.

---

## 11. Convertisseurs de Commandes

Le mod définit des convertisseurs VampireCommandFramework pour parser les arguments de commande :

| Convertisseur | Description |
|---|---|
| `FoundPlayerConverter` | Cherche un joueur par nom partiel (online ou offline), retourne `FoundPlayer` |
| `OnlinePlayerConverter` | Cherche un joueur online uniquement, retourne `OnlinePlayer` |
| `FoundVBloodConverter` | Cherche un VBlood par nom partiel, retourne `FoundVBlood` |

Chaque convertisseur utilise `Core.Players` pour résoudre les noms partiels.

---

## 12. Diagramme d'Architecture

```
                         BepInEx IL2CPP
                              │
                         Plugin.Load()
                              │
                    ┌─────────┴─────────┐
                    │                   │
              Harmony.PatchAll()   CommandRegistry
                    │              .RegisterAll()
           ┌───────┼───────┐           │
           │       │       │     ┌─────┼──────────┐
    Initialize  UserConn  UserDisc    │           │
      Patch      Patch     Patch     │           │
           │       │       │    DumpCommands  StateCommands  EntityCommands
           │       │       │         │              │              │
           └───────┼───────┘         │              │              │
                   │                 └──────┬───────┘              │
            Core.Initialize()               │                      │
                   │              EntityDebug.RetrieveComponentData()
           ┌───────┼───────┐               │
           │       │       │        ComponentInitializer
      PlayerSvc PrefabSvc LocalSvc  (registre 200+ extracteurs)
           │       │       │               │
           └───────┼───────┘        ECSExtensions
                   │              (Read/Write/Has/Add/Remove)
                   │                       │
              Unity ECS World ◄────────────┘
         (EntityManager, Systems,
          Prefabs, Components)
```

---

## 13. Résumé des Commandes Disponibles

### Groupe `.dump` — Extraction de données globales
```
.dump prefabs          — Tous les prefabs → prefabs.txt
.dump types            — Types ECS → componentTypes.txt
.dump entityqueries    — Queries ECS → EntityQueryDescriptions.txt
.dump prefabjsons      — Prefabs → JSON groupés par catégorie
.dump guidpos <id>     — Positions d'un prefab → CSV
.dump localization     — Traductions → JSON
.dump prefabnames      — Noms localisés → PrefabNames.json
.dump systems          — Hiérarchie systèmes → dossier
.dump archetypes       — Archétypes ECS → archetypes.txt
```

### Groupe `.entity` — Manipulation d'entités
```
.entity teleport <id> [v]   — Téléportation vers une entité
.entity despawn <id> [v]    — Tuer une entité
.entity destroy <id> [v]    — Détruire une entité
.entity topcount [n] [f]    — Top N entités par fréquence
```

### Groupe `.state` — Extraction d'état détaillé
```
.state player [joueur]      — État complet d'un joueur
.state clan <nom>           — État d'un clan
.state inventory [joueur]   — Inventaire d'un joueur
.state slots [joueur]       — Slots d'abilités
.state entity <id> [v]      — État d'une entité quelconque
.state prefab [id]          — État d'un/tous les prefabs
.state teams                — Toutes les équipes
.state nearby [rayon]       — Entités proches
.state tilemodels [rayon]   — TileModels proches
.state rooms [rayon]        — Rooms proches
.state door [joueur]        — Portes possédées
.state ownedby [joueur]     — Entités possédées
.state vblood <nom>         — État d'un VBlood
.state allvbloods           — Tous les VBloods
.state buffs                — Buffs du joueur
.state itemdata             — Données de tous les items
.state switchdump           — Basculer mode de dump
```

---

## 14. Dépendances

| Dépendance | Version | Rôle |
|---|---|---|
| **BepInEx** (BepInExPack V Rising) | 1.691.3 | Framework de modding pour Unity IL2CPP |
| **VampireCommandFramework** | 0.10.4 | Framework de commandes chat pour V Rising |
| **HarmonyLib** | (inclus dans BepInEx) | Patching de méthodes à la volée |
| **Il2CppInterop** | (inclus dans BepInEx) | Interop entre .NET managé et IL2CPP |

---

## 15. Conclusion

**KindredExtract** est un outil de **débogage et d'inspection** très complet pour les serveurs V Rising. Il ne modifie pas la logique du jeu (à l'exception du despawn/destroy d'entités) mais fournit une visibilité totale sur l'état interne du monde ECS.

### Points forts :
- Couverture massive de composants ECS (200+ types enregistrés)
- Nommage intelligent des fichiers de sortie
- Système dual de dump (Kindred custom vs ProjectM natif)
- Intégration PasteBin pour partage facile
- Architecture propre et modulaire

### Points d'attention :
- Le mod est **admin-only** (toutes les commandes ont `adminOnly: true`)
- `SavePrefabCountToCSV()` détruit les entités orphelines "Simulate" — effet de bord potentiel
- Le code utilise du marshaling **unsafe** pour contourner IL2CPP — sensible aux mises à jour du jeu
- Le mod est conçu pour le **côté serveur** (cherche d'abord le World "Server")
