# Presentation Outline: Agentic-First with GitHub Copilot

**Talk:** Agentic-First with GitHub Copilot: Teams That Work With AI, Not Just Next to It  
**Event:** Global Azure Columbus — April 18, 2026  
**Duration:** 30 minutes (presentation + live demo)

---

## 1. Hook (2 min)

- "Show of hands: who uses Copilot for autocomplete?"
- "Now keep your hand up if your *whole team* is faster because of it."
- Most teams have solo wins. The faster teams have reorganized *how work gets done.*
- **The agent isn't the point. The system your team builds around it is.**

---

## 2. AI Maturity Model (2 min)

> ⚠️ **External slide — speaker will bring this in separately.**

*Bridge from Hook: "Before we talk about the system, let's name where most teams are today."*

The maturity model maps the progression from autocomplete to agentic-first:

| Level | Mode | What it looks like |
|---|---|---|
| 1 | **Autocomplete** | Tab-complete, inline suggestions |
| 2 | **Chat** | Ask Copilot questions, get explanations |
| 3 | **Agent mode** | Autonomous edits in your local IDE |
| 4 | **Agentic workflows** | Cloud coding agent, background tasks, PR creation |
| 5 | **Agentic-first team** | Shared system — instructions, agents, skills, governance |

- Most teams are at Level 1–2. A few individuals have reached Level 3.
- The gap: **Level 5 requires a system, not just a tool.**
- This talk is about getting your *team* to Level 4–5.

*Narration bridge: "Let me show you why most teams stall at Level 2 — and what it takes to move."*

---

## 3. The Problem with Solo Wins (2 min)

- Each dev starts from scratch every session — no shared context
- Copilot doesn't know how *your* team writes code, reviews PRs, or deploys
- Tribal knowledge lives in people's heads, not in the tools
- The gap between "one person is faster" and "the team ships faster"

---

## 4. The System: What Goes in `.github/` (7 min)

*The composition model — the talk's core concept*

| Asset | What it does |
|---|---|
| `copilot-instructions.md` | Raises the floor — every dev gets the same baseline |
| `*.instructions.md` | Always-on coding standards (Clean Architecture, DDD, etc.) |
| `*.prompt.md` | Reusable one-click automation (PR descriptions, commit messages) |
| `SKILL.md` | Portable multi-step capabilities with scripts and resources |
| `*.agent.md` | Role-based specialists with tool restrictions and handoffs |
| `.mcp.json` | The execution layer — connects agents to real tools |

- Show the actual `.github/` folder from the demo repo
- Key point: **version-controlled, PR-reviewed, shared with the whole team**
- Decision tree: Instructions (always-on guardrails) → Skills (portable SOPs) → Agents (role-based specialists) → Prompts (lightweight macros)
- "Skills: the brain. MCP: the hands."

---

## 5. Adding Azure: The Azure Skills Plugin (3 min)

- One install, three layers: **Skills** (20 curated workflows) + **Azure MCP Server** (200+ tools) + **Foundry MCP** (AI specialist)
- Not a prompt pack — curated workflows, decision trees, and guardrails encoded in plain text
- Before/after: same prompt, completely different outcome
- `azure-prepare` → `azure-validate` → `azure-deploy` in sequence
- The skills live in `.github/plugins/azure-skills/` — visible, auditable, team-owned

---

## 6. Live Demo (12 min)

*Using Azure Skills on the Task Library application — narrate every step*

1. **Set the scene** — show the running Task Library app, walk the `.github/` folder: instructions, agents, `.mcp.json`, Azure Skills in `.github/plugins/azure-skills/`
2. **Prepare** — run `azure-prepare` on the project; watch it analyze the app and generate `Dockerfile`, `azure.yaml`, and Bicep infra
3. **Validate** — run `azure-validate`; pre-flight checks on resources, quotas, and permissions before burning time on a failed deploy
4. **Deploy** — run `azure-deploy`; `azd up` orchestrates the full deployment pipeline end-to-end
5. **Operate** — use `@azure` to query live resources: list resource groups, check App Insights logs, inspect the deployed function app
6. **Diagnose** — use `azure-diagnostics` to troubleshoot a real or simulated issue using logs, metrics, and KQL
7. **Narrate throughout** — contrast "what Copilot would have said without the plugin" vs. "what it actually does with the skills"

*Key moment: show the before/after — same prompt, completely different outcome*

---

## 7. Governance: This Isn't Blind Trust (3 min)

- Skills are **plain text, version-controlled, PR-reviewed** — not a black box
- Org-level controls: model policies, which agents are enabled, MCP server allow-list
- Audit log: `actor:Copilot` shows every action the agent took on GitHub
- Agent lifecycle: Proposal → Review → Production → Deprecation
- *"The right model is curated skills, trusted sources, tool approvals, and least-privilege access."*

---

## 8. How to Start (2 min)

Three steps to get your team moving:

1. **Add `copilot-instructions.md`** — encode how your team writes code
2. **Install the Azure Skills Plugin** — `aka.ms/azure-plugin`
3. **Build one agent** for a workflow your team repeats (code review, backlog generation, etc.)

Repo with all assets: `github.com/shawnewallace/2026-global-azure-columbus`

---

## Timing Notes

| Section | Time |
|---|---|
| Hook | 2 min |
| AI Maturity Model *(external slide)* | 2 min |
| The Problem with Solo Wins | 2 min |
| The System: What Goes in `.github/` | 7 min |
| Adding Azure: The Azure Skills Plugin | 3 min |
| Live Demo | 12 min |
| Governance | 3 min |
| How to Start | 2 min |
| **Total** | **33 min** |

> **If running long:** Trim demo to 10 min (saves 2 min), or cut governance to 2 min (saves 1 min). Target is 30 min.

---

## Key Takeaways (from abstract)

- How instructions, prompts, agents, skills, and MCP tools compose together in `.github/`
- Live demo: using Microsoft's Azure Skills and `@azure` to deploy and operate Azure resources from Copilot
- Shared assets — `copilot-instructions.md`, `AGENTS.md`, PR templates — that raise the floor for every developer
- Enterprise governance: model policies, audit logs, and the agent control plane
- How to move your team from solo wins to agentic-first workflows (the maturity progression)
