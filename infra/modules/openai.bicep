targetScope = 'resourceGroup'

param name string
param location string = resourceGroup().location
param tags object = {}

resource openAiAccount 'Microsoft.CognitiveServices/accounts@2024-04-01-preview' = {
  name: name
  location: location
  kind: 'OpenAI'
  tags: tags
  sku: {
    name: 'S0'
  }
  properties: {
    publicNetworkAccess: 'Enabled'
    customSubDomainName: name
  }
}

resource gpt4oMiniDeployment 'Microsoft.CognitiveServices/accounts/deployments@2024-04-01-preview' = {
  parent: openAiAccount
  name: 'gpt-4o-mini'
  sku: {
    name: 'GlobalStandard'
    capacity: 10
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4o-mini'
      version: '2024-07-18'
    }
  }
}

output name string = openAiAccount.name
output endpoint string = openAiAccount.properties.endpoint
output id string = openAiAccount.id
