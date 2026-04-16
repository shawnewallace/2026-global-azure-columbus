targetScope = 'resourceGroup'

param name string
param location string = 'eastus2'  // Static Web Apps has limited region availability
param tags object = {}

resource staticWebApp 'Microsoft.Web/staticSites@2022-09-01' = {
  name: name
  location: location
  tags: union(tags, { 'azd-service-name': 'web' })
  sku: {
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    stagingEnvironmentPolicy: 'Enabled'
    allowConfigFileUpdates: true
    enterpriseGradeCdnStatus: 'Disabled'
  }
}

output name string = staticWebApp.name
output url string = 'https://${staticWebApp.properties.defaultHostname}'
output id string = staticWebApp.id
