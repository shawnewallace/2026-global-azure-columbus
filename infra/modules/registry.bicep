targetScope = 'resourceGroup'

// ACR names must be alphanumeric only, 5-50 chars — caller must strip hyphens
param name string
param location string = resourceGroup().location
param tags object = {}

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
    publicNetworkAccess: 'Enabled'
  }
}

output name string = containerRegistry.name
output loginServer string = containerRegistry.properties.loginServer
output id string = containerRegistry.id
