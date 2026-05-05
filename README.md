# Voice Agent RAG

Voice Agent RAG is an AI-powered customer service voice agent.

The system listens to a user, converts speech to text, understands the intent, retrieves relevant information from a company knowledge base using RAG, and generates a natural language answer that can be converted back to speech.

---

## Demo

### UI Demo (Blazor)

A lightweight demo UI is available to simulate a real customer service interaction.

Features:
- Multilingual (FR / EN / AR)
- Conversation state management
- Hybrid RAG (vector + keyword + reranking)
- Local LLM via Ollama
- Real-time responses

### Screenshot

![Demo Screenshot1](docs/demo1.png)
![Demo Screenshot2 FR](docs/demo2-fr.png)
![Demo Screenshot2 AR](docs/demo2-ar.png)
![Demo Screenshot2 EN](docs/demo2-en.png)

---

## Goals

- Speech-to-text input
- Intent detection
- Retrieval-Augmented Generation (RAG)
- Natural language response generation
- Text-to-speech output
- Conversation history
- Human handoff for complex cases

---

## Architecture


Audio Input
→ Speech-to-Text
→ Intent Detection
→ RAG / Knowledge Base
→ Response Generation (LLM)
→ Text-to-Speech
→ Audio Output


---

## Tech Stack

- .NET 9 / ASP.NET Core Web API
- PostgreSQL + pgvector
- :contentReference[oaicite:0]{index=0} (local LLM)
- Blazor (Demo UI)
- STT / TTS (mocked for MVP)

---

## Project Structure


src/
VoiceAgentRag.Api
VoiceAgentRag.Application
VoiceAgentRag.Contracts
VoiceAgentRag.Domain
VoiceAgentRag.Infrastructure
VoiceAgentRag.Demo


---

## Architecture Approach

This project follows an **architecture-first approach**.


Architecture first
→ Pipeline validated
→ Then LLM integration


### Why this approach?

- Clean and testable architecture
- No tight coupling with AI providers
- Full validation of the RAG pipeline independently of the LLM
- Easy provider switching (Ollama, OpenAI, Azure…)

---

## Current State (MVP)

The system now supports:

- Document ingestion and chunking
- **Vector search using pgvector**
- **Hybrid retrieval (vector + keyword + reranking)**
- Multilingual conversations (FR / EN / AR)
- Intent detection (rule-based)
- **LLM response generation via Ollama**
- Voice pipeline (STT + TTS mocked)
- Conversation and voice interaction persistence
- Demo UI (Blazor)
- Clean API with ProblemDetails error handling

---

## RAG Pipeline

The system implements a production-ready RAG pipeline:


User query
→ Query embedding (Ollama)
→ Vector search (pgvector)
→ Keyword search fallback
→ Hybrid reranking
→ Context selection
→ LLM answer generation


This hybrid approach improves:

- semantic understanding
- robustness to phrasing variations
- precision of answers

---

## LLM Integration

The system uses :contentReference[oaicite:1]{index=1} as a local LLM provider.

Capabilities:
- Context-aware answers using RAG
- Multilingual responses
- Prompt-controlled behavior
- Local execution (no external dependency)

Switchable providers:


Fake → Ollama → OpenAI → Azure


---

## Demo UI

A Blazor demo application is included:


src/VoiceAgentRag.Demo


Features:
- Chat interface
- Language switching
- Conversation persistence
- Suggested prompts
- Runtime diagnostics (intent, handoff)

---

## MVP Scope

The first version focuses on the core AI pipeline without phone integration:


Text or audio input
→ transcription
→ RAG search
→ AI response
→ optional voice output


Phone integration can be added later (Twilio, SIP, Asterisk, etc.)

---

## Author

Rachid Bariz