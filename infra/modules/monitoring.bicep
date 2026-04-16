targetScope = 'resourceGroup'

param name string
param location string = resourceGroup().location
param tags object = {}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: name
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'ai-${name}'
  location: location
  kind: 'web'
  tags: tags
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
    IngestionMode: 'LogAnalytics'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

output logAnalyticsWorkspaceId string = logAnalyticsWorkspace.properties.customerId
output logAnalyticsWorkspaceKey string = logAnalyticsWorkspace.listKeys().primarySharedKey
output appInsightsConnectionString string = appInsights.properties.ConnectionString
output appInsightsId string = appInsights.id
