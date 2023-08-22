# Solv API

Following document presents the capabilities of the Solv API that can be used for integration with 3rd party systems.

## Managament API

System exposes several endpoints.
All endpoints accept only `application/json` requests.
All endpoints require ApiKey authorization. Following can be achieved by either:
 - request has to supply following header: `Authorization: ApiKey api-key-supplied-to-you`
 - request has to supply following query parameter: `https://api.solvnow.com/v1/tickets?apiKey=api-key-supplied-to-you`

### Creating a ticket

`POST /v1/tickets` - creates a ticket in Solv. Required payload structure:
```
{
  "question": "I have a problem with my query. Can you assist?",
  "email": "ken.adams@gmail.com",
  "firstname": "Ken",
  "lastName": "Adams",
  "referenceId" : "ref-004",
  "source": "chat",
  "transportType": "chat",
  "metadata": {
      "serialNumber": "123-456-XYZ",
      "model": "Proto-X5"
  }
}
```
| Item		      	| Description																| Mandatory |
|:----------------|:--------------------------------------------------------------------------|---|
| `question`   	  | The query raised by a customer | yes |
| `email`		      | The email of the customer raising the query | yes |
| `firstname`	    | First name of the customer raising the query	| yes |
| `lastName`		  | Last name of the customer raising the query						| yes |
| `referenceId`		| Reference for the ticket in 3rd party (yours) system | no |
| `source`			  | The source of the ticket | no |
| `transportType`			  | The transport used to continue conversation on the ticket. Can be eithet `chat` or `email` | no |
| `metadata`			  | The additional Metadata provided for the ticket. This is on object with user defined key-value data. They will be displayed e.g. in the admin dashboard to provider better intel on the ticket. | no |

Sample response:
```
{
  "id": "a606ea6a-df79-4ed1-b864-c97a7e43d0ab",
  "token": "xxxx",
  "chatUrl": "https://solvnow.com/chat/{id}/{token}"
}
```

| Item		      	| Description																|
|:----------------|:--------------------------------------------------------------------------|
| `id`   	        | Id of the ticket created in Solv |
| `token`		      | Private token for the ticket so customer can view the chat with customer |
| `chatUrl`	      | URL for the customer chat (can be e.g. embedded in iframe in your system)	|