THe event: https://coazure.github.io/cbus-global-azure-2026/agenda/#agentic-first-with-github-copilot

Workshop I give: https://github.com/centricconsulting/ai-coding-workshop


# Research Notes

## Azure Skills Plugin

**Source:** https://devblogs.microsoft.com/all-things-azure/announcing-the-azure-skills-plugin/
**GitHub:** https://github.com/microsoft/azure-skills
**Install:** https://aka.ms/azure-plugin

### What it is
- One install, three layers: **Skills (brain)** + **Azure MCP Server (hands)** + **Foundry MCP (AI specialist)**
- Not a prompt pack — curated workflows, decision trees, and guardrails
- Skills files are plain text, version-controlled, auditable, loadable on demand

### Azure Skills (20 curated)
- **Build & deploy:** `azure-prepare`, `azure-validate`, `azure-deploy`
- **Troubleshoot & operate:** `azure-diagnostics`, `azure-observability`, `azure-compliance`
- **Optimize & design:** `azure-cost-optimization`, `azure-compute`, `azure-resource-visualizer`
- **Data, AI & platform:** `azure-ai`, `azure-aigateway`, `azure-storage`, `azure-kusto`, `azure-rbac`, `azure-cloud-migrate`, `entra-app-registration`, `microsoft-foundry`

### Azure MCP Server
- 200+ structured tools across 40+ Azure services
- Real operations: list resources, check prices, query logs, run diagnostics, provision infra, drive deployment workflows

### Foundry MCP
- Model catalog, deployments, agents, evaluations
- Microsoft Foundry integration

### Multi-host support
- GitHub Copilot in VS Code
- Copilot CLI: `/plugin install azure@azure-skills`
- Claude Code
- IntelliJ IDEA

### Install locations
- Skills live in `.github/plugins/azure-skills/` in the workspace
- MCP config via `.mcp.json`

### Before/After (key demo talking point)
| | Without Plugin | With Plugin |
|---|---|---|
| Prompt | "Deploy my Python Flask API to Azure" | Same |
| Behavior | Generic tutorial, maybe some `az` commands | Activates `azure-prepare`, `azure-validate`, `azure-deploy` in sequence |
| Who decides? | You | Copilot follows Azure workflow encoded in skills |
| Output | Advice | Dockerfile, infra files, azure.yaml, validation output, deployment actions |
| What runs? | Usually nothing | Real MCP tool calls against Azure and Foundry |

### Blog series planned
1. Install Guide
2. How Skills + MCP Work Together
3. The Core Workflow (Prepare → Validate → Deploy)
4. Skills That Save You Hours
5. Inside a SKILL.md
6. Real-World Walkthrough
7. What's Next

---

## Copilot Cloud Agent (Coding Agent)

**Source:** https://docs.github.com/en/copilot/using-github-copilot/coding-agent/about-assigning-tasks-to-copilot

### What it is
- Copilot works **autonomously in the background** to complete tasks (like a human developer)
- Runs in ephemeral GitHub Actions environment
- Available on Copilot Pro, Pro+, Business, Enterprise

### What it can do
- Research a repository, create implementation plans
- Fix bugs, implement new features, improve test coverage
- Update docs, address technical debt, resolve merge conflicts
- Automates branch creation, commit messages, and push
- Open pull requests

### Entry points
- GitHub Issues (assign Copilot as the assignee)
- Agents panel at github.com/copilot/agents
- `@copilot` mention in a PR comment
- VS Code

### Coding Agent vs Agent Mode (important distinction for talk)
- **Copilot cloud agent**: autonomous, runs on GitHub Actions, works on GitHub — background, async
- **Agent mode in IDE**: autonomous edits directly in local dev environment — synchronous, local

### Custom Agents
- Create specialized agents for different tasks (frontend, docs, testing, etc.)
- Each tailored with specific prompts and tools

### Governance metrics available
- Total PRs created and merged by Copilot
- Median time to merge for Copilot-created PRs
- Useful for tracking adoption and throughput changes

