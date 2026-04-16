# Demo Script — Global Azure Columbus
**Talk:** Agentic-First with GitHub Copilot: Teams That Work With AI, Not Just Next to It
**Date:** April 18, 2026
**Demo duration:** ~12 min

---

## Before You Start

Verify everything is running:

```bash
# API on :8080
curl http://localhost:8080/api/tasks

# Web on :5173
open http://localhost:5173
```

If not running:
```bash
# Start API
cd /workspaces/2026-global-azure-columbus/src/api
dotnet run --project src/TaskLibrary.Api &

# Start web
cd /workspaces/2026-global-azure-columbus/src/web
npm run dev &
```

---

## Step 1 — Set the Scene (~2 min)

**Show the running app in the browser.**

> "This is the Task Library — a simple task manager we built live using the system I'm about to show you. Issues from GitHub, code written by Copilot's coding agent, tests first, clean architecture enforced automatically. Let me show you what made that possible."

**In VS Code, open the `.github/` folder and narrate each item:**

| Path | What to say |
|---|---|
| `copilot-instructions.md` | "Every developer on this team gets this context automatically — no onboarding doc, no tribal knowledge." |
| `instructions/` | "Always-on coding standards: DDD, Clean Architecture, Object Calisthenics. They fire on every file, every session." |
| `agents/` | "Role-based specialists — Sharp for .NET, Scout for planning, Archy for architecture decisions, Wire for API integration." |
| `plugins/azure-skills/` | "And this — the Azure Skills plugin. 20+ curated workflows for preparing, validating, deploying, and operating Azure." |

**Open `plugins/azure-skills/skills/azure-prepare/` and show the `SKILL.md`:**

> "Skills are plain text. Version-controlled. PR-reviewed. Not a black box — your team owns them."

> "Skills are the brain. MCP is the hands." — point to `.github/plugins/azure-skills/.mcp.json`

---

## Step 2 — Prepare (`azure-prepare`) (~3 min)

> **Demo choice — "blank slate" vs "regenerate":**
> - **Blank slate (crisper):** Delete `azure.yaml` and the `infra/` folder before running the prompt. The audience sees the skill discover everything from scratch — including `AzureOpenAILlmService.cs`, which triggers Azure OpenAI + model deployment + managed identity role assignment in the generated Bicep. Maximum "wow" factor.
> - **Regenerate (safer):** Leave the existing artifacts in place. The skill updates them. Less dramatic but lower risk if something goes wrong live.
>
> For a live demo, **blank slate** is recommended. Do it right before you take the stage:
> ```bash
> rm -rf infra/ azure.yaml .azure/
> ```
> - `infra/` and `azure.yaml` — removed so the skill generates them from scratch (maximum "wow" factor)
> - `.azure/` — contains saved azd environment state (`global-azure-demo`, subscription ID, previous deployment URLs). If left in place, `azd up` skips the environment-setup step and re-deploys against the old environment instead of provisioning fresh.

**Optional warm-up — verify the skills layer is active (from the README):**

```
What Azure services would I need to deploy this project?
```

> You should get structured Azure guidance, not a generic cloud answer. If you get generic advice, the skills aren't loaded.

**Then kick off prepare:**

```
Prepare this app for Azure. The app uses Azure OpenAI for AI-powered task prioritization — make sure the OpenAI resource and model deployment are included in the infrastructure.
```

> The explicit mention of Azure OpenAI ensures the skill provisions it. The skill will also find `AzureOpenAILlmService.cs` and `AZURE_OPENAI_ENDPOINT` in the code as corroborating signals.

**Narrate while it runs:**

> "Without the plugin, Copilot would guess at your infra. With the skill, it knows this is a .NET 10 API with Postgres, it knows your team's standards, and it generates the right artifacts."

**Watch for these outputs:**
- `azure.yaml` (azd manifest)
- Bicep infra: App Service + Postgres + Key Vault + **Azure OpenAI** + managed identity role assignments
- The skill finds `AzureOpenAILlmService.cs` and knows the app needs OpenAI — you didn't ask for it explicitly

> "One prompt. Production-ready infrastructure. And notice — it found the AI service requirement from the code. We didn't tell it. The skill read the codebase."

---

## Step 3 — Validate (`azure-validate`) (~2 min)

**In Copilot Chat:**

```
Validate my Azure deployment files before I run azd up.
```

> This is the exact prompt from the official azure-skills README.

**Narrate:**

> "Before we burn time on a failed deploy — pre-flight checks. Subscription quotas, RBAC permissions, naming conventions, resource availability in the target region."

**Watch for:**
- Green: "Safe to proceed"
- Red: surfaces blockers with remediation steps

> "This is governance built into the workflow. Not a checklist someone forgets to run."

---

## Step 4 — Deploy (`azure-deploy`) (~3 min)

**In Copilot Chat:**

```
Deploy this project to Azure.
```

> This is the exact prompt from the official azure-skills README.

**Narrate while `azd up` runs:**

> "No terminal commands to memorize. No YAML hunting. The skill knows the sequence: provision infra, build the container, push, configure the connection string, wire up managed identity."

> "And notice — no secrets. DefaultAzureCredential. The skill encoded that practice too."

**Once deployed — show the app URL in the browser.**

---

## Step 5 — Operate & Diagnose (`azure-diagnostics`) (~2 min)

**Optional: set the OpenAI endpoint in the App Service config to show the AI suggestion working for real.**

**In Copilot Chat:**

```
Troubleshoot why my app is failing health probes.
```

> This is the exact prompt from the official azure-skills README.

**Show structured telemetry in App Insights:**

> "Remember the correlation IDs and LLM token counts we added? Now watch them show up here — in structured logs, queryable with KQL, correlated across the full request chain."

**Point to a log entry with `CorrelationId`, `PromptTokens`, `CompletionTokens`, `LatencyMs`:**

> "This is what Issue #4 was. The coding agent wrote this. The skill deployed it. Now the operations skill is reading it back."

---

## Step 6 — The Before/After (~1 min)

**Key moment — say this clearly:**

> "Here's the thing. You could have typed these same prompts six months ago. 'Deploy my app to Azure.' And Copilot would have given you a generic answer — maybe some ARM JSON, a reminder to install the Azure CLI."

> "The difference isn't the model. It's the system. The skills encode *how your team deploys*. The instructions encode *how your team writes code*. The agents encode *who does what*. The model just executes."

> "That's the shift from Level 2 to Level 5. You're not prompting better. You're building a system that makes every prompt better — for every developer, every session."

---

## Closing Line

> "The agent isn't the point. The system your team builds around it is."

---

## Fallback (if deploy fails)

If `azd up` errors during the live demo:

1. Show the generated `Dockerfile` and Bicep files — "the artifacts are right, the infra is right"
2. Switch to `@azure` resource queries against a pre-deployed environment if available
3. Fall back to showing App Insights logs from a previous run

> "Live demos. The best part of any conference talk."
