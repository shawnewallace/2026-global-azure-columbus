targetScope = 'subscription'

@minLength(1)
@maxLength(64)
param environmentName string

@minLength(1)
param location string

@minLength(1)
@description('Owner tag value required by Azure Policy on all resource groups')
param owner string

var resourceSuffix = take(uniqueString(subscription().id, environmentName, location), 6)
var tags = { 'azd-env-name': environmentName, Owner: owner }

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}

module monitoring './modules/monitoring.bicep' = {
  name: 'monitoring'
  scope: rg
  params: {
    name: 'log-${environmentName}-${resourceSuffix}'
    location: location
    tags: tags
  }
}

module registry './modules/registry.bicep' = {
  name: 'registry'
  scope: rg
  params: {
    name: replace('cr${environmentName}${resourceSuffix}', '-', '')
    location: location
    tags: tags
  }
}

module keyVault './modules/keyvault.bicep' = {
  name: 'keyVault'
  scope: rg
  params: {
    name: take('kv-${environmentName}-${resourceSuffix}', 24)
    location: location
    tags: tags
  }
}

module postgres './modules/postgres.bicep' = {
  name: 'postgres'
  scope: rg
  params: {
    name: 'pg-${environmentName}-${resourceSuffix}'
    location: 'eastus2'  // eastus is subscription-restricted for PostgreSQL Flexible Server
    tags: tags
  }
}

module openai './modules/openai.bicep' = {
  name: 'openai'
  scope: rg
  params: {
    name: 'oai-${environmentName}-${resourceSuffix}'
    location: location
    tags: tags
  }
}

// Phase 1: Container Apps environment + API app (no registry link yet)
module containerApps './modules/container-apps.bicep' = {
  name: 'containerApps'
  scope: rg
  params: {
    environmentName: '${environmentName}-${resourceSuffix}'
    appName: 'api-${environmentName}-${resourceSuffix}'
    location: location
    tags: tags
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    logAnalyticsWorkspaceKey: monitoring.outputs.logAnalyticsWorkspaceKey
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    openAiEndpoint: openai.outputs.endpoint
    openAiDeployment: 'gpt-4o-mini'
    webUrl: staticWebApp.outputs.url
    postgresConnectionString: postgres.outputs.connectionString
  }
}

// Phase 2: AcrPull role assignment — separate module avoids circular dependency
module acrPullRole './modules/acr-pull-role.bicep' = {
  name: 'acrPullRole'
  scope: rg
  params: {
    acrName: registry.outputs.name
    principalId: containerApps.outputs.apiPrincipalId
  }
}

// Key Vault Secrets User for API managed identity
module kvSecretsRole './modules/keyvault-role.bicep' = {
  name: 'kvSecretsRole'
  scope: rg
  params: {
    keyVaultName: keyVault.outputs.name
    principalId: containerApps.outputs.apiPrincipalId
  }
}

// Cognitive Services OpenAI User for API managed identity
module openAiRole './modules/openai-role.bicep' = {
  name: 'openAiRole'
  scope: rg
  params: {
    openAiName: openai.outputs.name
    principalId: containerApps.outputs.apiPrincipalId
  }
}

module staticWebApp './modules/static-web-app.bicep' = {
  name: 'staticWebApp'
  scope: rg
  params: {
    name: 'swa-${environmentName}-${resourceSuffix}'
    location: 'eastus2'
    tags: tags
  }
}

// Outputs — UPPERCASE names become azd env vars
output AZURE_RESOURCE_GROUP string = rg.name
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = registry.outputs.loginServer
output AZURE_KEY_VAULT_NAME string = keyVault.outputs.name
output AZURE_LOG_ANALYTICS_WORKSPACE_ID string = monitoring.outputs.logAnalyticsWorkspaceId
output AZURE_OPENAI_ENDPOINT string = openai.outputs.endpoint
output API_URL string = containerApps.outputs.apiUrl
output WEB_URL string = staticWebApp.outputs.url
