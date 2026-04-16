param environmentName string
param location string
param tags object
param appServicePlanId string
param appInsightsConnectionString string
param keyVaultName string
param openAiEndpoint string
param openAiDeployment string

resource appService 'Microsoft.Web/sites@2022-03-01' = {
  name: 'app-${environmentName}-api'
  location: location
  tags: union(tags, { 'azd-service-name': 'api' })
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      http20Enabled: true
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
        {
          name: 'POSTGRES_CONNECTION_STRING'
          value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=postgres-connection-string)'
        }
        {
          name: 'AZURE_OPENAI_ENDPOINT'
          value: openAiEndpoint
        }
        {
          name: 'AZURE_OPENAI_DEPLOYMENT'
          value: openAiDeployment
        }
        {
          name: 'Cors__AllowedOrigins__0'
          value: '*'
        }
      ]
    }
  }
}

output id string = appService.id
output name string = appService.name
output uri string = 'https://${appService.properties.defaultHostName}'
output principalId string = appService.identity.principalId
