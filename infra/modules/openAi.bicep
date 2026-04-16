param environmentName string
param location string
param tags object

@description('Name of the gpt-4o-mini model deployment')
param deploymentName string = 'gpt-4o-mini'

resource openAiAccount 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: 'oai-${environmentName}'
  location: location
  tags: tags
  kind: 'OpenAI'
  sku: {
    name: 'S0'
  }
  properties: {
    customSubDomainName: 'oai-${environmentName}'
    publicNetworkAccess: 'Enabled'
    // Managed identity is the primary auth path (DefaultAzureCredential).
    // Key-based auth is left enabled for local developer convenience.
    disableLocalAuth: false
  }
}

resource modelDeployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  parent: openAiAccount
  name: deploymentName
  sku: {
    name: 'GlobalStandard'
    capacity: 10
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4o-mini'
    }
  }
}

output endpoint string = openAiAccount.properties.endpoint
output name string = openAiAccount.name
output id string = openAiAccount.id
output deploymentName string = modelDeployment.name
