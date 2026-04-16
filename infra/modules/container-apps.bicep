targetScope = 'resourceGroup'

param environmentName string
param appName string
param location string = resourceGroup().location
param tags object = {}
param logAnalyticsWorkspaceId string
param logAnalyticsWorkspaceKey string
param appInsightsConnectionString string
param openAiEndpoint string
param openAiDeployment string = 'gpt-4o-mini'
param webUrl string = ''
@secure()
param postgresConnectionString string

var containerImageName = 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: 'cae-${environmentName}'
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsWorkspaceId
        sharedKey: logAnalyticsWorkspaceKey
      }
    }
  }
}

// Phase 1: no registries block — azd deploy configures the registry/identity link
resource apiApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: appName
  location: location
  tags: union(tags, { 'azd-service-name': 'api' })
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    environmentId: containerAppsEnvironment.id
    configuration: {
      secrets: [
        {
          name: 'postgres-conn-string'
          value: postgresConnectionString
        }
      ]
      ingress: {
        external: true
        targetPort: 8080
        transport: 'auto'
        corsPolicy: {
          allowedOrigins: empty(webUrl) ? ['*'] : [webUrl]
          allowedMethods: ['GET', 'POST', 'PUT', 'PATCH', 'DELETE', 'OPTIONS']
          allowedHeaders: ['*']
        }
      }
    }
    template: {
      containers: [
        {
          name: 'api'
          image: containerImageName
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
          env: [
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: appInsightsConnectionString
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
              name: 'POSTGRES_CONNECTION_STRING'
              secretRef: 'postgres-conn-string'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 0
        maxReplicas: 3
      }
    }
  }
}

output apiUrl string = 'https://${apiApp.properties.configuration.ingress.fqdn}'
output apiPrincipalId string = apiApp.identity.principalId
output environmentId string = containerAppsEnvironment.id