---

## Repository Custom Instructions

**Source:** https://docs.github.com/en/copilot/customizing-copilot/adding-repository-custom-instructions-for-github-copilot

### Three types
1. **`copilot-instructions.md`** (`.github/`) — repository-wide, applies to all requests
2. **`NAME.instructions.md`** (`.github/instructions/`) — path-specific, applies to matching files
3. **`AGENTS.md`** — agent instructions; nearest file in directory tree takes precedence (also supports `CLAUDE.md`, `GEMINI.md` in repo root)

### What to put in copilot-instructions.md
- What the repo does
- How to build, test, lint, run
- Project layout and architecture
- Key file paths, config locations
- CI/CD and validation pipelines

### Key insight for talk
> The shared assets (copilot-instructions.md, AGENTS.md, PR templates) **raise the floor for every developer** — they encode how the team works into the tools.

---

## Enterprise Governance

**Source:** https://docs.github.com/en/copilot/managing-copilot/managing-github-copilot-in-your-organization/managing-policies-for-copilot-in-your-organization

### Policy controls (org/enterprise level)
- Enable/disable specific Copilot features
- **Model policies**: control which models are available (beyond base models)
- **MCP servers in Copilot policy**: controls MCP server access (GA feature)
- Enable/disable third-party coding agents (Anthropic Claude, OpenAI Codex)
- Preview feature opt-ins

### Audit logs
**Source:** https://docs.github.com/en/copilot/managing-copilot/managing-github-copilot-in-your-organization/reviewing-activity-related-to-github-copilot-in-your-organization/reviewing-audit-logs-for-copilot-business

