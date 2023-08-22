# WebHooks in Solv

Solv implements a [webhooks](https://en.wikipedia.org/wiki/Webhook) mechanism that allows third-party system to integrate and react to events that occur in Solv platform. This document briefly summarizes technical details of it and should help you get on the right path with implementing that integration in your system.

## Managament API

System exposes several endpoints allowing subscribing, listing and unsubcribing from the events.
All endpoints accept only `application/json` requests.
All endpoints require ApiKey authorization. Following can be achieved by either:
 - request has to supply following header: `Authorization: ApiKey api-key-supplied-to-you`
 - request has to supply following query parameter: `https://api.solvnow.com/v1/webhooks?apiKey=api-key-supplied-to-you`

### Registering a webhook

`POST /v1/webhooks` - registers new webhook for the client. Required payload structure:
```
{
  "eventType": "TicketStatusChangedEvent",
  "url": "https://your-system.com/handler-endpoint",
  "body": "{ \"format\": \"of_your_notification_message_here\",  }",
  "verb": "post",
  "contentType": "application/json",
  "secret": "secret",
  "authorization": "Basic dGVzdA=="
}
```
| Item		      	| Description																| Mandatory |
|:----------------|:--------------------------------------------------------------------------|---|
| `eventType`   	| Event type you desire to listen for								| yes |
| `url`		        | Url of the handler in your system							| yes |
| `body`	      	| Template of the body of notification. It uses [Liquid markup](https://shopify.github.io/liquid/). Placeholders for Solv system values can be used here. Available placeholders are described below for specific events					| yes |
| `verb`		      | Verb of notification that will be called (e.g. `PUT` or `POST`)						| yes |
| `contentType`		| `Content-Type` header content that will be provided | yes |
| `secret`			  | Secret that would be used as the key for generating signature of the delivered notifications| no |
| `authorization`	| `Authorization` header content that will be provided | no |

### Listing registered webhooks

`GET /v1/webhooks`	- lists all registered webhooks for the client

### Removing registered webhooks

`DELETE /v1/webhooks/{id}`	- unsubscribe existing webhook

## Notification

Once you registered a webhook in the system, it will be called once an event that it is listening for occurs. The ideal handling of those would be to store it's contents in your system and process them later on, so that retrieving notification would be as quick and as error-free as it can get. Below you can find description of those notifications.

### Headers

| Header		        	| Description																|
|:--------------------|:--------------------------------------------------------------------------|
| `X-Solv-Signature`	| contains signature of the body	|
| `User-Agent`        | contains information what type of agent made the notification								|
| `Content-Type`      | as provided when registering the WebHook |
| `Authorization`     | as provided when registering the WebHook |

#### Verification of signature

The `X-Solv-Signature` header contains the HMAC hex digest of the response body. This header will be sent if the webhook is configured with a secret. The HMAC hex digest is generated using the `sha1` hash function and the `secret` as the HMAC key (similiar to [GitHub's implementation ](https://developer.github.com/webhooks/securing/#validating-payloads-from-github))

### Body

Every webhook notification has the following data structure:

```
{
  "eventId": "ef8e7de5-88fb-401b-bb3b-ae3bf45e70fb",
  "eventType": "TicketStatusChangedEvent",
  "timestamp": "2019-09-23T08:02:11.004467Z",
  "data": { ... }
}
```

| Item			| Description																|
|:--------------|:--------------------------------------------------------------------------|
| `eventId`		| globally unique identifier of the event	|
| `eventType` 	| event type that triggered the notification								|
| `timestamp`		| timestamp of an event														|
| `data`			| payload of the event (actual data, depends on the `eventType`)				|


#### TicketStatusChangedEvent

This is a notification event that is being sent every time a ticket changes it's status in the system. The `data` structure of the notification is as follows:

```
"data": {
  "ticketId": "6cc10108-8eec-4bd7-94a6-c6736ca175a1",
  "referenceId": "ext-ref-1",
  "fromStatus": "Assigned",
  "toStatus": "Solved",
  "escalationReason": "OpenTimeExceeded",
  "transcript": "chat transcript"
}
```

| Item			| Description																|
|:--------------|:--------------------------------------------------------------------------|
| `ticketId`		      | globally unique identifier of the ticket									|
| `referenceId`     	| external reference of the ticket (if passed during creation)				|
| `fromStatus` 	      | status that ticket has transitioned from									|
| `toStatus`		      | status that ticket has transitioned to									|
| `escalationReason`	| if ticket hit status `Escalated`, this field will contain the reason for escalation|
| `transcript`		    | if ticket hit status `Escalated`, this field will contain the transcript of the conversation between the customer and the solver(s) |

##### Proposed `body` for notification

```
{
  "eventId": {{ eventId | json }},
  "eventType": {{ eventType | json }},
  "timestamp": {{ timestamp | isodate | json }},
  "data": {
    "ticketId": {{ data.ticketId | json }},
    "fromStatus": {{ data.fromStatus | json }},
    "toStatus": {{ data.toStatus | json }},
    {% if data.toStatus == "Escalated" %}
      "escalationReason": {{ data.toStatus | json }},
      "transcript": {{ data.transcript | json }},
    {% endif %}
    "referenceId": {{ data.referenceId | json }}
  }
}
```

#### TicketTagsChangedEvent

This is a notification event that is being sent every time a ticket's tags are changed in the system. The `data` structure of the notification is as follows:

```
"data": {
  "ticketId": "6cc10108-8eec-4bd7-94a6-c6736ca175a1",
  "referenceId": "ext-ref-1",
  "previousTags": ["linux", "software", "ios"],
  "currentTags": ["linux", "software", "shell", "ubuntu"],
  "removedTags": ["ios"],
  "addedTags": ["shell", "ubuntu"]
}
```

| Item			| Description																|
|:--------------|:--------------------------------------------------------------------------|
| `ticketId`		      | globally unique identifier of the ticket									|
| `referenceId`     	| external reference of the ticket (if passed during creation)				|
| `previousTags` 	      | list of tags assigned to the ticket before change									|
| `currentTags`		      | list of tags assigned to the ticket after change									|
| `removedTags`	| list of tags removed from the ticket by the change |
| `addedTags`		    | list of tags added to the ticket by the change |

##### Proposed `body` for notification

```
{
  "eventId": {{ eventId | json }},
  "eventType": {{ eventType | json }},
  "timestamp": {{ timestamp | isodate | json }},
  "data": {
    "addedTags": {{ data.addedTags | json }},
    "removedTags": {{ data.removedTags | json }},
  }
}
```
