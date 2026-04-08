# Agentic-First with GitHub Copilot
### Global Azure Columbus — April 18, 2026

> *Most teams use GitHub Copilot like a smarter autocomplete. The faster teams have reorganized how work gets done around agents that plan, code, deploy, and ship.*

This repository contains the demo environment and supporting assets for the talk **"Agentic-First with GitHub Copilot: Teams That Work With AI, Not Just Next to It"** presented at [Global Azure Columbus 2026](https://globalazure.net).

---

## What This Talk Covers

- How **instructions, prompts, agents, skills, and MCP tools** compose together in `.github/`
- **Live demo**: using Microsoft's Azure Skills and `@azure` to deploy and operate Azure resources from Copilot
- **Shared assets** — `copilot-instructions.md`, agent definitions, PR templates — that raise the floor for every developer
- **Enterprise governance**: model policies, audit logs, and the agent control plane
- How to move your team from solo wins to **agentic-first workflows**

---

## Repository Structure

```
.github/
  copilot-instructions.md       # Repo-wide Copilot context
  agents/                       # Custom agent definitions (.agent.md)
  instructions/                 # Path-specific coding standards
  prompts/                      # Reusable prompt files (.prompt.md)
  plugins/
    azure-skills/               # Azure Skills Plugin (skills + hooks)
.devcontainer/
  devcontainer.json             # Dev container configuration
  postCreate.sh                 # Automated environment setup
.mcp.json                       # Azure + Context7 MCP server config
```

---

## Demo Environment

The dev container gives you a fully configured environment with everything needed to run the demos.

### What's Included

| Tool | Purpose |
|------|---------|
| .NET 10 | Demo application runtime |
| Azure CLI (`az`) | Azure resource management |
| Azure Developer CLI (`azd`) | Deploy workflows via `azure-prepare` → `azure-validate` → `azure-deploy` |
| Node.js 22 | Required for MCP servers (`npx`) |
| GitHub CLI (`gh`) | Coding agent workflows, repo operations |
| Azure MCP Server | 200+ tools across 40+ Azure services |
| Context7 MCP | Up-to-date library docs for the agent |
| Azure Skills Plugin | 20 curated Azure skills in `.github/plugins/azure-skills/` |

### VS Code Extensions

- GitHub Copilot + Copilot Chat
- C# Dev Kit
- Azure MCP Server (wires up Azure Skills automatically)
- Azure Developer CLI (`azd`)
- Azure Resource Groups
- GitHub Actions (watch coding agent runs live)

### Getting Started

**Prerequisites:** Docker Desktop + VS Code with the [Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)

1. Clone this repo and open in VS Code
2. When prompted, click **Reopen in Container** (or `Cmd+Shift+P` → *Dev Containers: Reopen in Container*)
3. Wait for the container to build and `postCreate.sh` to complete
4. Sign in to Azure:

```bash
az login
azd auth login
```

Azure Skills are active in VS Code immediately. To also use them in Copilot CLI:

```
/plugin marketplace add microsoft/azure-skills
/plugin install azure@azure-skills
```

---

## Key Concepts

### The Customization Hierarchy

```
Instructions → Skills → Agents → Prompts
```

| Type | When Active | Best For |
|------|-------------|----------|
| `*.instructions.md` | Always-on | Coding standards, guardrails |
| `SKILL.md` | On-demand | Portable multi-step capabilities |
| `*.agent.md` | When selected | Role-based workflows, tool restrictions |
| `*.prompt.md` | On-demand | Simple recurring task automation |

### Azure Skills Plugin — Three Layers

| Layer | What It Does |
|-------|-------------|
| **Skills (brain)** | 20 curated Azure workflows: prepare, validate, deploy, diagnose, optimize... |
| **Azure MCP Server (hands)** | 200+ tools across 40+ services — real operations against live Azure |
| **Context7 MCP** | Up-to-date SDK and library documentation for the agent |

---

## Resources

- [Azure Skills Plugin](https://aka.ms/azure-plugin) — `aka.ms/azure-plugin`
- [microsoft/azure-skills](https://github.com/microsoft/azure-skills) — GitHub repo
- [Copilot Cloud Agent docs](https://docs.github.com/en/copilot/using-github-copilot/coding-agent/about-assigning-tasks-to-copilot)
- [Repository Custom Instructions](https://docs.github.com/en/copilot/customizing-copilot/adding-repository-custom-instructions-for-github-copilot)
- [AI Coding Workshop](https://github.com/centricconsulting/ai-coding-workshop) — hands-on workshop with full customization hierarchy examples

---

## Speaker

**Shawn Wallace** — [GitHub](https://github.com/shawnewallace) · [Twitter/X](https://x.com/ShawnWallace)

*Centric Consulting*
