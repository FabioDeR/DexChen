# 🎴 DexChen

<p align="center">
  <img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet" alt="dotnet 9.0" />
  <img src="https://img.shields.io/badge/Blazor-WebAssembly-512BD4?style=for-the-badge&logo=blazor" alt="blazor webassembly" />
  <img src="https://img.shields.io/badge/Supabase-3ECF8E?style=for-the-badge&logo=supabase" alt="supabase" />
  <img src="https://img.shields.io/badge/PWA-Ready-5A0FC8?style=for-the-badge&logo=pwa" alt="PWA Ready" />
</p>

## 📝 Description

**DexChen** est une application web moderne pour les collectionneurs de cartes Pokémon qui permet de scanner, identifier et estimer la valeur de vos cartes en temps réel.

<p align="center">
  <b>Scanne. Estime. Collectionne.</b>
</p>

## ✨ Fonctionnalités

- 📸 **Scanner de cartes** - Prenez une photo de votre carte pour l'identifier automatiquement
- 🔍 **Identification précise** - Reconnaissance de la série, rareté et édition de vos cartes
- 💰 **Estimation du prix** - Valeurs marchandes actualisées (min, moyenne, max)
- 📊 **Suivi des prix** - Graphiques d'évolution de prix dans le temps
- 📚 **Gestion de collection** - Ajoutez, organisez et suivez toutes vos cartes
- 🔔 **Alertes de prix** - Notifications quand les prix atteignent vos seuils définis
- ❤️ **Favoris** - Marquez vos cartes préférées pour y accéder rapidement
- 🌐 **PWA** - Fonctionne hors ligne et peut être installée comme une application native

## 🛠️ Technologies

### Frontend
- **Blazor WebAssembly** - Framework SPA pour créer des applications web interactives avec C# et .NET
- **PWA** - Progressive Web App pour une expérience utilisateur améliorée
- **LocalStorage** - Gestion de la session et des données en local

### Backend
- **Supabase** - Plateforme BaaS (Backend-as-a-Service) pour:
  - **Base de données PostgreSQL** - Stockage des données
  - **Auth** - Authentification et gestion des utilisateurs
  - **Storage** - Stockage des images de cartes

### Architecture
- **Clean Architecture** - Structure claire avec séparation des responsabilités:
  - **Domain** - Entités et logique métier
  - **Application** - Cas d'utilisation et services
  - **UI** - Interface utilisateur Blazor

## 🚀 Démarrage rapide

### Prérequis
- .NET 9.0 SDK
- Compte Supabase (pour les clés API)

### Installation

1. Clonez le dépôt
```bash
git clone https://github.com/votre-username/DexChen.git
cd DexChen
```

2. Restaurez les packages
```bash
dotnet restore
```

3. Lancez l'application
```bash
cd DexChen.UI
dotnet run
```

4. Accédez à l'application dans votre navigateur: `https://localhost:5001`

## 📱 Utilisation

1. **Créez un compte** ou connectez-vous
2. **Scannez une carte** en utilisant l'appareil photo de votre appareil
3. **Consultez les détails** de la carte et son estimation de prix
4. **Ajoutez la carte** à votre collection
5. **Suivez l'évolution** des prix au fil du temps

## 🌍 Roadmap

- [ ] Support multilingue (EN, JP)
- [ ] Support pour d'autres jeux de cartes (Magic, Yu-Gi-Oh)
- [ ] API pour développeurs
- [ ] Applications mobiles natives (iOS/Android)
- [ ] Recommandations personnalisées
---

<p align="center">
  Fait avec ❤️ par l'équipe DexChen
</p>