- Changes to Copilot plan, settings, policies, license assignments
- **Agent activity** on GitHub website (use `actor:Copilot` search)
- Search with `action:copilot` for all Copilot events
- Logs retained 180 days; recommend streaming to SIEM
- **Does NOT include** client session data (prompts sent locally) — custom hooks needed for that
- See also: [Audit log events for agents](https://docs.github.com/en/copilot/reference/agentic-audit-log-events)

---

## Workshop: AI Coding with GitHub Copilot (.NET Edition)

**Source:** https://github.com/centricconsulting/ai-coding-workshop

This is your own workshop and is the richest source of reusable material for this talk. It covers the full customization hierarchy hands-on.

### What the workshop covers
- **Part 1 (3h):** Fundamentals — copilot-instructions.md, TDD, requirements → code, refactoring, testing, docs, conventional commits
- **Part 2 (3h):** Advanced — Interaction models, Skills, custom agents, agent design, governance
- Full Clean Architecture + DDD + TDD .NET 9 solution as the demo vehicle

### The customization hierarchy (directly maps to talk takeaway #1)

```
Prompts → Instructions → Skills → Agents
```

| Type | File | When Active | Best For |
|------|------|-------------|----------|
| Instructions | `*.instructions.md` | Always-on | Coding standards, guardrails |
| Skills | `SKILL.md` | On-demand | Portable multi-step capabilities w/ scripts |
| Agents | `*.agent.md` | When selected | Role-based workflows, tool restrictions, handoffs |
| Prompts | `*.prompt.md` | On-demand | Simple one-off task automation |

**Key insight from customization-decision-guide.md:**
- **Skills** = a specialized **toolkit** you give to any agent (portable, can include scripts/resources)
- **Agents** = a role-based **specialist** you hire for a workflow (tool restrictions, handoffs, model selection)
- Skills are multi-host portable (VS Code, CLI, Claude Code, cloud agent) — Agents are VS Code + cloud only

### Agents in the workshop (`.github/agents/`)
Real examples to show or reference in the talk:
- `architect.agent.md` — planning, ADRs, backlog curation (markdown editing only)
- `architecture-reviewer.agent.md` — read-only code review for Clean Architecture compliance
- `backlog-generator.agent.md` — requirements → user stories
- `plan.agent.md` (Scout) — strategic analysis, read-only, no implementation
- `test-strategist.agent.md` — test strategy and coverage planning
- `expert-dotnet-software-engineer.agent.md` — SOLID, TDD, performance
- `api-architect.agent.md` (Wire) — API client code with resilience patterns
- `Check.agent.md` — code review before PRs
- `ellie.agent.md` — (larger agent, likely a more complex persona)

### Skills in the workshop (`.github/skills/`)
- `test-data-generator/` — domain knowledge + templates for realistic test data, no tool access needed

### Governance (docs/guides/agent-governance.md)
Full lifecycle defined — directly applicable to the "governance" section of the talk:
- **6 stages:** Proposal → Development → Review → Production → Maintenance → Deprecation
- **Change types:** Minor (no review), Moderate (1 approval), Major (2 approvals + announcement)
- **Principles:** Quality over quantity, test before merge, team ownership, continuous improvement
- **Commit conventions:** `feat(agent):`, `fix(agent):`, `docs(agent):` etc.
- **Deprecation:** Front matter `deprecated: true`, grace period, then removal

### Key reusable framing from workshop

**Decision tree for the talk:**
```
What do you need?
├─ Enforce coding standards always? → Instructions
├─ Portable capability with scripts/resources? → Skill
├─ Role-based workflow with tool restrictions? → Agent
└─ Quick one-off automation? → Prompt
```

**The mental model that resonates with audiences:**
- Instructions: "the rules your whole team agreed to" (always on, nobody thinks about it)
- Skills: "the SOP in a wiki, but the AI actually reads it" (on-demand, portable)
- Agents: "the specialist you call in for specific jobs" (role, restrictions, handoffs)
- Prompts: "the macro you run when you do the same thing every time" (lightweight)

### Assets that prove the "raise the floor" point
- `.github/copilot-instructions.md` — auto-applies Clean Architecture, DDD, .NET 9 to every request
- PR templates in `.github/PULL_REQUEST_TEMPLATE/`
- Every dev who clones the repo gets the same AI behavior — no manual setup

### Connecting workshop → talk arc
The workshop teaches "how to build the system." The talk is about "why teams should build this system and what it unlocks at scale (including Azure)." The workshop is your proof that this is teachable and practical — not just theoretical.

**Reference material to pull from for demo setup:**
- `docs/guides/customization-decision-guide.md` — decision tree / comparison tables are slide-ready
- `docs/guides/agent-governance.md` — lifecycle stages, change management for governance section
- `.github/agents/` — real agent files to show live
- `.github/copilot-instructions.md` — real instructions file to show the "floor raising" effect

---

## Key Themes / Angles for the Talk

### The composition model
```
.github/
  copilot-instructions.md   ← raises the floor for everyone
  instructions/             ← path-specific context
  prompts/                  ← reusable prompt library
  agents/                   ← agent definitions
  plugins/
    azure-skills/           ← Azure Skills Plugin installs here
  .mcp.json                 ← MCP server configuration
```

### The progression narrative
1. **Solo win** — one dev uses Copilot for autocomplete
2. **Team asset** — shared instructions + prompts + AGENTS.md encode team knowledge
3. **Agentic workflow** — coding agent works from issues → PRs autonomously
4. **Azure-capable agent** — Azure Skills Plugin adds Azure MCP + skills layer
5. **Governed system** — org policies, model controls, audit logs

### The live demo arc
Issue → coding agent writes code → Azure Skills wire up infra → azd deploy → PR opened
All narrated while it happens.

### Quotes/framings worth using
- "The agent isn't the point. The system your team builds around it is."
- "Skills: the brain. MCP: the hands. The plugin: keeps them aligned."
- "Skills scale expertise — one team packages hard-won Azure knowledge once and reuses it forever."
- "Same prompt. Completely different outcome." (before/after demo framing)
- Audit logs note: "The audit log does not include client session data" — important enterprise caveat to address honestly
