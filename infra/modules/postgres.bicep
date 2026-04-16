targetScope = 'resourceGroup'

param name string
param location string = resourceGroup().location
param tags object = {}

var administratorLogin = 'pgadmin${take(uniqueString(resourceGroup().id), 8)}'
// Deterministic password — store in Key Vault post-deployment for production rotation
var administratorPassword = '${take(uniqueString(resourceGroup().id, name, 'pw'), 16)}Aa1!'

resource postgresServer 'Microsoft.DBforPostgreSQL/flexibleServers@2024-08-01' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: 'Standard_B1ms'
    tier: 'Burstable'
  }
  properties: {
    version: '16'
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
    storage: {
      storageSizeGB: 32
    }
    backup: {
      backupRetentionDays: 7
      geoRedundantBackup: 'Disabled'
    }
    highAvailability: {
      mode: 'Disabled'
    }
    authConfig: {
      activeDirectoryAuth: 'Enabled'
      passwordAuth: 'Enabled'
    }
  }
}

resource postgresDatabase 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2024-08-01' = {
  parent: postgresServer
  name: 'tasklibrary'
  properties: {
    charset: 'UTF8'
    collation: 'en_US.utf8'
  }
}

// Allow Azure services (Container Apps) to connect
resource firewallRuleAzure 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2024-08-01' = {
  parent: postgresServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

output serverName string = postgresServer.name
output serverFqdn string = postgresServer.properties.fullyQualifiedDomainName
output databaseName string = postgresDatabase.name
// Connection string passed securely to Container Apps as a direct secret
output connectionString string = 'Host=${postgresServer.properties.fullyQualifiedDomainName};Database=tasklibrary;Username=${administratorLogin};Password=${administratorPassword};SSL Mode=Require;Trust Server Certificate=false'
