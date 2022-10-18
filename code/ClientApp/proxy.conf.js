const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:21255';

const PROXY_CONFIG = [
  {
    context: [
      "/api/weatherforecast",
      "/api/customers",
      "/api/auth/login",
      "/api/token/refresh",
      "/api/token/revoke"
   ],
    target: target,
    secure: false,
    pathRewrite: { "^/api": "" },
    headers: {
      Connection: 'Keep-Alive'
    }
  }
]

module.exports = PROXY_CONFIG;
