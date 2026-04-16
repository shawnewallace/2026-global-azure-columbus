targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment (e.g. task-library)')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string

@description('Owner tag value required by subscription policy')
param ownerTag string = 'global-azure-demo'

var resourceGroupName = 'rg-${environmentName}'
var tags = { 'azd-env-name': environmentName, Owner: ownerTag }

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

module appInsights 'modules/appInsights.bicep' = {
  name: 'appInsights'
  scope: rg
  params: {
    environmentName: environmentName
    location: location
    tags: tags
  }
}

module postgresql 'modules/postgresql.bicep' = {
  name: 'postgresql'
  scope: rg
  params: {
    environmentName: environmentName
    location: location
    tags: tags
  }
}

module openAi 'modules/openAi.bicep' = {
  name: 'openAi'
  scope: rg
  params: {
    environmentName: environmentName
    location: location
    tags: tags
  }
}

module keyVault 'modules/keyVault.bicep' = {
  name: 'keyVault'
  scope: rg
  params: {
    environmentName: environmentName
    location: location
    tags: tags
    postgresConnectionString: postgresql.outputs.connectionString
  }
}

module appServicePlan 'modules/appServicePlan.bicep' = {
  name: 'appServicePlan'
  scope: rg
  params: {
    environmentName: environmentName
    location: location
    tags: tags
  }
}

module appService 'modules/appService.bicep' = {
  name: 'appService'
  scope: rg
  params: {
    environmentName: environmentName
    location: location
    tags: tags
    appServicePlanId: appServicePlan.outputs.id
    appInsightsConnectionString: appInsights.outputs.connectionString
    keyVaultName: keyVault.outputs.name
    openAiEndpoint: openAi.outputs.endpoint
    openAiDeployment: openAi.outputs.deploymentName
  }
}

// Grant App Service managed identity access to Key Vault
module keyVaultRoleAssignment 'modules/keyVaultRoleAssignment.bicep' = {
  name: 'keyVaultRoleAssignment'
  scope: rg
  params: {
    keyVaultName: keyVault.outputs.name
    principalId: appService.outputs.principalId
  }
}

// Grant App Service managed identity Cognitive Services OpenAI User role
module openAiRoleAssignment 'modules/openAiRoleAssignment.bicep' = {
  name: 'openAiRoleAssignment'
  scope: rg
  params: {
    openAiName: openAi.outputs.name
    principalId: appService.outputs.principalId
  }
}

module staticWebApp 'modules/staticWebApp.bicep' = {
  name: 'staticWebApp'
  scope: rg
  params: {
    environmentName: environmentName
    location: location
    tags: tags
    apiBaseUrl: appService.outputs.uri
  }
}

output AZURE_LOCATION string = location
output AZURE_TENANT_ID string = tenant().tenantId
output SERVICE_API_ENDPOINTS array = ['${appService.outputs.uri}/api']
output VITE_API_BASE_URL string = appService.outputs.uri
output SERVICE_WEB_URI string = staticWebApp.outputs.uri
