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

## 2. The Problem with Solo Wins (3 min)

- Each dev starts from scratch every session — no shared context
- Copilot doesn't know how *your* team writes code, reviews PRs, or deploys
- Tribal knowledge lives in people's heads, not in the tools
- The gap between "one person is faster" and "the team ships faster"

---

## 3. The System: What Goes in `.github/` (7 min)

*The composition model — the talk's core concept*

| Asset | What it does |
|---|---|
| `copilot-instructions.md` | Raises the floor — every dev gets the same baseline |
| `*.instructions.md` | Always-on coding standards (Clean Architecture, DDD, etc.) |
| `*.prompt.md` | Reusable one-click automation (PR descriptions, commit messages) |
| `SKILL.md` | Portable multi-step capabilities with scripts and resources |
| `*.agent.md` | Role-based specialists with tool restrictions and handoffs |
| `.mcp.json` | The execution layer — connects agents to real tools |

- Show the actual `.github/` folder from the repo
- Key point: **this is version-controlled, reviewable, and shared with the whole team**
- "Skills: the brain. MCP: the hands."

---

## 4. Adding Azure: The Azure Skills Plugin (3 min)

- One install, three layers: Skills (20 workflows) + Azure MCP (200+ tools) + Context7
- Before/after: same prompt, completely different outcome
- `azure-prepare` → `azure-validate` → `azure-deploy` in sequence
- The skills live in `.github/plugins/azure-skills/` — visible, auditable, team-owned

---

## 5. Live Demo (12 min)

*Starting from a GitHub issue, narrate every step*

1. **Show the system** — walk the `.github/` folder: instructions, agents, `.mcp.json`
2. **Assign the issue** to Copilot's coding agent from GitHub
3. **Watch it work** — agent reads instructions, creates a plan, writes code on a branch
4. **Azure Skills kick in** — `@azure` wires up infrastructure, runs `azure-prepare`
5. **Deploy** — `azd` runs the deployment pipeline live
6. **PR opened** — agent commits, pushes, opens PR with description

*Narrate what's happening and why at each step*

---

## 6. Governance: This Isn't Blind Trust (3 min)

- Skills are **plain text, version-controlled, PR-reviewed** — not a black box
- Org-level controls: model policies, which agents are enabled, MCP server allow-list
- Audit log: `actor:Copilot` shows every action the agent took on GitHub
- Agent lifecycle: Proposal → Review → Production → Deprecation
- *"The right model is curated skills, trusted sources, tool approvals, and least-privilege access."*

---

## 7. How to Start (2 min)

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
| The Problem with Solo Wins | 3 min |
| The System: What Goes in `.github/` | 7 min |
| Adding Azure: The Azure Skills Plugin | 3 min |
| Live Demo | 12 min |
| Governance | 3 min |
| How to Start | 2 min |
| **Total** | **32 min** |

> **If running long:** Trim demo to 10 min, or cut governance to 2 min.

---

## Key Takeaways (from abstract)

- How instructions, prompts, agents, skills, and MCP tools compose together in `.github/`
- Live demo: using Microsoft's Azure Skills and `@azure` to deploy and operate Azure resources from Copilot
- Shared assets — `copilot-instructions.md`, `AGENTS.md`, PR templates — that raise the floor for every developer
- Enterprise governance: model policies, audit logs, and the agent control plane
- How to move your team from solo wins to agentic-first workflows
