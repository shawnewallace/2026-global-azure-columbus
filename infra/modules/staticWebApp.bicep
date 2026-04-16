param environmentName string
param location string
param tags object
param apiBaseUrl string

resource staticWebApp 'Microsoft.Web/staticSites@2022-03-01' = {
  name: 'swa-${environmentName}'
  location: location
  tags: union(tags, { 'azd-service-name': 'web' })
  sku: {
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    buildProperties: {
      appLocation: '/'
      outputLocation: 'dist'
    }
  }
}

// Inject the API base URL so the Vite build picks it up via azd's env var pipeline
resource swaAppSettings 'Microsoft.Web/staticSites/config@2022-03-01' = {
  parent: staticWebApp
  name: 'appsettings'
  properties: {
    VITE_API_BASE_URL: apiBaseUrl
  }
}

output uri string = 'https://${staticWebApp.properties.defaultHostname}'
output name string = staticWebApp.name
