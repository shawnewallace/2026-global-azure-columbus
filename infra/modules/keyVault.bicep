param environmentName string
param location string
param tags object

@secure()
param postgresConnectionString string

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: 'kv-${environmentName}'
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: tenant().tenantId
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
  }
}

resource connectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault
  name: 'postgres-connection-string'
  properties: {
    value: postgresConnectionString
  }
}

output name string = keyVault.name
output uri string = keyVault.properties.vaultUri
