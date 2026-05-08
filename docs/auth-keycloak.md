# Keycloak auth flow

Mapper uses Keycloak as the OpenID Connect provider for local Docker Compose.

## Local endpoints

| Purpose | URL |
| --- | --- |
| Keycloak admin UI | `http://localhost:8080/auth/admin` |
| Mapper realm | `http://localhost:8080/auth/realms/mapper` |
| OIDC discovery | `http://localhost:8080/auth/realms/mapper/.well-known/openid-configuration` |
| Swagger UI through reverse proxy | `http://localhost:8080` |
| WebApi direct debug port | `http://localhost:5001` |
| Keycloak direct debug port | `http://localhost:5002/auth/admin` |

Admin credentials for local development:

| Field | Value |
| --- | --- |
| Username | `admin` |
| Password | `admin` |

Test user imported with the realm:

| Field | Value |
| --- | --- |
| Username | `mapper.admin` |
| Password | `mapper-admin` |

## Clients

| Client | Type | Usage |
| --- | --- | --- |
| `mapper.swagger` | Public, Authorization Code + PKCE | Swagger UI and browser frontend development. |
| `mapper.client` | Confidential, client credentials | Service-to-service calls. |

The dev-only secret for `mapper.client` is `mapper-client-dev-secret`.

## Browser flow

Use Authorization Code + PKCE with scopes:

```text
openid profile api
```

Swagger is configured to use:

```text
http://localhost:8080/auth/realms/mapper/protocol/openid-connect/auth
http://localhost:8080/auth/realms/mapper/protocol/openid-connect/token
```

## Service token example

```bash
curl -X POST "http://localhost:8080/auth/realms/mapper/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=mapper.client" \
  -d "client_secret=mapper-client-dev-secret" \
  -d "grant_type=client_credentials" \
  -d "scope=api"
```

Use the returned token with the API:

```http
Authorization: Bearer <access_token>
```

## WebApi settings

Inside Docker, WebApi downloads Keycloak metadata through the Docker network but validates tokens against the public local issuer:

```text
Jwt__Authority=http://localhost:8080/auth/realms/mapper
Jwt__MetadataAddress=http://keycloak:8080/auth/realms/mapper/.well-known/openid-configuration
Jwt__Audience=api
SwaggerOAuth__Authority=http://localhost:8080/auth/realms/mapper
SwaggerOAuth__ClientId=mapper.swagger
```

This avoids the common local Docker problem where the browser uses the reverse proxy on `localhost:8080`, while the API container must reach Keycloak by service name for metadata.
