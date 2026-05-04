# Voice Agent RAG

Voice Agent RAG is an AI-powered customer service voice agent.

The system listens to a user, converts speech to text, understands the intent, retrieves relevant information from a company knowledge base using RAG, and generates a natural language answer that can be converted back to speech.

## Goals

- Speech-to-text input
- Intent detection using an LLM
- Retrieval-Augmented Generation over company documents
- Natural language response generation
- Text-to-speech output
- Conversation history
- Human handoff for complex cases

## Architecture

```text
Audio Input
  → Speech-to-Text
  → Intent Detection
  → RAG / Knowledge Base
  → Response Generation
  → Text-to-Speech
  → Audio Output
Tech Stack
.NET 8
ASP.NET Core Web API
PostgreSQL
pgvector
Local or cloud LLM
Speech-to-Text
Text-to-Speech
Project Structure
src/
  VoiceAgentRag.Api
  VoiceAgentRag.Application
  VoiceAgentRag.Contracts
  VoiceAgentRag.Domain
  VoiceAgentRag.Infrastructure
MVP Scope

The first version focuses on the core AI pipeline without real phone integration.

Text or audio input
→ transcription
→ RAG search
→ AI response
→ optional voice output

Phone integration can be added later through Twilio, Asterisk, SIP, or another provider.


Autor
Rachid BARIZ