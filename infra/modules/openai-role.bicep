targetScope = 'resourceGroup'

param openAiName string
param principalId string

resource openAiAccount 'Microsoft.CognitiveServices/accounts@2024-04-01-preview' existing = {
  name: openAiName
}

// Cognitive Services OpenAI User — call OpenAI APIs, no key needed
resource openAiUserRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(openAiAccount.id, principalId, 'openaicognitiveservicesuser')
  scope: openAiAccount
  properties: {
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'
    )
    principalId: principalId
    principalType: 'ServicePrincipal'
  }
}
