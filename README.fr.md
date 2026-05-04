# Voice Agent RAG

Voice Agent RAG est un agent vocal intelligent pour l’automatisation du service client.

Le système écoute un utilisateur, convertit la voix en texte, comprend l’intention, recherche les informations pertinentes dans une base de connaissance d’entreprise via RAG, puis génère une réponse naturelle qui peut être reconvertie en audio.

## Objectifs

- Entrée vocale avec Speech-to-Text
- Détection d’intention avec un LLM
- Recherche documentaire avec RAG
- Génération de réponse en langage naturel
- Sortie vocale avec Text-to-Speech
- Historique des conversations
- Transfert vers un humain pour les cas complexes

## Architecture

```text
Entrée audio
  → Speech-to-Text
  → Détection d’intention
  → RAG / Base de connaissance
  → Génération de réponse
  → Text-to-Speech
  → Sortie audio
Stack technique
.NET 8
ASP.NET Core Web API
PostgreSQL
pgvector
LLM local ou cloud
Speech-to-Text
Text-to-Speech
Structure du projet
src/
  VoiceAgentRag.Api
  VoiceAgentRag.Application
  VoiceAgentRag.Contracts
  VoiceAgentRag.Domain
  VoiceAgentRag.Infrastructure
Périmètre MVP

La première version se concentre sur le cœur du pipeline IA, sans intégration téléphonique réelle.

Entrée texte ou audio
→ transcription
→ recherche RAG
→ réponse IA
→ sortie vocale optionnelle

L’intégration téléphonique pourra être ajoutée plus tard via Twilio, Asterisk, SIP ou un autre fournisseur.

Autor
Rachid BARIZ