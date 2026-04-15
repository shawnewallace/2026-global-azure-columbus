#!/bin/bash
set -e

echo ""
echo "================================================================"
echo "  Global Azure Columbus 2026 — Demo Environment Setup"
echo "================================================================"
echo ""

# ── Azure Developer CLI (azd) ────────────────────────────────────────
echo "▶ Installing Azure Developer CLI (azd)..."
curl -fsSL https://aka.ms/install-azd.sh | bash -s -- --version latest
echo "✓ azd installed: $(azd version 2>/dev/null || echo 'restart shell to verify')"
echo ""

# ── Azure Skills Plugin (Copilot CLI) ────────────────────────────────
# Pre-install the plugin files so the CLI plugin is ready to activate.
# The user still needs to run: /plugin install azure@azure-skills
# but this ensures the marketplace entry is registered.
echo "▶ Registering Azure Skills Plugin marketplace..."
if command -v gh &> /dev/null; then
  echo "  (GitHub CLI available — plugin activation works in Copilot CLI session)"
else
  echo "  ⚠ GitHub CLI not found; plugin install requires 'gh' CLI"
fi
echo ""

# ── Pre-warm MCP server packages ─────────────────────────────────────
# Cache the Azure MCP and Foundry MCP npm packages so first-run
# `npx` calls don't download during a live demo.
echo "▶ Pre-caching Azure MCP server package..."
npx --yes @azure/mcp@latest --version 2>/dev/null && echo "✓ @azure/mcp cached" || echo "  ⚠ Could not pre-cache @azure/mcp (will download on first use)"
echo ""

echo "▶ Pre-caching Context7 MCP server package..."
npx --yes @upstash/context7-mcp@latest --version 2>/dev/null && echo "✓ @upstash/context7-mcp cached" || echo "  ⚠ Could not pre-cache @upstash/context7-mcp (will download on first use)"
echo ""


# ── Azure Skills Plugin files ─────────────────────────────────────────
# Pull skill files directly from microsoft/azure-skills into .github/plugins/
# so they're visible and version-controllable in the project.
echo "▶ Installing Azure Skills into .github/plugins/azure-skills/..."
if [ ! -d ".github/plugins/azure-skills" ]; then
  git clone --depth=1 --filter=blob:none --sparse \
    https://github.com/microsoft/azure-skills.git /tmp/azure-skills 2>/dev/null
  cd /tmp/azure-skills
  git sparse-checkout set .github/plugins/azure-skills 2>/dev/null
  cd - > /dev/null
  mkdir -p .github/plugins
  cp -r /tmp/azure-skills/.github/plugins/azure-skills .github/plugins/
  rm -rf /tmp/azure-skills
  echo "✓ Azure Skills installed to .github/plugins/azure-skills/"
else
  echo "✓ Azure Skills already present, skipping"
fi
echo ""
if [ -f ".config/dotnet-tools.json" ]; then
  echo "▶ Restoring .NET local tools..."
  dotnet tool restore
  echo "✓ .NET tools restored"
  echo ""
fi

# ── Web dependencies ─────────────────────────────────────────────────
# Pre-install npm packages so the React dev server starts instantly.
echo "▶ Installing web dependencies..."
if [ -d "src/web" ]; then
  cd src/web && npm install --silent && cd - > /dev/null
  echo "✓ Web dependencies installed"
else
  echo "  ⚠ src/web not found, skipping"
fi
echo ""

# ── .NET API build ────────────────────────────────────────────────────
# Pre-build the API so `dotnet run` starts quickly on postStartCommand.
echo "▶ Building .NET API..."
if [ -d "src/api" ]; then
  cd src/api && dotnet build --nologo -v q \
    && echo "✓ .NET API built" \
    || echo "  ⚠ Build failed — check compiler output above"
  cd - > /dev/null
else
  echo "  ⚠ src/api not found, skipping"
fi
echo ""

# ── EF Core Migrations ────────────────────────────────────────────────
# Apply database migrations against the local PostgreSQL container.
# The connection string is injected via the POSTGRES_CONNECTION_STRING
# environment variable set in docker-compose.yml.
echo "▶ Applying EF Core migrations..."
if [ -d "src/api/src/TaskLibrary.Infrastructure" ]; then
  cd src/api
  dotnet ef database update \
    --project src/TaskLibrary.Infrastructure \
    --startup-project src/TaskLibrary.Api \
    && echo "✓ Database migrations applied" \
    || echo "  ⚠ Migrations failed — run manually: dotnet ef database update"
  cd - > /dev/null
else
  echo "  ⚠ Infrastructure project not found, skipping migrations"
fi
echo ""

# ── Done ─────────────────────────────────────────────────────────────
echo "================================================================"
echo "  ✅ Environment ready!"
echo "================================================================"
echo ""
echo "  Azure Skills + MCP servers are active in VS Code via the"
echo "  Azure MCP extension and .mcp.json. No extra steps needed."
echo ""
echo "  Before running Azure demos, sign in:"
echo ""
echo "    az login"
echo "    azd auth login"
echo ""
echo "  Optional: to also use Azure Skills in Copilot CLI:"
echo ""
echo "    /plugin marketplace add microsoft/azure-skills"
echo "    /plugin install azure@azure-skills"
echo ""
